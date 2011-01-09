using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace N8Parser.Level_Modifiers
{
    public class Terrain
    {
        private static Heightmap Map;
        private static Random rand;
        private static double vstep;
        private static HashSet<Point> Holds;
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
        public static Heightmap GetHeightmap(int ReqSize, int variability, double roughness, Heightmap seed = null, HashSet<Point> holds = null)
        {
            int size = (1 << ReqSize) +1;
            
            List<List<int>> map = seed.Map ?? new List<List<int>>(size);

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
            rand = new Random();
            doHeightmap(step, variability, size/2, size/2);

            return Map;
        }

        private static void doHeightmap(int step, int variability, int i, int j)
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
                doHeightmap(newStep, newVar, i + newStep, j + newStep);
                doHeightmap(newStep, newVar, i - newStep, j + newStep);
                doHeightmap(newStep, newVar, i + newStep, j - newStep);
                doHeightmap(newStep, newVar, i - newStep, j - newStep);
            }
        }

        public static List<List<Heightmap>> ChopHeightmap(int SquareSize, Heightmap input)
        {
            List<List<Heightmap>> ret = new List<List<Heightmap>>();

            List<List<int>> rawmap = input.Map;
            IEnumerable<List<int>> ColumnRest = rawmap.Skip(SquareSize);
            IEnumerable<List<int>> Columns = rawmap.Take(SquareSize);
            do
            {
                IEnumerable<IEnumerable<int>> RowsRest = Columns.Select((c) => c.Skip(SquareSize));
                IEnumerable<IEnumerable<int>> Rows = Columns.Select((c) => c.Take(SquareSize));
                ret.Add(new List<Heightmap>());
                do
                {
                    ret.Last().Add(new Heightmap(Rows.Select((c) => c.ToList()).ToList()));
                    Rows = RowsRest.Select((c) => c.Take(SquareSize));
                    RowsRest = RowsRest.Select((c) => c.Skip(SquareSize));
                } while (Rows.Max((c) => c.Count()) > 0);

                Columns = ColumnRest.Take(SquareSize);
                ColumnRest = ColumnRest.Skip(SquareSize);

            } while (Columns.Count() > 0);

            return ret;
        }
    }
    public class Heightmap
    {
        private List<List<int>> _map;
        private HashSet<Point> HeldValues = new HashSet<Point>();
        public Heightmap()
        {
            _map = new List<List<int>>();
        }
         
        public Heightmap(List<List<int>> seed)
        {
            _map = seed;
        }

        public Heightmap(List<List<int>> seed, HashSet<Point> holds)
        {
            _map = seed;
            HeldValues = holds;
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
        }

        public int this[int i, int j]
        {
            
            set 
            {
                i = NormalizeI(i);
                j = NormalizeJ(i, j);
                
                if(!HeldValues.Contains(new Point(i,j)))
                {
                    _map[i][j] = value; 
                }
            }
            get 
            {
                i = NormalizeI(i);
                j = NormalizeJ(i, j);   
                return _map[i][j];
            }
        }

        public int NormalizeI(int i)
        {
            while (i < 0)
            {
                i += Map.Count;
            }

            i = i % Map.Count;

            return i;
        }

        public int NormalizeJ(int i, int j)
        {
            while (j < 0)
            {
                j += Map[i].Count;
            }

            j = j % Map[i].Count;

            return j;
        }

        public List<List<int>> Map
        {
            get { return _map; }
        }
    }
}
