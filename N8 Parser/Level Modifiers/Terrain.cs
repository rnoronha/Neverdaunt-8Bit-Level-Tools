using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace N8Parser.Level_Modifiers
{
    public class Terrain
    {
        public static Heightmap Map;
        private static Random rand;
        private static double vstep;
        private static HashSet<Point> Holds;

        //from here: http://terraineer.sourceforge.net/plugins/hills.php
        public static Heightmap Hills(Heightmap seed, int numHills, double Steepness = 1, double ValleyProb = 0, int MaxRadius = 25, bool ellipse = false, HashSet<Point> holds = null, Random Random = null)
        {
            Map = new Heightmap(seed.Map, holds??new HashSet<Point>());
            Random rand = Random??new Random();

            for (int c = 0; c < numHills; c++)
            {
                Hill(Steepness, ValleyProb, MaxRadius, ellipse, rand);
            }

            return Map;
        }

        //From here: http://terraineer.sourceforge.net/plugins/fault.php
        public static Heightmap Faults(Heightmap seed, int numFaults, int Displacement = 2, HashSet<Point> holds = null, Random random = null)
        {
            Map = new Heightmap(seed.Map, holds ?? new HashSet<Point>());
            Random rand = random ?? new Random();

            for (int f = 0; f < numFaults; f++)
            {
                Fault(Displacement, rand);
            }

            return Map;
        }

        //Executes various different heightmap forming functions at random
        public static Heightmap Arbitrary(Heightmap seed, SortedList<double, Action> functions, int iterations = 10000, Random random = null, Func<int, bool> StepCallback = null)
        {
            Map = seed;
            Random rand = random ?? new Random();

            //Want to make sure that they're sorted in descending order and normalized.
            double total = functions.Aggregate<KeyValuePair<double, Action>, double>(0, (current, next) => current + next.Key);
            var actionsSorted = (from kvp in functions select new {Key = kvp.Key/total, 
                                                                  Value = kvp.Value}).OrderBy((x)=>x.Key);

            List<int> actionsDone = new List<int>(functions.Count);
            for (int i = 0; i < functions.Count; i++)
            {
                actionsDone.Add(0);
            }

            for (int i = 0; i < iterations; i++)
            {
                //try to "roll under" the key value; we perform the first action whose key + cumulative value is higher than this value.
                double randomValue = rand.NextDouble();
                double cumulativeValue = 0;

                for(int j = 0; j < actionsSorted.Count(); j++)
                {
                    var kvp = actionsSorted.ElementAt(j);
                    cumulativeValue += kvp.Key;
                    if (randomValue < cumulativeValue)
                    {
                        actionsDone[j]++;
                        kvp.Value();
                        break;
                    }
                }

                if (StepCallback != null)
                {
                    bool Continue = StepCallback(i);
                    if (!Continue)
                        break;
                }
            }
            


            return Map;
        }


        public static void Fault(double DisplacementRatio = 0.01, Random rand = null)
        {
            int sizeX = Map.sizeX;
            int sizeY = Map.sizeY;
            rand = rand ?? new Random();

            int x1 = Map.WrapX(rand.Next());
            int x2 = Map.WrapX(rand.Next());
            int y1 = Map.WrapY(rand.Next(), rand.Next());
            int y2 = Map.WrapY(rand.Next(), rand.Next());

            int Displacement = (int)((Map.max() - Map.min()) * DisplacementRatio);
            Console.WriteLine("Max is: " + Map.max() + ", min is: " + Map.min() + ", displacement is: " + Displacement);

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if ((x2 - x1) * (y - y1) - (y2 - y1) * (x - x1) > 0)
                        Map[x, y] += Displacement;
                    else
                        Map[x, y] -= Displacement;
                }
            }

        }

        public static void Normalize(int min, int max)
        {
            Console.WriteLine("Normalizing...");
            Console.WriteLine("Before - Max is: " + Map.max() + ", min is: " + Map.min());
            Map = Map.normalize(min, max);
            Console.WriteLine("After - Max is: " + Map.max() + ", min is: " + Map.min());
        }

        public static void Hill(double Steepness = 1, double ValleyProb = 0, int MaxRadius = 25, bool ellipse = false, Random Random = null)
        {
            Random rand = Random ?? new Random();
            int sizeX = Map.sizeX;
            int sizeY = Map.sizeY;

            bool IsValley = rand.NextDouble() < ValleyProb;
            int valleyMul = 1;
            if (IsValley)
            {
                valleyMul *= -1;
            }

            //Calculate both random values always to make this more stable between true and false
            int radiusX = rand.Next(MaxRadius);
            int radiusY = rand.Next(MaxRadius);
            if (!ellipse)
            {
                radiusY = radiusX;
            }


            //Pick a point
            int x = Map.WrapX(rand.Next());
            int y = Map.WrapY(rand.Next(), rand.Next());


            //And then gradually raise a hill at that point
            for (int i = x - radiusX; i < x + radiusX; i++)
            {
                for (int j = y - radiusY; j < y + radiusY; j++)
                {
                    int mag = radiusX * radiusY - ((i - x) * (i - x) + (j - y) * (j - y));
                    if (mag >= 0)
                    {
                            Map[i, j] += (int)(mag * Steepness * valleyMul);
                    }
                }
            }
        }

        /// <summary>
        /// Trims the bitmap so that it fits inside a bounding box; only looks at black pixels, so you might want to process it first.
        /// </summary>
        /// <param name="input">The bitmap to trim</param>
        /// <returns>The trimmed bitmap</returns>
        public static Bitmap TrimBounds(Bitmap input)
        {
            Point max = new Point(0, 0);
            Point min = new Point(input.Width, input.Height);
            
            for (int i = 0; i < input.Width; i++)
            {
                for (int j = 0; j < input.Height; j++)
                {
                    if (input.GetPixel(i, j) == Color.FromArgb(0, 0, 0))
                    {
                        max.X = Math.Max(i, max.X);
                        max.Y = Math.Max(j, max.Y);
                        min.X = Math.Min(i, min.X);
                        min.Y = Math.Min(j, min.Y);
                    }
                }
            }

            //If we didn't find any black pixels, just return a copy of the input - we can't trim it.

            if(max == new Point(0,0) && min == new Point(input.Width, input.Height))
            {
                return new Bitmap(input);
            }

            Bitmap ret = new Bitmap(1+max.X - min.X, 1+max.Y - min.Y);

            for (int i = min.X; i <= max.X; i++)
            {
                for (int j = min.Y; j <= max.Y; j++)
                {
                    int x = i - min.X;
                    int y = j - min.Y;

                    ret.SetPixel(x, y, input.GetPixel(i, j));
                }
            }

            return ret;
        }


        /// <summary>
        /// Takes the map provided by the N8 server, which has each cell represented by an 8x8
        /// pixel square, and shrinks it down such that each cell is represented by a single pixel.
        /// </summary>
        /// <param name="path">Path to the N8 map; assumes each cell is represented by 8x8 pixels.</param>
        /// <param name="scale">Scale by which the map should be minimized; default 8 because that's what the server uses</param>
        /// <returns>A minimized version of the map</returns>
        public static Bitmap MinimizeMap(string path, int scale = 8)
        {
            return MinimizeMap(new Bitmap(path), scale);
        }

        public static Bitmap MinimizeMap(Bitmap input, int scale = 8)
        {
            Bitmap ret = new Bitmap((input.Width / scale), (input.Height / scale));

            for (int i = 0; i < ret.Width; i++)
            {
                for (int j = 0; j < ret.Height; j++)
                {
                    ret.SetPixel(i, j, input.GetPixel(i * scale, j * scale));
                }
            }

            return ret;
        }

        /// <summary>
        /// Expands a bitmap by a certain scale
        /// </summary>
        /// <param name="input">The bitmap to expand</param>
        /// <param name="scale">The scale by which to expand</param>
        /// <returns></returns>
        public static Bitmap ExpandMap(Bitmap input, int scale = 10)
        {
            Bitmap ret = new Bitmap(input.Width * scale, input.Height * scale);

            for (int i = 0; i < ret.Width; i++)
            {
                for (int j = 0; j < ret.Height; j++)
                {
                    ret.SetPixel(i, j, input.GetPixel(i / scale, j / scale));
                }
            }

            return ret;
        }

        /// <summary>
        /// Turns blank cells that are adjacent to a non-blank cell black - in other words, outlines the map.
        /// </summary>
        /// <param name="input">The map to outline</param>
        /// <returns>The outlined map</returns>
        public static Bitmap OutlineMap(Bitmap input)
        {
            Bitmap ret = new Bitmap(input);
            for (int i = 0; i < input.Width; i++)
            {
                for (int j = 0; j < input.Height; j++)
                {
                    Color current = ret.GetPixel(i, j);
                    //Check to see if empty pixels are borders
                    if (current == Color.FromArgb(0, 0, 0, 0))
                    {
                        for (int x = i - 1; x <= i + 1; x++)
                        {
                            for (int y = j - 1; y <= j + 1; y++)
                            {
                                //Don't test yourself obvs
                                if (!((x == 0) && (y == 0)))
                                {
                                    //Invalid states, skip them
                                    if (x < 0 || x >= input.Width || y < 0 || y >= input.Height)
                                    {
                                        continue;
                                    }

                                    //If we're next to a non-blank or black pixel, color us in
                                    if (ret.GetPixel(x, y) != Color.FromArgb(0, 0, 0, 0) &&
                                        ret.GetPixel(x, y) != Color.FromArgb(255, 0, 0, 0))
                                    {
                                        ret.SetPixel(i, j, Color.FromArgb(255, 0, 0, 0));
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Processes the map to make it fit for consumption. Does a couple of things:
        /// 1. Turns all build-in-able cells green (argb 255, 0, 255, 0)
        /// 1a. This means team cells, elite cells, construct cells and public cells
        /// 2. Turns all not-build-in-able cells red (argb 255, 255, 0)
        /// </summary>
        /// <param name="input">A 40x40 N8 landmass bitmap</param>
        /// <returns>The bitmap, modified as stated in the summary</returns>
        public static Bitmap ProcessMap(Bitmap input)
        {
            //Magic numbers! These are the colors for:
            /* 1. Normal cells
             * 2. Elite cells
             * 3. Make-Elite cells
             * 4. Public cells
             * 5. Construct cells
             * 6. Red team cells
             * 7. Blue team cells
             * 8. Green team cells
            */
            List<Color> BuildableCells = new List<Color>(8);
            BuildableCells.Add(Color.FromArgb(34,139,34));
            BuildableCells.Add(Color.FromArgb(128,128,128));
            BuildableCells.Add(Color.FromArgb(255, 0, 255));
            BuildableCells.Add(Color.FromArgb(0,255,255));
            BuildableCells.Add(Color.FromArgb(224,255,255));
            BuildableCells.Add(Color.FromArgb(139,0,0));
            BuildableCells.Add(Color.FromArgb(0,0,139));
            BuildableCells.Add(Color.FromArgb(0,100,0));

            //More magic numbers! These are the colors for:
            /* 1. Locked cells
             * 2. Safe cells
             * 3. Sacred cells
            */
            List<Color> NonBuildableCells = new List<Color>(3);
            NonBuildableCells.Add(Color.FromArgb(135,206,235));
            NonBuildableCells.Add(Color.FromArgb(250,250,210));
            NonBuildableCells.Add(Color.FromArgb(173,255,47));

            Bitmap ret = new Bitmap(input);
            for (int i = 0; i < input.Width; i++)
            {
                for (int j = 0; j < input.Height; j++)
                {
                    Color current = ret.GetPixel(i, j);

                    if (BuildableCells.Contains(current))
                    {
                        ret.SetPixel(i, j, Color.FromArgb(0,255,0));
                    }
                    else if(NonBuildableCells.Contains(current))
                    {
                        ret.SetPixel(i, j, Color.FromArgb(255,0,0));
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Converts a bitmap to a hold list; only red squares are held, the rest is free.
        /// </summary>
        /// <param name="input">A processed bitmap at 1 cell = 10 pixels scale with cells to be held in red</param>
        /// <returns>A list of holds at a 1 cell = 10 points scale</returns>
        public static HashSet<Point> BitmapToHolds(Bitmap input)
        {
            HashSet<Point> ret = new HashSet<Point>();

            for (int i = 0; i < input.Width; i++)
            {
                for (int j = 0; j < input.Height; j++)
                {
                    if (input.GetPixel(i, j) == Color.FromArgb(255, 0, 0))
                    {
                        ret.Add(new Point(i, j));
                    }
                }
            }

            return ret;
        }


        //Just shows a bitmap. Same thing as in the Heightmap class. Should remove later.
        public static void ShowBitmap(Bitmap me)
        {
            string temp_path = System.IO.Path.GetTempFileName() + ".png";

            me.Save(temp_path, System.Drawing.Imaging.ImageFormat.Png);
            System.Diagnostics.Process.Start(temp_path);
        }

        /// <summary>
        /// Generates a heightmap using the diamond-square method explained here: http://www.gameprogrammer.com/fractal.html
        /// For any given input size, it returns a square heightmap of dimension 2^size + 1
        /// </summary>
        /// <param name="ReqSize">The size of the resultant heightmap; it will be 2^size+1</param>
        /// <param name="variability">The initial variability in height</param>
        /// <param name="seed">The seeded values</param>
        /// <param name="roughness">How quickly the variability drops off; should be less than one</param>
        /// <param name="holds">Set of points whose seeded value should be maintained</param>
        /// <returns></returns>
        public static Heightmap DiamondSquare(int ReqSize, int variability, double roughness, Heightmap seed = null, HashSet<Point> holds = null, Random Random = null)
        {
            int size = (1 << ReqSize);
            
            List<List<int>> map = (seed ?? new Heightmap(size)).Map;

            for (int i = 0; i < size; i++)
            {
                if (map.Count <= i)
                {
                    map.Add(new List<int>(size));
                }

                if (map[i].Count < size)
                {
                    for (int j = 0; j < size; j++)
                    {
                        map[i].Add(0);
                    }
                }
            }
            if(seed == null)
            {
                //A default seeding - mountains in the "middle" (actually corners, but whatever).
                map[0][0] = 1000;
                map[0][size-1] = 1000;
                map[size-1][0] = 1000;
                map[size-1][size-1] = 1000;
            }

            int step = size/2;
            Map = new Heightmap(map);
            Holds = holds ?? new HashSet<Point>();
            vstep = roughness;
            rand = Random??new Random();
            DiamondSquare_Recursive(step, variability, size/2, size/2);

            return Map;
        }

        private static void DiamondSquare_Recursive(int step, int variability, int i, int j)
        {
            //Do the square averageing

            int SquareAvg = 0;
            for(int k = -1; k <=1; k+=2)
            {
                for(int l = -1; l <=1; l+=2)
                {
                    SquareAvg += Map[i + k * step, j + l * step];
                }
            }
            // Add the square averaging so we maintian whatever value was seeded here before.
            Map[i, j] += SquareAvg/4 + rand.Next(-variability, variability);
            
            //Do the diamond averaging - keep in mind that we might fall off the edge of the world, 
            //in which case the heightmap wraps around

            for (int k = -1; k <= 1; k += 2)
            {
                int DiamondAvgI = 0;
                int DiamondAvgJ = 0;
                for (int l = -1; l <= 1; l += 2)
                {
                    DiamondAvgI += Map[i + k * step, j + l * step];
                    DiamondAvgI += Map[i + k * step + l * step, j];

                    DiamondAvgJ += Map[i + l * step, j + k * step];
                    DiamondAvgJ += Map[i, j + k * step + l * step];
                }

                
                Map[i, j + k * step] = DiamondAvgI/4 + rand.Next(-variability, variability);
                Map[i + k * step, j] = DiamondAvgJ/4 + rand.Next(-variability, variability);
            }

            //And then do the same thing for each quadrant
            if (step > 1)
            {
                int newStep = step / 2;
                int newVar = (int)((double)variability * vstep);
                DiamondSquare_Recursive(newStep, newVar, i + newStep, j + newStep);
                DiamondSquare_Recursive(newStep, newVar, i - newStep, j + newStep);
                DiamondSquare_Recursive(newStep, newVar, i + newStep, j - newStep);
                DiamondSquare_Recursive(newStep, newVar, i - newStep, j - newStep);
            }
        }

        public static List<List<Heightmap>> ChopHeightmap(int SquareSize, Heightmap input)
        {
            List<List<Heightmap>> ret = new List<List<Heightmap>>();
            List<List<int>> rawmap = input.Map;

            IEnumerable<List<int>> RestColumns;
            IEnumerable<IEnumerable<int>> RestRows;
            RestColumns = rawmap;
            int count = 0;
            do
            {
                IEnumerable<List<int>> CurrentColumns = RestColumns.Take(SquareSize);
                RestColumns = RestColumns.Skip(SquareSize);

                ret.Add(new List<Heightmap>());
                RestRows = CurrentColumns;

                do
                {

                    IEnumerable<IEnumerable<int>> CurrentRows = RestRows.Select((x) => x.Take(SquareSize));
                    RestRows = RestRows.Select((x) => x.Skip(SquareSize));

                    List<List<int>> CurrentMap = CurrentRows.Select((x) => x.ToList()).ToList();
                    ret[count].Add(new Heightmap(CurrentMap));
                } while (RestRows.Max((x)=>x.Count()) > 0);

                count++;

            } while (RestColumns.Count() > 0);
            
            return ret;
        }
    }
    public class Heightmap
    {
        private List<List<int>> _map;
        private HashSet<Point> HeldValues = new HashSet<Point>();
        private int myXSize;
        private int myYSize;

        public HashSet<Point> Holds
        {
            get
            {
                return HeldValues;
            }
            set
            {
                HeldValues = value;
            }
        }

        public Heightmap()
        {
            _map = new List<List<int>>();
            myXSize = GetSizeX();
            myYSize = GetSizeY();
        }
         
        public Heightmap(List<List<int>> seed)
        {
            _map = seed;
            myXSize = GetSizeX();
            myYSize = GetSizeY();
        }

        public Heightmap(List<List<int>> seed, HashSet<Point> holds)
        {
            _map = seed;
            HeldValues = holds;
            myXSize = GetSizeX();
            myYSize = GetSizeY();
        }

        public Heightmap(int size)
        {
            _map = new List<List<int>>(size);
            for (int i = 0; i < size; i++)
            {
                _map.Add(new List<int>(size));
                for (int j = 0; j < size; j++)
                {
                    _map[i].Add(0);
                }
            }
            myXSize = GetSizeX();
            myYSize = GetSizeY();
        }

        public Heightmap(int xSize, int ySize, HashSet<Point> holds = null)
        {
            this.HeldValues = holds ?? new HashSet<Point>();
            _map = new List<List<int>>(xSize);
            for (int i = 0; i < xSize; i++)
            {
                _map.Add(new List<int>(ySize));
                for (int j = 0; j < ySize; j++)
                {
                    _map[i].Add(0);
                }
            }
            myXSize = GetSizeX();
            myYSize = GetSizeY();
        }


        public Heightmap(Bitmap input)
        {
            _map = new List<List<int>>(input.Width);
            for (int i = 0; i < input.Width; i++)
            {
                _map.Add(new List<int>(input.Height));
                for (int j = 0; j < input.Height; j++)
                {
                    _map[i].Add(input.GetPixel(i, j).ToArgb());
                }
            }
        }

        public Bitmap ToBitmap()
        {
            Bitmap ret = new Bitmap(Map.Count, Map.Max((x) => x.Count));

            for (int i = 0; i < Map.Count; i++)
            {
                for (int j = 0; j < Map[i].Count; j++)
                {
                    ret.SetPixel(i, j, Color.FromArgb(this[i, j]));
                }
            }

            return ret;
        }

        public void ShowHeatmap(int GridSize = 0, Bitmap outline = null)
        {
            Bitmap me = ToHeatmap(GridSize, outline);
            string temp_path = System.IO.Path.GetTempFileName() + ".bmp";

            me.Save(temp_path, System.Drawing.Imaging.ImageFormat.Bmp);
            System.Diagnostics.Process.Start(temp_path);
        }

        public Bitmap ToHeatmap(int GridSize, Bitmap outline)
        {
            int scalemax = 510;
            Bitmap image = new Bitmap(Map.Count, Map.First().Count);
            var temp = GetNormalizingFactors(scalemax);
            Color[] colors = new Color[511];

            bool grBorder = false;

            for (int i = 0; i < colors.Length; i++)
            {
                int blue = -1;
                int green = -1;
                int red = -1;

                if (!grBorder)
                {
                    blue = 255 - i;
                    if (blue < 0)
                    {
                        blue = 0;
                    }

                    green = 255 - blue;
                    if (green < 0)
                    {
                        green = 0;
                    }

                    red = 0;

                    if (green == 255)
                    {
                        grBorder = true;
                    }
                }
                else if (grBorder)
                {
                    green = 255 - (i - 255);
                    if (green < 0)
                    {
                        green = 0;
                    }

                    red = 255 - green;
                    if (red < 0)
                    {
                        red = 0;
                    }

                    blue = 0;
                }



                byte B = (byte)(blue);
                byte G = (byte)(green);
                byte R = (byte)(red);

                colors[i] = Color.FromArgb(R, G, B);
            }
            colors = colors.OrderBy((c) => c.R).ThenBy((c) => c.G).ThenBy((c) => c.B).ToArray();

            int offset = temp.Item1;
            double scale = temp.Item2;

            for (int x = 0; x < Map.Count; x++)
            {
                for (int y = 0; y < Map[x].Count; y++)
                {
                    
                    int scaled = (int)((this[x, y] + offset) * scale);
                    
                    if (scaled > scalemax || scaled < 0)
                    {
                        throw new Exception("Improperly scaled value!");
                    }
                    Color heat = colors[scaled];

                    if (GridSize > 0)
                    {
                        if (x % GridSize == 0 || y % GridSize == 0)
                        {
                            heat = Color.Black;
                        }
                    }

                    if (outline != null)
                    {
                        if (outline.GetPixel(x, y) == Color.FromArgb(255, 0, 0, 0))
                        {
                            heat = Color.Black;
                        }
                    }

                    if (HeldValues.Contains(new Point(x, y)))
                    {
                        heat = Color.Gray;
                    }
                    

                    image.SetPixel(x, y, heat);
                }
            }

            return image;


        }

        public int min()
        {
            return _map.Min((x) => (x.Min((y) => y)));
        }

        public int max()
        {
            return _map.Max((x) => (x.Max((y) => y)));
        }

        public Tuple<int, double> GetNormalizingFactors(int reqMax)
        {
            int myMin = min();
            int myMax = max();
            //Two O(n^2) calls! Whoo hoo!

            //Calculate the offset - what do we add to the current minimum to get it up to the new minimum?
            int offset = -myMin;
            //Calculate our scaling factor - what do we multiply the current maximum by (plus the offset) to get to the new maximum?
            double scale = (double)(reqMax) / (double)(myMax + offset);

            //This runs in to problems if my min and my max are zero - at that point scale should be zero too.
            if (myMax == 0 && myMin == 0)
            {
                scale = 0;
            }

            return Tuple.Create(offset, scale);

        }

        public Heightmap normalize(int min, int max)
        {

            var temp = GetNormalizingFactors(max - min);
            int offset = temp.Item1;
            double scale = temp.Item2;

            _map = _map.Select((x) => x.Select((y) => (int)(Math.Round(((double)y + offset) * scale) + min)).ToList()).ToList();

            return this;
        }

        public int this[int i, int j]
        {
            
            set 
            {
                i = WrapX(i);
                j = WrapY(i, j);
                
                if(!HeldValues.Contains(new Point(i,j)))
                {
                    _map[i][j] = value; 
                }
            }
            get 
            {
                i = WrapX(i);
                j = WrapY(i, j);   
                return _map[i][j];
            }
        }

        public int WrapX(int x)
        {
            while (x < 0)
            {
                x += Map.Count;
            }

            x = x % Map.Count;

            return x;
        }

        public int WrapY(int x, int y)
        {
            x = WrapX(x);
            while (y < 0)
            {
                y += Map[x].Count;
            }

            y = y % Map[x].Count;

            return y;
        }

        public int sizeX
        {
            get
            {
                return myXSize;
            }
        }

        public int sizeY
        {
            get
            {
                return myYSize;
            }
        }


        private int GetSizeX()
        {
            return _map.Count;
        }

        private int GetSizeY()
        {
            return _map.Max((x) => (x.Count));
        }

        public List<List<int>> Map
        {
            get { return _map; }
        }
    }
}
