using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.IO;

namespace N8Parser.Level_Modifiers
{
    public class Translator
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
        /// Translates a level, including all tronics (and vectors in tronics) contained within it.
        /// </summary>
        /// <param name="SavePath">Path from which the level should be loaded</param>
        /// <param name="offset">The offset the level should be moved by</param>
        /// <returns></returns>
        public static N8Level GetTranslatedLevel(string SavePath, Vector3D offset)
        {
            N8Level Level = new N8Level(SavePath);
            foreach (N8Block b in Level.blocks.Blocks)
            {
                //Only blocks which are not attached to something need to be translated
                //(for the ones that are attached to something, the translation will be handled
                //by the block they're attached to)
                if (b.AttachedTo == null)
                {
                    b.position += offset;
                }
            }

            //In tronics, we also need to translate the vectors in data blocks
            foreach (N8Tronic t in Level.blocks.Tronics)
            {
                if (t.AttachedTo == null)
                {
                    t.position += offset;
                }

                if (t.type == "cdata")
                {
                    if (t.IsVector())
                    {
                        Vector3D contents = t.DataToVector();
                        contents += offset;
                        t.data = contents.ToData();
                    }
                }
            }
            //And then make sure everything is in bounds
            {
                foreach (N8Tronic t in Level.blocks.Tronics)
                {
                    EnforceBounds(t);
                }
                foreach (N8Block b in Level.blocks.Blocks)
                {
                    EnforceBounds(b);
                }
            }

            Level = MinorModifiers.OrderLoading(Level, new Vector3D(0, 0, -1));

            return Level;

        }

        private static void EnforceBounds(N8Block b)
        {
            if (b.position.Z < -1000)
            {
                b.position.Z = -1000;
            }
            if (b.position.Z > 2000)
            {
                b.position.Z = 2000;
            }

            if (b.position.X < -2000)
            {
                b.position.X = -2000;
            }
            if (b.position.X > 2000)
            {
                b.position.X = 2000;
            }


            if (b.position.Y < -2000)
            {
                b.position.Y = -2000;
            }
            if (b.position.Y > 2000)
            {
                b.position.Y = 2000;
            }
        }

        public static void TranslateLevel(string SavePath, Vector3D offset, string OutPath = null)
        {
            N8Level Level = GetTranslatedLevel(SavePath, offset);
            if (OutPath == null)
            {
                OutPath = Path.GetDirectoryName(SavePath) +
                    Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(SavePath) + "_moved.ncd";
            }
            Utilities.Save(OutPath, Level);
        }
    }
}
