using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace N8Parser.Level_Modifiers
{
    class MinorModifiers
    {
        public static N8Level TranslateLevel(N8Level Input, Vector3D Offset)
        {
            //var NotLands = Utilities.GetNotLands(Input);

            foreach (N8Block b in Input.blocks.BlocksByID)
            {
                if(b.AttachedTo == null)
                    b.position += Offset;
            }

            foreach (N8Tronic t in Input.blocks.TronicsByID)
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

        public static N8Level OrderLoadingY(N8Level Input, bool Ascending)
        {
            IEnumerable<N8Block> BlocksByY;
            if(Ascending)
                BlocksByY = Input.blocks.BlocksByID.OrderBy((b) => b.position.Y);
            else
                BlocksByY = Input.blocks.BlocksByID.OrderByDescending((b) => b.position.Y);

            int count = 1;
            foreach (N8Block b in BlocksByY)
            {
                Input.blocks.ChangeID(b, count);
                count++;
            }

            return Input;
        }

        public static N8Level OrderLoadingX(N8Level Input, bool Ascending)
        {
            IEnumerable<N8Block> BlocksByX;
            if (Ascending)
                BlocksByX = Input.blocks.BlocksByID.OrderBy((b) => b.position.X);
            else
                BlocksByX = Input.blocks.BlocksByID.OrderByDescending((b) => b.position.X);

            int count = 1;
            foreach (N8Block b in BlocksByX)
            {
                Input.blocks.ChangeID(b, count);
                count++;
            }

            return Input;
        }

        public static N8Level OrderLoadingZ(N8Level Input, bool Ascending)
        {
            IEnumerable<N8Block> BlocksByZ;
            if (Ascending)
                BlocksByZ = Input.blocks.BlocksByID.OrderBy((b) => b.position.Z);
            else
                BlocksByZ = Input.blocks.BlocksByID.OrderByDescending((b) => b.position.Z);

            int count = 1;
            foreach (N8Block b in BlocksByZ)
            {
                Input.blocks.ChangeID(b, count);
                count++;
            }

            return Input;
        }
    }
}
   