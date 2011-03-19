using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using N8Parser.Geometry;
using System.Windows.Media.Media3D;

namespace N8Parser.Terrain
{
    public static class Terrain
    {
        public static Heightmap Map;
        public static Random rand;
        private static double vstep;
        private static HashSet<Point> Holds;
        public static Dictionary<Tuple<int,int,int,int>, double> randomVals;

        /// <summary>
        /// Generates a random Point.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static Point NextPoint(this Random r)
        {
            int X = r.Next();
            int Y = r.Next();
            return new Point(Map.WrapX(X), Map.WrapY(X, Y));
        }

        #region terrain modifiers

        #region Perlin noise stuff
        //From here: http://freespace.virgin.net/hugo.elias/models/m_perlin.htm

        private static double Noise(int seed, int x, int y, int frequency)
        {
            //Memoize the randoms so we don't keep on creating new ones, it's slooow
            //Also I realize that this isn't precisely one random per seed value, but I think that's okay
            Tuple<int,int,int,int> key = Tuple.Create(seed, x, y, frequency);
            double ret;

            if (!randomVals.TryGetValue(key, out ret))
            {
                ret = rand.NextDouble();
                randomVals.Add(key, ret);
            }
            return ret;
        }

        private static double SmoothNoise(int seed, int x, int y, int freq)
        { 
            double corners = ( Noise(seed, x-1, y-1, freq)+Noise(seed, x+1, y-1, freq)+Noise(seed, x-1, y+1, freq)+Noise(seed, x+1, y+1, freq)) / 16;
            double sides   = ( Noise(seed, x-1, y, freq)  +Noise(seed, x+1, y, freq)  +Noise(seed, x, y-1, freq)  +Noise(seed, x, y+1, freq)) /  8;
            double center  =  Noise(seed, x, y, freq) / 4;
            return corners + sides + center;
        }

        //Cosine interpolation - he says it is slow, but then that page was written in 1998... and he also says to roll your own RNG
        
        /// <summary>
        /// Cosine interpolation between A and B
        /// </summary>
        /// <param name="a">The "left" point to interpolate from</param>
        /// <param name="b">The "right" point to interpolate from</param>
        /// <param name="x">Where the actual point we're interpolating is between A and B, expressed as a fraction. 0 = A, 1 = B</param>
        /// <returns></returns>
        private static double CosInterpolate(double a, double b, double x)
        {
	        double ft = x * Math.PI;
	        double f = (1 - Math.Cos(ft)) * .5;
	        return  a*(1-f) + b*f;
        }

        
        //I just realized that this function has an implicit "how far away from each other are p0, p1, p2 and p3" and that's set to 1. I think that's okay.
        /// <summary>
        /// One-dimensional interpolation between points p1 and p2, using additional information from p0 and p3.
        /// From: http://www.paulinternet.nl/?page=bicubic
        /// </summary>
        /// <param name="p">Double array of count 4. p[0] is to the "left" of p[1], the interpolation point is between p[1] and p[2], p[3] is to the "right" of p[2]</param>
        /// <param name="x">Where the point we're trying to interpolate is between p[1] and p[2], as a fraction. 0 = @p[1], 1 = @p[2]</param>
        /// <returns>The cubic interpolation between p[1] and p[2]</returns>
        private static double CubicInterpolate(double[] p, double x)
        {
            if (p.Length != 4)
            {
                throw new Exception("Cubic interpolation requires four entries exactly, recieved " + p.Length);
            }
            return p[1] + 0.5 * x * 
                                    (p[2] - p[0] + 
                                         x * (2.0 * p[0] - 5.0 * p[1] + 4.0 * p[2] - p[3] + 
                                             x * (3.0 * (p[1] - p[2]) + p[3] - p[0])));

        }

