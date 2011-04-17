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

            N8Level moon = new N8Level();
            List<Tuple<Vector3D, Quaternion>> sphere = Utilities.GenerateSphere(new Vector3D(0, 0, 1000), 1000, 200);

            foreach (var block in sphere)
            {
                N8Block b = moon.blocks.GenerateBlock("simple.white.land.mega", "moon");
                b.position = block.Item1;
                b.rotation = block.Item2;
            }

            Utilities.Save(Utilities.GetDefaultSaveFolder() + "moon.ncd", moon);
        }
        
    }
}
