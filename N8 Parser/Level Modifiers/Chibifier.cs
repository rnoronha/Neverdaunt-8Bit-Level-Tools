using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace N8Parser.Level_Modifiers
{
    public static class Chibifier
    {

        public static void scale(N8Block b, int xOffset, int yOffset)
        {
            //Scaling ratio between chibi and normal blocks is 1/5th
            b.position /= 5;

            //Megalands are 800x800
            b.position.X += xOffset * 800;
            b.position.Y += yOffset * 800;

            //Magic number! This is 4/5ths of 70, we add it so that the ground plane of the chibi area is the same height
            //as the ground plane of the non-chibi area.
            b.position.Z += 56;
            b.Special = 2;
        }
        /// <summary>
        /// Chibifies a level. Currently does not handle tronics. 
        /// </summary>
        /// <param name="SavePath">The level save to load</param>
        /// <param name="xLocation">Which megaland to replace, in X. Valid values are (-2,2)</param>
        /// <param name="yLocation">Which megaland to replace, in Y. Valid values are (-2,2)</param>
        /// <returns></returns>
        public static N8Level GetChibifiedLevel(string SavePath, int xLocation, int yLocation)
        {
            N8Level Level = new N8Level(SavePath);
            foreach (N8Block b in Level.blocks.BlocksByID)
            {
                scale(b, xLocation, yLocation);
            }

            //In tronics, we also need to scale the vectors in data blocks
            Regex IsVector = new Regex(@"^v(-?\d+),(-?\d+),(-?\d+)$");
            foreach (N8Tronic t in Level.blocks.TronicsByID)
            {
                scale(t, xLocation, yLocation);
                //Dump all tronics down at the bottom of the level, because they don't scale physically
                t.position.Z = -500;
                if (t.type == "cdata")
                {
                    Match m = IsVector.Match(t.data);
                    if (m.Success)
                    {
                        string NewData = "v" + (double.Parse(m.Groups[1].Value) / 5) + "," + (double.Parse(m.Groups[2].Value) / 5) + "," + (double.Parse(m.Groups[3].Value) / 5);
                        t.data = NewData;
                    }
                }
            }

            //And then re-tile the land so it's not just some little chibi island floating in space.
            if (xLocation < -2 || xLocation > 2 || yLocation < -2 || yLocation > 2)
            {
                throw new Exception("xLocation or yLocation were not in valid range; must be between -2 and 2 inclusive");
            }

            for (int i = -2; i <= 2; i++)
            {
                for (int j = -2; j <= 2; j++)
                {
                    //Skip the land we just planted, of course.
                    if (i == xLocation && j == yLocation)
                        continue;

                    N8Block land = Level.blocks.GenerateBlock("landmega", "Land");
                    land.position.X = i * 800;
                    land.position.Y = j * 800;
                }
            }

            Level = MinorModifiers.OrderLoadingZ(Level, false);

            return Level;

        }

        public static void ChibifyLevel(string SavePath, string OutPath = null, int xLocation = 0, int yLocation = 0)
        {
            N8Level Level = GetChibifiedLevel(SavePath, xLocation, yLocation);
            if (OutPath == null)
            {
                OutPath = Path.GetDirectoryName(SavePath) +
                    Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(SavePath) + "_chibi.ncd";
            }
            Utilities.Save(OutPath, Level);
        }
    }
}
