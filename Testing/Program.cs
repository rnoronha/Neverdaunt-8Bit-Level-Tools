using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N8Parser.Level_Modifiers;
using N8Parser;
using System.Windows.Media.Media3D;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            //Chibifier.ChibifyLevel(@"C:\Program Files (x86)\N8\Saves\n8star.ncd");

            //AddMoon.GenerateLevel(@"C:\Program Files (x86)\N8\Saves\customs.ncd");
            //EliteCellGenerator.GenerateLevel();


            //DeIsland.GenerateLevel(@"C:\Program Files (x86)\N8\Saves\mosquito.ncd");
            //GeometriesTest.GenerateLevel();
            //MaxProtectTest.GenerateProxyBubble();
            //MaxProtectTest.GenerateProxyBimesh();
            //MaxProtectTest.GenerateLevel();
            //Flies.GenerateLevel();
            //Eyestrain.GenerateLevel();
            //Crazy.GenerateLevel();
            //SCIENCE.GenerateLevel();
            //RotateSave.GenerateLevel(@"C:\Program Files (x86)\N8\Saves\arrow+y.ncd");

            //TronicsTesting.GenerateLevel();

            //TileFloor.GenerateLevel();

            //AddMoon.GenerateLevel();

            /*
            N8Level Level = new N8Level(@"C:\Program Files (x86)\N8\Saves\arrow.ncd");
            MinorModifiers.RotateLevel(Level, 90, new Vector3D(1, 0, 0));
            Utilities.Save(@"C:\Program Files (x86)\N8\Saves\arrow_r.ncd", Level);
            Console.Read();
            */

            N8Level Level = new N8Level(@"C:\Program Files (x86)\N8\Saves\megaprotect_skeleton.ncd");
            var sphere = Utilities.EvenSphere(new Vector3D(0, 0, -1000), 525, 2000, Math.PI / 2);
            foreach (Tuple<Vector3D, Quaternion> loc in sphere)
            {
                N8Block b = Level.blocks.GenerateBlock("landmega", "Hill");
                b.position.X = Math.Round(loc.Item1.X);
                b.position.Y = Math.Round(loc.Item1.Y);
                b.position.Z = Math.Round(loc.Item1.Z);
                b.rotation = loc.Item2;
            }
            Level = MinorModifiers.OrderLoading(Level, new Vector3D(0, 0, 1));

            Utilities.Save(@"C:\Program Files (x86)\N8\Saves\for_duck.ncd", Level);

            Console.WriteLine("Number of blocks: " + Level.blocks.Blocks.Count);
            Console.Read();
        }
    }
}
