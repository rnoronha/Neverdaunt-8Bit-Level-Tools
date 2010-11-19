using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace N8Parser.Level_Modifiers
{
    static class AddMoon
    {
        public static void GenerateLevel()
        {
            N8Level BensGraveyard = new N8Level(@"C:\Program Files (x86)\N8\Saves\bens_graveyard.ncd");
            

            var sphere = Utilities.EvenSphere(new Vector3D(1000, 0, 1500), 250, 500);
            foreach (var t in sphere)
            {
                N8Block moonblock = BensGraveyard.blocks.GenerateBlock("landwinter", "Moon");
                moonblock.position = t.Item1;
                moonblock.rotation = t.Item2;
            }

            string SavePath = @"C:\Program Files (x86)\N8\Saves\moonyard.ncd";

            Utilities.Save(SavePath, BensGraveyard);
        }
    }
}
