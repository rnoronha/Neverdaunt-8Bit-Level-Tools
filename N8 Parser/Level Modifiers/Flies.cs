using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using N8Parser.Tronics;

namespace N8Parser.Level_Modifiers
{
    class Flies
    {
        public static void GenerateLevel()
        {
            string SavePath = @"C:\Program Files (x86)\N8\Saves\proxybubble.ncd";

            N8Level Level = new N8Level();

            N8BlockFactory LevelBlocks = Level.blocks;
            List<N8Tronic> ProxyBlocks = new List<N8Tronic>();


            //List<Tuple<Vector3D, Quaternion>> points = new List<Tuple<Vector3D, Quaternion>>();
            //points.AddRange(Utilities.EvenCircle(new Vector3D(0, 0, -1), 45, 45));
            /*
            points.AddRange(Utilities.EvenSphere(new Vector3D(0, 0, 50), 70, 150, (double)10 / 16 * Math.PI));
            points.AddRange(Utilities.EvenSphere(new Vector3D(0, 0, 50), 90, 250, (double)8 / 16 * Math.PI));
            points.AddRange(Utilities.EvenSphere(new Vector3D(0, 0, 50), 145, 350, (double)9 / 16 * Math.PI));
            points.AddRange(Utilities.EvenSphere(new Vector3D(0, 0, 50), 175, 450, (double)9 / 16 * Math.PI));
            */

            var b1 = Utilities.EvenSphere(new Vector3D(0, 0, 50), 70, 60);
            var b2 = Utilities.EvenSphere(new Vector3D(0, 0, 50), 45, 50);
            var b3 = Utilities.EvenSphere(new Vector3D(0, 0, 50), 80, 75);
            var b4 = Utilities.EvenSphere(new Vector3D(0, 0, 50), 30, 40);

            //points.AddRange(Utilities.EvenSphere(new Vector3D(0, 0, 50), 90, 250));
            // points.AddRange(Utilities.EvenSphere(new Vector3D(0, 0, 50), 145, 350));
            //points.AddRange(Utilities.EvenSphere(new Vector3D(0, 0, 50), 175, 450));

            Quaternion InitialRotation = new Quaternion(new Vector3D(0, 0, 1), -90) * new Quaternion(new Vector3D(0, 1, 0), 90);

            N8Block Bubble1 = LevelBlocks.GenerateBlock("snowmancoal", "Bubble 1 CP");
            Bubble1.position = new Vector3D(0, 200, 50);
            foreach (var t in b1)
            {
                Proxy p = LevelBlocks.Proxy("Bubble 1");
                p.position = t.Item1;
                p.rotation = t.Item2;
                Bubble1.AttachToMe(p);
            }

            N8Block Bubble2 = LevelBlocks.GenerateBlock("snowmancoal", "Bubble 2 CP");
            Bubble2.position = new Vector3D(0, -200, 50);
            foreach (var t in b2)
            {
                Proxy p = LevelBlocks.Proxy("Bubble 2");
                p.position = t.Item1;
                p.rotation = t.Item2;
                Bubble2.AttachToMe(p);
            }

            N8Block Bubble3 = LevelBlocks.GenerateBlock("snowmancoal", "Bubble 3 CP");
            Bubble3.position = new Vector3D(200, 0, 50);
            foreach (var t in b3)
            {
                Proxy p = LevelBlocks.Proxy("Bubble 3");
                p.position = t.Item1;
                p.rotation = t.Item2;
                Bubble3.AttachToMe(p);
            }

            N8Block Bubble4 = LevelBlocks.GenerateBlock("snowmancoal", "Bubble 4 CP");
            Bubble4.position = new Vector3D(-200, 0, 50);
            foreach (var t in b4)
            {
                Proxy p = LevelBlocks.Proxy("Bubble 4");
                p.position = t.Item1;
                p.rotation = t.Item2;
                Bubble4.AttachToMe(p);
            }


            Utilities.Save(SavePath, Level);
        }
    }
}
