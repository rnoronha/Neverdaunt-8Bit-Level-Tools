using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N8Parser.Tronics
{
    [Serializable]
    public class Node
    {
        public NodeType Type
        {
            get
            {
                return _type;
            }
        }
        private NodeType _type;
        private N8Tronic _owner;

        public N8Tronic Owner
        {
            get
            {
                return _owner;
            }
        }

        public Node(NodeType t, N8Tronic owner)
        {
            _type = t;
            this._owner = owner;
        }

        public Node(N8Tronic owner) 
        {
            this._owner = owner;
        }

        /// <summary>
        /// Returns true if this node is any of DataInA, DataInB, DataOutA or DataOutB
        /// </summary>
        /// <returns></returns>
        public bool IsData()
        {
            return (this.Type >= NodeType.DataInA) && (this.Type <= NodeType.DataOutB);
        }

        /// <summary>
        /// Returns true if this node is a FlowOutA or FlowOutB node.
        /// </summary>
        /// <returns></returns>
        public bool IsFlowOut()
        {
            return this.Type == NodeType.FlowOutA || this.Type == NodeType.FlowOutB;
        }

        public static bool IsFlowOut(NodeType input)
        {
            return input == NodeType.FlowOutA || input == NodeType.FlowOutB;
        }


        /// <summary>
        /// Returns true if this node can only be wired once
        /// </summary>
        public bool CanOnlyBeWiredOnce()
        {
            return IsData() || IsFlowOut();
        }

        public bool IsCompatible(Node other)
        {
            //At the node-node level, all we can check is that DataIn or DataOut goes to DataBlock and that 
            //FlowIn goes to FlowOut

            //First we make sure that we're not related to the same owner. Incest is not allowed!
            //We can't trust IDs any more, so let's figure it out another way.
            if (this.Owner.Equals(other))
            {
                throw new Exception("Invalid wiring! From tronic " + this.Owner.ID + " to itself!");
            }

            //Then some business logic hooray!
            if (this.Type == NodeType.FlowIn)
            {
                if ((other.Type == NodeType.FlowOutA) || (other.Type == NodeType.FlowOutB))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if ((this.Type == NodeType.DataBlock))
            {
                //Kinda hacky, but it reads much better than a bunch of ors.
                if ((other.Type >= NodeType.DataInA) && (other.Type <= NodeType.DataOutB))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }


            //The following is a neat trick I just figured out - we don't have to duplicate that logic going the other way,
            //we just check to make sure that the other node is one of the checked types and return what it thinks.

            if ( (other.Type != NodeType.FlowIn) && (other.Type != NodeType.DataBlock) )
            {
                throw new Exception("Invalid wiring! From a flow in or data block node to a flow out or data block node!");
            }

            //And now we're guaranteed to return something - the other side must be either a FlowIn or a DataBlock.
            return other.IsCompatible(this);            
        }
    }

    [Serializable]
    public enum NodeType
    {
        FlowIn = 0,
        FlowOutA, //1
        FlowOutB, //2
        DataInA,  //3
        DataInB,  //4
        DataOutA, //5 
        DataOutB, //6
        DataBlock //7
    };

}
