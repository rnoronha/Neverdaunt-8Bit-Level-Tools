using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N8Parser.Tronics;

namespace N8Parser
{
    [Serializable]
    public class Wire : IComparable<Wire>
    {
        //Order doesn't actually matter to wires, so I call them left and right for convenience.
        //Left is always the node index that is either a DataBlock node or a FlowIn node
        //Right is any of the DataOuts, DataIns or FlowOuts.
        //Note that N8 itself doesn't seem to care.
        public Node Left, Right;

        public Wire(Node A, Node B)
        {
            if (!A.IsCompatible(B))
            {
                throw new Exception("Incompatible wiring attempted");
            }

            if (A.Type == NodeType.DataBlock || A.Type == NodeType.FlowIn)
            {
                Left = A;
                Right = B;
            }
            else if (B.Type == NodeType.DataBlock || B.Type == NodeType.FlowIn)
            {
                Left = B;
                Right = A;
            }
            else
            {
                throw new Exception("Invalid wiring attempted, but you shouldn't be seeing this.");
            }
        }

        /// <summary>
        /// Gets whatever node is on the other side of this wire.
        /// </summary>
        /// <param name="currentSide">The node on the known side of this wire</param>
        /// <returns>The node on the unknown side of this wire</returns>
        public N8Tronic GetOtherSide(N8Tronic CurrentSide)
        {
            N8Tronic OtherSide;
            if (Left.Owner != CurrentSide && Right.Owner == CurrentSide)
            {
                OtherSide = Left.Owner;
            }
            else if (Right.Owner != CurrentSide && Left.Owner == CurrentSide)
            {
                OtherSide = Right.Owner;
            }
            else
            {
                throw new Exception("Invalid request; wire " + this + " was asked to find other side, but the current side that was given is invalid; tronic that I was looking for is: " + CurrentSide);
            }

            return OtherSide;
        }

        public Node GetNode(N8Tronic side)
        {
            Node ret;

            if (Left.Owner != side && Right.Owner == side)
            {
                ret = Right;
            }
            else if (Right.Owner != side && Left.Owner == side)
            {
                ret = Left;
            }
            else
            {
                throw new Exception("Invalid request; wire " + this + " was asked to find a node, but the provided side is invalid; tronic that I was looking for is: " + side);
            }


            return ret;
        }

        public int CompareTo(Wire other)
        {
            bool LeftTronicEqual = (this.Left.Owner.ID == other.Left.Owner.ID);
            bool RightTronicEqual = (this.Right.Owner.ID == other.Right.Owner.ID);

            bool LeftNodeEqual = (this.Left.Type == other.Left.Type);
            bool RightNodeEqual = (this.Right.Type == other.Right.Type);

            //Ensure that differences between tronic IDs are always larger than differences between node numbers
            //Since there's at most like six nodes on a tronic, 20 should be more than enough.
            //Of course, this is a gigantic horrible hack but still.
            int IDDiffMultiplier = 20;

            //Usual case - left tronics not equal. Just return the difference in their IDs
            if (!LeftTronicEqual)
            {
                return (this.Left.Owner.ID - other.Left.Owner.ID) * IDDiffMultiplier;
            }
            //Left tronics are equal, but right tronics aren't. Return the difference in right IDs.
            else if (LeftTronicEqual && !RightTronicEqual)
            {
                return (this.Right.Owner.ID - other.Right.Owner.ID) * IDDiffMultiplier;
            }
            //Left and right tronics are equal for both wires 
            //(e.g, a mover hooked up to a data block on both DataIn and DataOut)
            //Return the negative difference of whichever nodes are different.
            else if (LeftTronicEqual && RightTronicEqual && !LeftNodeEqual)
            {
                return this.Left.Type - other.Left.Type;
            }
            else if (LeftTronicEqual && RightTronicEqual && LeftNodeEqual && !RightNodeEqual)
            {
                return this.Right.Type - other.Right.Type;
            }
            //If neither of the nodes are different, then these two wires represent the same thing.
            else if (LeftTronicEqual && RightTronicEqual && LeftNodeEqual && RightNodeEqual)
            {
                return 0;
            }
            else
            {
                throw new Exception("wtf is going on with these wires?\nthis = " + this + "\nother = " + other);
            }
        }

        public override string ToString()
        {
            return Left.Owner.ID + "," + (int)Left.Type + "," + Right.Owner.ID + "," + (int)Right.Type;
        }
    }
}
