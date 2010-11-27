using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace N8Parser.Level_Modifiers
{
    static class GeometriesTest
    {
        public static void GenerateLevel()
        {
            string SavePath = @"C:\Program Files (x86)\N8\Saves\geometry.ncd";

            N8Level Level = new N8Level();

            /*
            Spherical direction = new Spherical(0, Math.PI/4, Math.PI);

            direction.R = 100;

            N8Block b = Level.blocks.GenerateBlock("pole", "Test");

            b.position = direction.ToCartesian();
            b.rotation = direction.GetNormalRotation();
            */

            //Quaternion rotation = new Quaternion(0,0,0,1);
            //*

            string[] colors = {"blue", "green", "magenta"};
            var points = Utilities.GenerateTetrahedron(new Vector3D(0, 0, 0), 3, 160);
            for (int i = 0; i < 5; i++)
            {
                N8Block ControlPoint = Level.blocks.GenerateBlock("snowmancoal", "Control Point " + i);
                ControlPoint.position.Z = 100;
                foreach (var p in points)
                {
                    N8Block b = Level.blocks.GenerateBlock("cartridge." + colors[i%colors.Length] + "light", "Line");
                    b.position = p.Item1;
                    b.rotation = p.Item2;
                    ControlPoint.AttachToMe(b);
                }
            }

            Console.Read();
            //*/


            Utilities.Save(SavePath, Level);

        }
    }
}
