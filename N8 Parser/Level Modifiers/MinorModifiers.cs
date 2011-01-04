using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using N8Parser.Geometry;

namespace N8Parser.Level_Modifiers
{
    public class MinorModifiers
    {
        /// <summary>
        /// Rotates an entire level. Currently doesn't do tronics.
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="RotationDegrees"></param>
        /// <param name="axis"></param>
        /// <returns></returns>

        public static N8Level RotateLevel(N8Level Input, double RotationDegrees, Vector3D axis)
        {
            var LevelBlocks = Input.blocks.Blocks;

            foreach (N8Block b in LevelBlocks)
            {
                Cylindrical c = new Cylindrical(b.position, axis);
                c.Theta += RotationDegrees * Utilities.DegToRad;
                b.position = c.ToCartesian();
            }

            return Input;
        }

        public static N8Level TranslateLevel(N8Level Input, Vector3D Offset)
        {
            //var NotLands = Utilities.GetNotLands(Input);

            foreach (N8Block b in Input.blocks.Blocks)
            {
                if(b.AttachedTo == null)
                    b.position += Offset;
            }

            foreach (N8Tronic t in Input.blocks.Tronics)
            {
                if (t.AttachedTo == null)
                    t.position += Offset;
            }

            return Input;
        }

        public static N8Level AttachLevel(N8Level Input, N8Block To, bool Absolute = true)
        {
            var NotLands = Utilities.GetNotLands(Input);

            foreach (N8Block b in NotLands)
            {
                if (Absolute)
                {
                    To.AttachToMeAbsolute(b);
                }
                else
                {
                    To.AttachToMe(b);
                }
            }
            return Input;
        }

        /// <summary>
        /// Changes the order in which blocks appear on the level, by changing their location in the list of blocks. Currently does not affect tronics.
        /// </summary>
        /// <param name="Input">The level to modify</param>
        /// <param name="Direction">The "direction" in which blocks should load; for instance, (0,0,1) would load blocks from the bottom of the level upwards, whereas (0,0,-1) would do the opposite. You can think of the value as being which component is more important; (1,2,0) would mean "when loading, the Y cooridnate is twice as important as X".</param>
        /// <returns>The modified level; note that the level is already modified in place</returns>
        public static N8Level OrderLoading(N8Level Input, Vector3D Direction)
        {
            Input.blocks.Blocks = Input.blocks.Blocks.OrderBy((b) => Utilities.DotProduct(b.position, Direction)).ToList();
            return Input;
        }
    }
}
   