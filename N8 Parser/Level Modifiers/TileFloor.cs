using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace N8Parser.Level_Modifiers
{
    class TileFloor
    {
        public static void GenerateLevel()
        {
            N8Level level = new N8Level();
            string BlockType = "land";
            string BlockName = "Land";

            int blocksize = 400;
            for (int i = -2000 + blocksize / 2; i <= 2000 - blocksize / 2; i += blocksize)
            {
                for (int j = -2000 + blocksize / 2; j <= 2000 - blocksize / 2; j += blocksize)
                {
                    N8Block block = level.blocks.GenerateBlock(BlockType, BlockName);
                    block.position = new Vector3D(i, j, 0);
                }
            }

            level.MergeWithDestructive(GenerateStack.GetLevel(1000, new Vector3D(0, 0, -1000), "floor", "Stack", 65));

            Utilities.Save(@"C:\Program Files (x86)\N8\Saves\default_lands.ncd", level);
        }
    }
}