        /// <summary>
        /// Bicubic interpolation from a 4x4 grid onto a point somewhere in that grid
        /// From: http://www.paulinternet.nl/?page=bicubic
        /// </summary>
        /// <param name="p">A 4x4 grid of points</param>
        /// <param name="x">How far the point of interpolation is between p[1][1] and p[2][1]</param>
        /// <param name="y">How far the point of interpolation is between p[1][1] and p[1][2]</param>
        /// <returns></returns>
        public static double BicubicInterpolate(double[][] p, double x, double y)
        {
            if (p.Length != 4)
            {
                throw new Exception("Bicubic interpolation requires a 4x4 matrix, recieved this many in X: " + p.Length);
            }
            double[] yInterpolation = new double[4];
            yInterpolation[0] = CubicInterpolate(p[0], y);
            yInterpolation[1] = CubicInterpolate(p[1], y);
            yInterpolation[2] = CubicInterpolate(p[2], y);
            yInterpolation[3] = CubicInterpolate(p[3], y);
            return CubicInterpolate(yInterpolation, x);
        }



        private static double InterpolatedNoise(int seed, double x, double y, int freq)
        {
              int intX     = (int) x;
              double fracX = x - intX;

              int intY     = (int) y;
              double fracY = y - intY;

              double v1 = SmoothNoise(seed, intX,     intY, freq);
              double v2 = SmoothNoise(seed, intX + 1, intY, freq);
              double v3 = SmoothNoise(seed, intX, intY + 1, freq);
              double v4 = SmoothNoise(seed, intX + 1, intY + 1, freq);

              double i1 = CosInterpolate(v1, v2, fracX);
              double i2 = CosInterpolate(v3, v4, fracX);

              return CosInterpolate(i1 , i2 , fracY);
        }


        /// <summary>
        /// Calculates the Perlin noise for a given X,Y and seed. Result should be close to being on the range 0 and 1, but observed range is about 0.16 to 0.88 (probably due to the smoothing).
        /// </summary>
        /// <param name="seed">Seed value to use; every 2D perlin noise call for the same noise map should use the same seed</param>
        /// <param name="x">X value to calculate</param>
        /// <param name="y">Y value to calculate</param>
        /// <param name="persistence">A tuning parameter, not really sure what it means</param>
        /// <param name="NumberOfOctaves">Number of noise maps to add together</param>
        /// <returns></returns>
        public static double PerlinNoise_2D(int seed, int x, int y, double persistence = 0.25, int NumberOfOctaves = 5)
        {
              double total = 0;
              double p = persistence; //should be 0<p<1
              double amplitude = 1;
              int n = NumberOfOctaves; //Should be like four or something
              int frequency = 1;
              double scale = 0;
              for(int i = 0; i < n; i++)
              {
                  frequency *= 2;
                  amplitude *= p;
                  scale += amplitude;
                  total += InterpolatedNoise(seed, x * frequency, y * frequency, frequency) * amplitude;
              }
              
              return total * (1/scale);
        }

        #endregion

        public static Heightmap PerlinNoise(double ratio = 0.002)
        {
            return PerlinNoise(new Point(0,0), new Point(Map.sizeX, Map.sizeY), ratio);
        }

        public static Heightmap PerlinNoise(Point start, Point end, double ratio = 0.002)
        {
            double displacement = (Map.max() - Map.min()) * ratio;
            int min = (int)-displacement / 2;
            int max = (int) displacement / 2;
            return PerlinNoise(min, max, start, end);
        }

        public static Heightmap PerlinNoise(int min, int max)
        {
            return PerlinNoise(min, max, new Point(0, 0), new Point(Map.sizeX, Map.sizeY));
        }

        public static Heightmap PerlinNoise(int min, int max, Point start, Point end)
        {
            randomVals = new Dictionary<Tuple<int, int, int, int>, double>(Map.sizeX * Map.sizeY * 5);
            Point RealStart = new Point(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y));
            Point RealEnd = new Point(Math.Max(start.X, end.X), Math.Max(start.Y, end.Y));
            int mag = max - min;
            int seed = rand.Next();
            for (int i = RealStart.X; i < RealEnd.X; i++)
            {
                Console.WriteLine("x = " + i);
                for (int j = RealStart.Y; j < RealEnd.Y; j++)
                {
                    double perlin = PerlinNoise_2D(seed, i, j);

                    Map[i, j] += (int)((perlin * mag) - min);
                }
            }

