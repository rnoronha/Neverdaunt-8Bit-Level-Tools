using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N8Parser.Level_Modifiers;
using N8Parser;
using System.Windows.Media.Media3D;
using N8Parser.Geometry;
using N8Parser.Tronics;
using System.Drawing;
using N8Parser.Terrain;
namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {

            Terrain.rand = new Random(0);
            Terrain.randomVals = new Dictionary<Tuple<int, int, int, int>, double>(10000);
            List<double> noise = new List<double>(10000);
            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < 1000; j++)
                {
                    noise.Add(Terrain.PerlinNoise_2D(1, i, j));
                }
            }
            Console.WriteLine("Number of unique random values: " + Terrain.randomVals.Count);
            Console.WriteLine("Min is: " + noise.Min() + ", max is: " + noise.Max());
            Console.Read();
        }
        
    }
}
