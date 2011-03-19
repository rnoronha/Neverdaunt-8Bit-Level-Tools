using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace N8Parser.Terrain
{
    
    [Serializable]
    public class Heightmap
    {
        //Maximum and minimum values the heightmap can take
        private int? _maxValue;
        private int? _minValue;

        public int? MaxValue
        {
            get
            {
                return _maxValue;
            }
            set
            {
                _maxValue = value;
            }
        }

        public int? MinValue
        {
            get
            {
                return _minValue;
            }

            set
            {
                _minValue = value;
            }
        }


        public enum OutOfBoundsType
        {
            Zeros,
            Error,
            Wrap,
            Mirror
        }

        private OutOfBoundsType _oobType;

        public OutOfBoundsType OOBType
        {
            set
            {
                _oobType = value;
            }

            get
            {
                return _oobType;
            }
        }

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
            myXSize = GetSizeX();
            myYSize = GetSizeY();
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

            //If the difference between MyMax and MyMin is really really big (like say MyMax and MyMin are both close to int.MaxValue and int.MinValue respectively)
            //then we can get really bad values. What to do then?

            //This runs in to problems if my min and my max are equal - at that point scale should be zero.
            if (myMax == myMin)
            {
                scale = 0;
            }

            //And then do some sanity checking to see if these results are sensible
            if ((scale < 0) || (double.IsNaN(scale)) || (double.IsInfinity(scale)))
            {
                throw new Exception("Scale is nonsense!");
            }

            return Tuple.Create(offset, scale);

        }

        private Heightmap AverageReduction(int sizeX, int sizeY)
        {
            Heightmap ret = new Heightmap(sizeX, sizeY);
            int WindowLengthX = myXSize - sizeX;
            int WindowLengthY = myYSize - sizeY;

            //Store the old out of bounds behavior
            OutOfBoundsType mine = this.OOBType;
            OutOfBoundsType rets = ret.OOBType;

            //Make sure we don't wrap around for the duration of this thingy
            this.OOBType = OutOfBoundsType.Error;
            ret.OOBType = OutOfBoundsType.Error;

            for (int i = 0; i < ret.myXSize; i++)
            {
                for (int j = 0; j < ret.myYSize; j++)
                {
                    int total = 0;
                    for (int l = i; l < i + WindowLengthX; l++)
                    {
                        for (int k = j; k < j + WindowLengthY; k++)
                        {
                            total += this[l, k];
                        }
                    }

                    ret[i, j] = total / (WindowLengthX * WindowLengthY);
                }
            }
            ret.OOBType = rets;
            this.OOBType = mine;

            return ret;
        }

        private Heightmap BicubicReduction(int sizeX, int sizeY)
        {
            Heightmap ret = new Heightmap(sizeX, sizeY);
            
            double[][] window = new double[4][];
            for (int i = 0; i < 4; i++)
            {
                window[i] = new double[4];
            }

            int diffX = myXSize - sizeX;
            int diffY = myYSize - sizeY;

            double xOffset = 0;
            double yOffset = 0;

            //If we're stepping down an odd number of steps, the new points are in between old points. Otherwise they're exactly on them.
            if (diffX % 2 == 1)
            {
                xOffset = 0.5;
            }

            if (diffY % 2 == 1)
            {
                yOffset = 0.5;
            }

            

            for (int i = 0; i < ret.myXSize; i++)
            {
                for (int j = 0; j < ret.myYSize; j++)
                {
                    //do something that fills the window
                    for (int k = 0; k < 4; k++)
                    {
                        int x;
                        int y;
                        if(xOffset == 0.5)
                        {
                            x = k - 2;
                        }
                        else
                        {
                            x = k - 1;
                        }

                        for (int l = 0; l < 4; l++)
                        {

                            if(yOffset == 0.5)
                            {
                                y = l - 2;
                            }
                            else
                            {
                                y = l - 1;
                            }

                            window[k][l] = this[x + i, y + j];
                        }
                    }


                    ret[i, j] = (int)Math.Round(Terrain.BicubicInterpolate(window, xOffset, yOffset));
                }
            }

            return ret;
        }

        public Heightmap reduceTo(int sizeX, int sizeY)
        {
            //short circuit if we're just suppsoed to resize to our own size
            if ((myXSize == sizeX) && (myYSize == sizeY))
            {
                return this;
            }
            else if ((myXSize < sizeX) || (myYSize < sizeY))
            {
                throw new Exception("This is a reduction function, not a grow function! God geez!");
            }

            //return AverageReduction(sizeX, sizeY);
            return BicubicReduction(sizeX, sizeY);
        }

        public List<Heightmap> serialReduction(int minSizeX, int minSizeY)
        {
            List<Heightmap> ret = new List<Heightmap>();
            ret.Add(this);

            while (ret.Last().sizeX >= minSizeX)
            {
                ret.Add(ret.Last().reduceTo(ret.Last().sizeX - 1, ret.Last().sizeY - 1));
            }

            return ret;
        }

        public Heightmap normalize(int min, int max)
        {

            var temp = GetNormalizingFactors(max - min);
            int offset = temp.Item1;
            double scale = temp.Item2;

            for (int i = 0; i < this.sizeX; i++)
            {
                for (int j = 0; j < this.sizeY; j++)
                {
                    int h = this[i,j];
                    this[i, j] = (int)(Math.Round(((double)h + offset) * scale) + min);
                }
            }

            return this;
        }

        public int this[int i, int j]
        {
            
            set 
            {
                int x; 
                int y;
                switch (this.OOBType)
                {
                    //Error mode throws an error whenever out of bounds are accessed
                    case (OutOfBoundsType.Error):
                    if ((i < 0) || (i >= myXSize))
                    {
                        throw new IndexOutOfRangeException();
                    }

                    if ((j < 0) || (j >= myYSize))
                    {
                        throw new IndexOutOfRangeException();
                    }
                    x = i;
                    y = j;
                    break;
                    
                    //Wrap mode simply wraps around, as if you were on a magic square.
                    case(OutOfBoundsType.Wrap):
                    x = WrapX(i);
                    y = WrapY(i, j);
                    break;

                    //Mirror should reflect the results, as if there were mirrors at the boundaries
                    case(OutOfBoundsType.Mirror):
                    throw new NotImplementedException();
                    break;

                    //If we're in zeros mode, we ignore any out of bounds sets.
                    case(OutOfBoundsType.Zeros):
                    if ((i < 0) || (i >= myXSize))
                    {
                        return;
                    }

                    if ((j < 0) || (j >= myYSize))
                    {
                        return;
                    }
                    x = i;
                    y = j;
                    break;

                    default:
                    throw new ArgumentException("Unhandled out of bounds mode!");
                        
                }
                if(!HeldValues.Contains(new Point(x,y)))
                {
                    if (MinValue != null && value < MinValue)
                        _map[x][y] = (int)MinValue;
                    else if (MaxValue != null && value > MaxValue)
                        _map[x][y] = (int)MaxValue;
                    else
                        _map[x][y] = value;
                    
                }
            
            }
            get 
            {
                int x, y;
                switch(this.OOBType)
                {
                    case(OutOfBoundsType.Error):

                    if ((i < 0) || (i >= myXSize))
                    {
                        throw new IndexOutOfRangeException();
                    }

                    if ((j < 0) || (j >= myYSize))
                    {
                        throw new IndexOutOfRangeException();
                    }
                    x = i;
                    y = j;
                    break;

                    case(OutOfBoundsType.Wrap):
                    x = WrapX(i);
                    y = WrapY(i, j);
                    break;

                    case(OutOfBoundsType.Mirror):
                    throw new NotImplementedException();
                    break;

                    case(OutOfBoundsType.Zeros):
                    if ((i < 0) || (i >= myXSize))
                    {
                        return 0;
                    }

                    if ((j < 0) || (j >= myYSize))
                    {
                        return 0;
                    }
                    x = i;
                    y = j;
                    break;
                    
                    default:
                    throw new ArgumentException("Invalid out of bounds mode!");
                    break;
                }
                return _map[x][y];
            }
        }

        public int this[Point pos]
        {
            set
            {
                this[pos.X, pos.Y] = value;
            }

            get
            {
                return this[pos.X, pos.Y];
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
