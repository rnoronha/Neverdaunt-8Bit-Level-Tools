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
        /// Copies a block in input with ID blockID and puts it in NewPosition
        /// This will copy attachments and wirings too.
        /// Note that this does not work on tronics, though if a tronic is attached to a block it will be copied.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="BlockID"></param>
        /// <param name="NewPosition"></param>
        /// <returns></returns>
        public static N8Level CopyBlock(N8Level input, int BlockID, Vector3D NewPosition)
        {
            N8Block ToCopy = (from N8Block b in input.blocks.Blocks where b.ID == BlockID select b).First();

            N8Block Copy = input.blocks.GenerateBlock(ToCopy.type, ToCopy.name);
            Copy.position = NewPosition;

            foreach (N8Block b in ToCopy.Attachees)
            {
                if (b is N8Tronic)
                {

                }
            }

            return input;

        }

        /// <summary>
        /// Adds a megaland border to the level
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static N8Level AddBorder(N8Level input)
        {
            for (int i = -2; i < 2; i++)
            {
                N8Block land1 = input.blocks.GenerateBlock("landmega", "border " + i);
                N8Block land2 = input.blocks.GenerateBlock("landmega", "border " + i);
                N8Block land3 = input.blocks.GenerateBlock("landmega", "border " + i);
                N8Block land4 = input.blocks.GenerateBlock("landmega", "border " + i);

                land1.position.X = i * -800;
                land2.position.X = i * 800;

                land1.position.Y = 1600;
                land2.position.Y = -1600;

                land3.position.Y = i * 800;
                land4.position.Y = i * -800;

                land3.position.X = 1600;
                land4.position.X = -1600;

            }

            return input;
        }

        public static N8Level AddCrossroads(N8Level input)
        {
            N8Block land = input.blocks.GenerateBlock("street.highway.4way.cross", "Crossroads");
            for (int i = 0; i < 2; i++)
            {
                
                    N8Block landXp = input.blocks.GenerateBlock("street.highway.mega", "Crossings");
                    N8Block landYp = input.blocks.GenerateBlock("street.highway.mega", "Crossings");
                    N8Block landXm = input.blocks.GenerateBlock("street.highway.mega", "Crossings");
                    N8Block landYm = input.blocks.GenerateBlock("street.highway.mega", "Crossings");

                    landYp.rotation = new Quaternion(new Vector3D(0, 0, 1), 90);
                    landYm.rotation = new Quaternion(new Vector3D(0, 0, 1), 90);

                    landXp.position.X = i * 1000 + 935;
                    landXm.position.X = -(i * 1000 + 935);
                    landYp.position.Y = i * 1000 + 935;
                    landYm.position.Y = -(i * 1000 + 935);
            }

            return input;
        }

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
            axis.Normalize();

            foreach (N8Block b in LevelBlocks)
            {
                Cylindrical c = new Cylindrical(b.position, axis);
                c.Theta += RotationDegrees * Utilities.DegToRad;
                b.position = c.ToCartesian();
                b.rotation = b.rotation * new Quaternion(axis, RotationDegrees);
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

        public static N8Level OrderLoadingCylindrical(N8Level Input)
        {
            Input.blocks.Blocks = Input.blocks.Blocks.OrderBy((b) => new Cylindrical(b.position).Theta).ToList();
            return Input;
        }
    }
}
   