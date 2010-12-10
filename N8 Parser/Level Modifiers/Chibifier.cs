using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using N8Parser.Tronics;
using System.Windows.Media.Media3D;

namespace N8Parser.Level_Modifiers
{
    public static class Chibifier
    {
        public static Vector3D translate(Vector3D v, int xOffset, int yOffset)
        {
            //Megalands are 800x800
            v.X += xOffset * 800;
            v.Y += yOffset * 800;

            //Magic number! This is 4/5ths of 70, we add it so that the ground plane of the chibi area is the same height
            //as the ground plane of the non-chibi area.
            v.Z += 56;

            return v;
        }

        public static Vector3D scale(Vector3D v)
        {
            //Scaling ratio between chibi and normal blocks is 1/5th
            v /= 5;

            return v;
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
                //Everything gets scaled down
                b.position = scale(b.position);

                //But only blocks which are not attached to something need to be translated
                //(for the ones that are attached to something, the translation will be handled
                //by the block they're attached to)
                if (b.AttachedTo == null)
                {
                    b.position = translate(b.position, xLocation, yLocation);
                }
                b.Special = 2;
            }

            //In tronics, we also need to scale the vectors in data blocks
            
            foreach (N8Tronic t in Level.blocks.TronicsByID)
            {
                t.position = translate(scale(t.position), xLocation, yLocation);
                //Dump all tronics down at the bottom of the level, because they don't scale physically. Of course, we shouldn't do this with displays.
                if (t.type != "tdisplay")
                {
                    t.position.Z = -1000;
                }

                if (t.type == "troter")
                {
                    if(t.AttachedTo != null)
                    {
                        t.position = t.AttachedTo.position;
                    }
                }
                
                if (t.type == "cdata")
                {
                    if (t.IsVector())
                    {
                        Vector3D contents = t.DataToVector();
                        contents = translate(scale(contents), xLocation, yLocation);
                        t.data = contents.ToData();
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
