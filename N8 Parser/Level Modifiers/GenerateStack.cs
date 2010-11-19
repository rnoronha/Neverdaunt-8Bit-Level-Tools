using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace N8Parser.Level_Modifiers
{
    class GenerateStack
    {
        public static N8Level GetLevel(int height, Vector3D bottom, string BlockType, string BlockName, int BlockHeight)
        {
            N8Level level = new N8Level();

            for (int i = 0; i < height; i += BlockHeight)
            {
                N8Block StackBlock = level.blocks.GenerateBlock(BlockType, BlockName);
                //All blocks start off at the origin
                StackBlock.position.Z = i;
                StackBlock.position += bottom;
            }

            return level;
        }
    }
}
