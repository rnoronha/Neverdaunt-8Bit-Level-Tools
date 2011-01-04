using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace N8Parser.Level_Modifiers
{
    static class ShiftSave
    {
        public static void GenerateLevel(string InputPath, Vector3D ShiftAmount)
        {
            N8Level Level = GetLevel(InputPath, ShiftAmount);
            string SavePath = InputPath.Substring(0, InputPath.Length - 4) + "_shifted.ncd";
            Utilities.Save(SavePath, Level);
        }

        public static N8Level GetLevel(string InputPath, Vector3D ShiftAmount)
        {
            N8Level Level = new N8Level(InputPath);
            foreach (N8Block b in Level.blocks.Blocks)
            {
                //If b is attached to something, it'll get rotated when whatever it's attached to is rotated.
                if (b.AttachedTo != null)
                {
                    b.position += ShiftAmount;
                }
            }

            return Level;

        }
    }

}
