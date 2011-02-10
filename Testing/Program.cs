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

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {

            Bitmap temp = Terrain.MinimizeMap(@"C:\Projects\N8 Parser\Map.png");
            temp = Terrain.ProcessMap(temp);
            temp = Terrain.TrimBounds(temp);
            temp = Terrain.ExpandMap(temp);

            HashSet<Point> holds = Terrain.BitmapToHolds(temp);

            Console.WriteLine("Calculating mixture...");
            Random rand = new Random(1);

            Heightmap blank = new Heightmap(temp.Width, temp.Height);
            
            SortedList<double, Action> actions = new SortedList<double, Action>();
            actions.Add(0.0001, () => Terrain.Normalize(-5000, 5000));
            actions.Add(0.009, () => Terrain.Fault());
            actions.Add(0.01, () => Terrain.Hill(1, 0.5, 25));
            actions.Add(0.75, () => Terrain.Hill(1, 0.5, 10));
            actions.Add(0.25, () => Terrain.Hill(1, 0.5, 10, true));
            Heightmap res = Terrain.Arbitrary(blank, actions, 500000, holds, rand);
            Console.WriteLine("Max is: " + res.max() + ", min is: " + res.min());
            res.ShowHeatmap(10, temp);

            SortedList<double, Action> FinalActions = new SortedList<double, Action>();
            FinalActions.Add(0.75, () => Terrain.Hill(1, 0.5, 10));
            FinalActions.Add(0.25, () => Terrain.Hill(1, 0.5, 10, true));
            res = Terrain.Arbitrary(blank, FinalActions, 5000, holds, rand);
            res.ShowHeatmap(10, temp);
            Console.ReadLine();
            /*
            Heightmap res = Terrain.DiamondSquare(8, 400, 0.2, null, null, rand);
            res.ShowBitmap();
            /*
            res.normalize(0, 1000);
            

            /*
            Heightmap seed = new Heightmap(17);
            seed[5, 5] = 300;
            for(int i = -1; i <= 1; i += 2)
                for (int j = -1; j <= 1; j += 2)
                {
                    seed[5 + i, 5 + j] = 200;
                }

            Heightmap res = Terrain.GetHeightmap(4, 100, 0.5, seed);

            N8Level Level = new N8Level();
            for (int i = 1; i <= 10; i++)
            {
                for (int j = 1; j <= 10; j++)
                {
                    N8Block land = Level.blocks.GenerateBlock("land", "Terrain");
                    land.position.X = (i - 5) * 400 - 200;
                    land.position.Y = (j - 5) * 400 - 200;
                    land.position.Z = res[i, j];
                }
            }

            int min = (int)Level.blocks.Blocks.Min((b) => b.position.Z);
            MinorModifiers.TranslateLevel(Level, new Vector3D(0, 0, -min));

            Utilities.Save(Utilities.GetDefaultSaveFolder() + "terrain.ncd", Level);
             */
        }
        
    }
}
