using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace N8Parser.Level_Modifiers
{
    static class DeIsland
    {
        public static void GenerateLevel(string BasePath)
        {
            N8Level Base = new N8Level(BasePath);
            N8Level megadefault = new N8Level(@"C:\Program Files (x86)\N8\Saves\default.ncd");

            foreach (N8Block b in megadefault.blocks.BlocksByID)
            {
                b.position.Z -= 1;
            }

            Base.MergeWithDestructive(megadefault);

            string NewPath = Path.GetDirectoryName(BasePath) + "\\" + Path.GetFileNameWithoutExtension(BasePath) + "_defaulted.ncd";
            Console.WriteLine(NewPath);
            Console.Read();
            Utilities.Save(NewPath, Base);

        }

    }
}