            return Map;
        }

        public static Heightmap UniformNoise(int min, int max)
        {
            return UniformNoise(min, max, new Point(0, 0), new Point(Map.sizeX, Map.sizeY));
        }

        public static Heightmap UniformNoise(int min, int max, Point start, Point end)
        {
            Point RealStart = new Point(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y));
            Point RealEnd = new Point(Math.Max(start.X, end.X), Math.Max(start.Y, end.Y));

            for (int i = RealStart.X; i < RealEnd.X; i++)
            {
                for (int j = RealStart.Y; j < RealEnd.Y; j++)
                {
                    Map[i, j] += rand.Next(min, max);
                }
            }

            return Map;
        }

        //Code from: http://terraineer.sourceforge.net/plugins/fluid.n
        //Theory from: http://webcache.googleusercontent.com/search?q=cache:kVlmqAi7HQYJ:www.gamedev.net/reference/articles/article2001.asp+gamedev+fluid+simulation&cd=1&hl=en&ct=clnk&gl=us&client=firefox-a&source=www.google.com

        public static Heightmap FluidSimulation(int Iterations = 10, int sC = 100, int sD = 10, double sT = 0.033, int sM = 100)
        {
            if (Iterations < 2)
            {
                throw new Exception("Fluid simulation must be run for at least two iterations to be useful, I got " + Iterations);
            }
            //Takes samples from the previous two timesteps, so create two other heightmaps
            Heightmap.OutOfBoundsType oldType = Map.OOBType;
            Map.OOBType = Heightmap.OutOfBoundsType.Zeros;
            Heightmap oldMap = Utilities.DeepClone(Map);
            Heightmap newMap = Utilities.DeepClone(Map);
            int max = Map.max();
            int min = Map.min();
            for (int i = 0; i < Iterations; i++)
            {
                for (int x = 0; x < Map.sizeX; x++)
                {
                    for (int y = 0; y < Map.sizeY; y++)
                    {
                        //I don't even know what this means.
                        newMap[x, y] = (int)(
                                       ((4 - 8 * sC * sC * sT * sT / (sD * sD)) / (sM * sT + 2)) * Map[x, y]
                                     + ((sM * sT - 2) / (sM * sT + 2)) * oldMap[x, y]
                                     + ((2 * sC * sC * sT * sT / sD * sD) / (sM * sT + 2)) * (Map[x + 1, y] + Map[x - 1, y] + Map[x, y + 1] + Map[x, y - 1]));
                        //Do some bounds, this seems to explode
                        if (newMap[x, y] > 1000000)
                        {
                            newMap[x, y] = 1000000;
                        }
                        else if (newMap[x, y] < -1000000)
                        {
                            newMap[x, y] = -1000000;
                        }
                    }
                }
                //Scale it back down to the original max and min
                newMap.normalize(min, max);
                //And then advance the simulation one step
                for (int x = 0; x < Map.sizeX; x++)
                {
                    for (int y = 0; y < Map.sizeY; y++)
                    {
                        oldMap[x, y] = Map[x, y];
                        Map[x, y] = newMap[x, y];
                    }
                }
                
            }
            Map.OOBType = oldType;
            return Map;
        }

        public static IEnumerator<Point> RandomXY(Random r = null)
        {
            Random myRand = r ?? rand;
            while (true)
            {
                yield return myRand.NextPoint();
            }
        }

        public static IEnumerator<Point> RandomWalk(int InitThetaDegrees, int ThetaStepDegrees, int RStepMax, int RStepMin, Point init, Random r = null)
        {
            Random myRand = r ?? rand;

            Cylindrical C = new Cylindrical();
            C.Theta_Degrees = InitThetaDegrees;
            Point current = init;

            while (true)
            {
                yield return current;
                int RStep = myRand.Next(RStepMin, RStepMax);
                C.R = RStep;
                int ThetaStep = myRand.Next(-ThetaStepDegrees / 2, ThetaStepDegrees / 2);
                C.Theta_Degrees += ThetaStep;
                Vector3D cartesian = C.ToCartesian();
                current = new Point((int)cartesian.X + current.X, (int)cartesian.Y + current.Y);
            }

        }

        public static IEnumerator<Point> HillFall(Point init)
        {
            int PrevHeight = int.MaxValue;
            int CurrHeight = Map[init];
            Point PrevPoint = new Point(-1, -1);
            Point CurrPoint = init;

            while (PrevHeight >= CurrHeight && PrevPoint != CurrPoint)
            {
                yield return CurrPoint;
                PrevPoint = CurrPoint;
                PrevHeight = CurrHeight;

                Point NextPoint = CurrPoint;
                int NextHeight = CurrHeight;

                for (int i = CurrPoint.X - 1; i <= CurrPoint.X + 1; i++)
                {
                    for (int j = CurrPoint.Y - 1; j <= CurrPoint.Y + 1; j++)
                    {
                        //Don't look at diagonals
                        if (Math.Abs(CurrPoint.X - i) > 0 && Math.Abs(CurrPoint.Y - j) > 0)
                        {
                            continue;
                        }

                        if (Map[i, j] < CurrHeight)
                        {
                            NextHeight = Map[i, j];
                            NextPoint = new Point(i, j);
                        }
                    }
                }

                CurrHeight = NextHeight;
                CurrPoint = NextPoint;
            }
        }

        public static IEnumerator<Point> HillClimb(Point init)
        {
            int PrevHeight = int.MinValue;
            int CurrHeight = Map[init];
            Point PrevPoint = new Point(-1, -1);
            Point CurrPoint = init;

            while (PrevHeight <= CurrHeight && PrevPoint != CurrPoint)
            {
                yield return CurrPoint;
                PrevPoint = CurrPoint;
                PrevHeight = CurrHeight;

                Point NextPoint = CurrPoint;
                int NextHeight = CurrHeight;

                for (int i = CurrPoint.X - 1; i <= CurrPoint.X + 1; i++)
                {
                    for (int j = CurrPoint.Y - 1; j <= CurrPoint.Y + 1; j++)
                    {
                        if (Math.Abs(CurrPoint.X - i) > 0 && Math.Abs(CurrPoint.Y - j) > 0)
                        {
                            continue;
                        }
                        if (Map[i, j] > CurrHeight)
                        {
                            NextHeight = Map[i, j];
                            NextPoint = new Point(i, j);
                        }
                    }
                }

                CurrHeight = NextHeight;
                CurrPoint = NextPoint;
            }
        }

        public static Heightmap RiverFormation(double SinkRatio = 0.005, int MaxSize = 5)
        {
            int min = Map.min();
            int max = Map.max();
            double displacement = (max - min) * SinkRatio;
            Point init = rand.NextPoint();
            //Climb to the top of a local hill
            
            IEnumerator<Point> hill;
            if (SinkRatio > 0)
            {
                hill = HillClimb(init);
            }
            else
            {
                hill = HillFall(init);
            }
            
            while (hill.MoveNext())
            {
                init = hill.Current;
            }

            //And then fall down - have to do it this way, because if we modified things on the fly it would affect the hill climber.
            List<Point> RiverCourse = new List<Point>();
            IEnumerator<Point> downslope;
            if (SinkRatio > 0)
            {
                downslope = HillFall(init);
            }
            else
            {
                downslope = HillClimb(init);
            }
            while (downslope.MoveNext())
            {
                RiverCourse.Add(downslope.Current);
            }
            
            int RiverSize = rand.Next(1, MaxSize);

            foreach (Point p in RiverCourse)
            {
                for (int i = -RiverSize; i <= RiverSize; i++)
                {
                    for (int j = -RiverSize; j <= RiverSize; j++)
                    {
                        Point current = new Point(i, j);
                        double DistanceToP = Math.Sqrt((current.X) * (current.X) + (current.Y) * (current.Y));
                        if (DistanceToP == 0)
                        {
                            DistanceToP = RiverSize;
                        }

                        int sink = (int)((RiverSize / DistanceToP) * displacement);
                        if (Math.Abs(sink) > 100000)
                        {
                            //Console.WriteLine("Whoa nelly");
                        }
                        int newHeight = Map[current.X + p.X, current.Y + p.Y];
                        //Rivers can't sink beneath the current min
                        Map[current.X + p.X, current.Y + p.Y] = Math.Max(newHeight - sink, min);                     
                    }
                }
            }

            return Map;
        }

        public static Heightmap ValleyRaiser(int steepness = 1, int MaxRadius = 50, bool ellipse = false)
        {
            Point location = rand.NextPoint();
            IEnumerator<Point> steps = HillFall(location);

            //Find the local maximum
            while (steps.MoveNext())
            {
                location = steps.Current;
            }

            Hill(location, steepness, 0, MaxRadius, MaxRadius, ellipse);
            return Map;
        }
        
        public static Heightmap MountainSmasher(int steepness = 1, int MaxRadius = 50, bool ellipse = false)
        {
            Point location = rand.NextPoint();
            IEnumerator<Point> steps = HillClimb(location);

            //Find the local maximum
            while (steps.MoveNext())
            {
                location = steps.Current;
            }

            Hill(location, steepness, 1, MaxRadius, MaxRadius, ellipse);
            return Map;
        }

        public static Heightmap Mountains(int NumSteps, double Steepness = 1, int MinRadius = 10, int MaxRadius = 25, bool ellipse = false)
        {
            IEnumerator<Point> points = RandomWalk(rand.Next(360), 45, 50, 25, rand.NextPoint());
            return Hills(NumSteps, Steepness, 0, MinRadius, MaxRadius, points, ellipse);
        }

        public static Heightmap Valleys(int NumSteps, double Steepness = 1, int MinRadius = 10, int MaxRadius = 25, bool ellipse = false)
        {
            IEnumerator<Point> points = RandomWalk(rand.Next(360), 45, 50, 25, rand.NextPoint());
            return Hills(NumSteps, Steepness, 1, MinRadius, MaxRadius, points, ellipse);
        }

        //from here: http://terraineer.sourceforge.net/plugins/hills.php
        public static Heightmap Hills(int numHills, double Steepness = 1, double ValleyProb = 0, int MinRadius = 10, int MaxRadius = 25, IEnumerator<Point> points = null, bool ellipse = false)
        {
            points = points ?? RandomXY();
            for (int c = 0; c < numHills; c++)
            {
                points.MoveNext();
                Hill(points.Current, Steepness, ValleyProb, MinRadius, MaxRadius, ellipse);
            }

            return Map;
        }

        //From here: http://terraineer.sourceforge.net/plugins/fault.php
        public static Heightmap Faults(int numFaults, double DisplacementRatio = 0.01, Random r = null)
        {
            Random MyRand = r ?? rand;

            for (int f = 0; f < numFaults; f++)
            {
                Fault(DisplacementRatio, MyRand);
            }

            return Map;
        }

        //Executes various different heightmap forming functions at random
        public static Heightmap Arbitrary(SortedList<double, Action> functions, int iterations = 10000, Func<int, bool> StepCallback = null)
        {
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

        //For each block, takes the sum of the heights of the surrounding blocks (and the block's own height), averages it, and sets it to that block's height
        public static void Average()
        {
            Heightmap NewMap = new Heightmap(Map.sizeX, Map.sizeY);

            for (int i = 0; i < Map.sizeX; i++)
            {
                for (int j = 0; j < Map.sizeY; j++)
                {
                    NewMap[i, j] = AverageAround(new Point(i, j), 1);
                }
            }

            Map = NewMap;
        }

        //Take the average around a point p in the heightmap, using points up to sizeXsize pixels away.
        public static int AverageAround(Point p, int size)
        {
            int NumPoints = 0;
            int total = 0;
            for (int i = p.X - size; i <= p.X + size; i++)
            {
                for (int j = p.Y - size; j <= p.Y - size; j++)
                {
                    NumPoints++;
                    total += Map[i, j];
                }
            }

            total = total / NumPoints;

            return total;
        }


        public static void Fault(double DisplacementRatio = 0.01, Random r = null)
        {
            int sizeX = Map.sizeX;
            int sizeY = Map.sizeY;
            Random MyRandom = r ?? rand;

            int x1 = Map.WrapX(MyRandom.Next());
            int x2 = Map.WrapX(MyRandom.Next());
            int y1 = Map.WrapY(MyRandom.Next(), MyRandom.Next());
            int y2 = Map.WrapY(MyRandom.Next(), MyRandom.Next());

            int Displacement = (int)((Map.max() - Map.min()) * DisplacementRatio);

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
            Map = Map.normalize(min, max);
        }

        public static void Hill(Point location, double SteepnessRatio = 0.0008, double ValleyProb = 0, int MinRadius = 10, int MaxRadius = 25, bool ellipse = false)
        {
            int Steepness = Math.Max(1, (int)((Map.max() - Map.min()) * SteepnessRatio));
            Steepness = Math.Min(10, Steepness);
            Hill(location, Steepness, ValleyProb, MinRadius, MaxRadius, ellipse);
        }
        
        public static void Hill(Point location, int Steepness = 1, double ValleyProb = 0, int MinRadius = 10, int MaxRadius = 25, bool ellipse = false)
        {
            int sizeX = Map.sizeX;
            int sizeY = Map.sizeY;

            bool IsValley = rand.NextDouble() < ValleyProb;
            int valleyMul = 1;
            if (IsValley)
            {
                valleyMul *= -1;
            }

            //Calculate both random values always to make this more stable between true and false
            int radiusX = rand.Next(MinRadius, MaxRadius);
            int radiusY = rand.Next(MinRadius, MaxRadius);
            if (!ellipse)
            {
                radiusY = radiusX;
            }

            int x = location.X;
            int y = location.Y;


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
            if (seed == null)
            {
                //A default seeding - mountains in the "middle" (actually corners, but whatever).
                map[0][0] = 1000;
                map[0][size - 1] = 1000;
                map[size - 1][0] = 1000;
                map[size - 1][size - 1] = 1000;
            }

            int step = size / 2;
            Map = new Heightmap(map);
            Holds = holds ?? new HashSet<Point>();
            vstep = roughness;
            rand = Random ?? new Random();
            DiamondSquare_Recursive(step, variability, size / 2, size / 2);

            return Map;
        }

        private static void DiamondSquare_Recursive(int step, int variability, int i, int j)
        {
            //Do the square averageing

            int SquareAvg = 0;
            for (int k = -1; k <= 1; k += 2)
            {
                for (int l = -1; l <= 1; l += 2)
                {
                    SquareAvg += Map[i + k * step, j + l * step];
                }
            }
            // Add the square averaging so we maintian whatever value was seeded here before.
            Map[i, j] += SquareAvg / 4 + rand.Next(-variability, variability);

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


                Map[i, j + k * step] = DiamondAvgI / 4 + rand.Next(-variability, variability);
                Map[i + k * step, j] = DiamondAvgJ / 4 + rand.Next(-variability, variability);
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

        #endregion

        #region bitmap modifiers
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
        #endregion

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
}
