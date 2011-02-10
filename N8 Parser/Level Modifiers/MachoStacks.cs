using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace N8Parser.Level_Modifiers
{
    public class MachoStacks
    {
        public static void GenerateLevels()
        {
            var stacks = GetLevels();
            for (int i = 0; i < stacks.Count; i++)
            {
                N8Level stack = stacks[i];
                Utilities.Save(Utilities.GetDefaultSaveFolder() + "machostack" + i + ".ncd", stack);
            }
        }
        public static List<N8Level> GetLevels()
        {
            List<N8Level> ret = new List<N8Level>();
            for (int i = 0; i < 4; i++)
            {
                N8Level stack = MaxProtectTest.GetMachoBubble();
                int k;
                for (k = -1000; k < 2000; k += 60)
                {
                    N8Block b = stack.blocks.GenerateBlock("megafloor", "stack");
                    b.position = new Vector3D(1995, 1995, k+i);
                }

                N8Block number = stack.blocks.GenerateBlock("letter." + i + ".dark.big", "ID");
                number.position = new Vector3D(1995, 1995, k+60);

                N8Block corner1 = stack.blocks.GenerateBlock("landmega", "Ring");
                corner1.position = new Vector3D(-1600, -1600, 0);
                corner1.rotation = new Quaternion(new Vector3D(0, 0, 1), 90);
                N8Block corner2 = stack.blocks.GenerateBlock("landmega", "Ring");
                corner2.position = new Vector3D(-800, -800, 0);
                corner2.rotation = new Quaternion(new Vector3D(0, 0, 1), 90);

                //Inner ring
                for (int l = 0; l <= 1600; l += 800)
                {
                    N8Block ringX = stack.blocks.GenerateBlock("landmega", "Ring");
                    ringX.position = new Vector3D(l, -800, 0);
                    ringX.rotation = new Quaternion(new Vector3D(0, 0, 1), 90);

                    N8Block ringY = stack.blocks.GenerateBlock("landmega", "Ring");
                    ringY.position = new Vector3D(-800, l, 0);
                    ringY.rotation = new Quaternion(new Vector3D(0, 0, 1), 90);
                }

                //outer ring
                for (int l = -800; l <= 1600; l += 800)
                {
                    N8Block ringX = stack.blocks.GenerateBlock("landmega", "Ring");
                    ringX.position = new Vector3D(l, -1600, 0);
                    ringX.rotation = new Quaternion(new Vector3D(0, 0, 1), 90);

                    N8Block ringY = stack.blocks.GenerateBlock("landmega", "Ring");
                    ringY.position = new Vector3D(-1600, l, 0);
                    ringY.rotation = new Quaternion(new Vector3D(0, 0, 1), 90);
                }

                stack = MinorModifiers.RotateLevel(stack, i * 90, new Vector3D(0, 0, 1));


                ret.Add(stack);
            }

            return ret;
        }
    }
}
