using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media.Media3D;

namespace N8Parser.Level_Modifiers
{
    class RotateSave
    {
        public static void GenerateLevel(string InputPath, int degrees)
        {
            N8Level Level = GetLevel(InputPath, degrees);
            string SavePath = InputPath.Substring(0, InputPath.Length - 4) + "_rotated" + degrees + ".ncd";
            Utilities.Save(SavePath, Level);
        }

        public static N8Level GetLevel(string InputPath, int degrees)
        {
            N8Level Level = new N8Level(InputPath);
            foreach (N8Block b in Level.blocks.BlocksByID.Values)
            {
                //If b is attached to something, it'll get rotated when whatever it's attached to is rotated.
                if (b.AttachedTo != null)
                {
                    Utilities.Rotate(b, degrees);
                }
            }

            return Level;

        }
    }
}
