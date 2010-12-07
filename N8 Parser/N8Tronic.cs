using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N8Parser.Tronics;
using System.Text.RegularExpressions;

namespace N8Parser
{
    [Serializable]
    public class N8Tronic : N8Block
    {
        public List<Wire> WiredTo = new List<Wire>();
        private List<Node> Nodes = new List<Node>();

        //note that while all tronics have a data segment in the save file, only display and data block tronics ever seem to actually use it.
        public string data = "";

        
        public N8Tronic(string[] parts)
            : base(parts)
        {
            data = parts[5];
            SetNodes();
        }

        public N8Tronic(int id, string type, string name) : base(id, type, name) 
        {
            SetNodes();
        }

        public N8Tronic(int id, string type, string name, string data) : base(id, type, name) 
        {
            SetNodes();
            this.data = data;
        }

        //copy constructor, doesn't copy wiring.
        public N8Tronic(N8Tronic source, int ID)
            : base(source, ID)
        {
            SetNodes();
            this.data = source.data;
        }

        public override string ToString()
        {
            string SanitizedData = Utilities.Sanitize(data);
            
            return this.ID + ":" + type + ":" + name + ":" + (float)position.X + "," + (float)position.Z + "," + (float)position.Y
                            + ":" + (float)rotation.W + "," + (float)rotation.X + "," + (float)rotation.Z + "," + (float)rotation.Y + ":" + SanitizedData + ":";
        }

        public void CheckWiringCompat(Wire w)
        {
            Node ThisNode = w.GetNode(this);


            //If this node can only be wired once, make sure a wire for it doesn't already exist
            if (ThisNode.CanOnlyBeWiredOnce())
            {
                foreach (Wire test in WiredTo)
                {
                    if (test.GetNode(this) == ThisNode)
                    {
                        throw new Exception("Invalid wiring attempted; tronic " + this + " has wire " + test + " and was trying to add wire " + w + " which re-wires node " + ThisNode);
                    }
                }
            }
        }

        public virtual void WireTo(Wire w)
        {
            //At the tronic level, we have to check one thing to make sure the wiring is valid:
            //If the wiring on my side involves one of my DataIn, DataOut or FlowOut nodes, I need to make sure that node isn't already wired

            CheckWiringCompat(w);

            this.WiredTo.Add(w);
            w.GetOtherSide(this).HalfWireTo(w);
        }


        public virtual void WireTo(Node myNode, Node otherNode)
        {
            //We just silently ignore requests to wire to null - this makes things a lot easier later on.
            if (otherNode == null)
            {
                return;
            }

            Wire temp = new Wire(myNode, otherNode);
            this.WireTo(temp);
        }

        //The other half of wire to, so we don't recursively loop. Should be overridden too.
        protected virtual void HalfWireTo(Wire NewWire)
        {
            CheckWiringCompat(NewWire);
            this.WiredTo.Add(NewWire);
        }

        //Default unspecified tronics have all nodes; this should be overridden to only set up the proper ones.
        protected virtual void SetNodes()
        {
            SetupNodes(new bool[] {true, true, true, true, true, true, true, true});
        }


        /// <summary>
        /// Gets the node of type NodeType from this tronic. By default, creates that node if it doesn't exist. Override this function.
        /// </summary>
        /// <param name="type">The type of node to get.</param>
        /// <returns>The index of the node in this tronic's node list</returns>
        public virtual Node GetNode(NodeType type)
        {
            return Nodes[(int)type];
        }

        public virtual void RemoveWire(N8Tronic other, NodeType myNode, NodeType otherNode)
        {
            Wire temp = new Wire(this.GetNode(myNode), other.GetNode(otherNode));
            this.WiredTo.Remove(temp);
            other.WiredTo.Remove(temp);
        }

        public virtual void RemoveWire(Wire w)
        {
            this.WiredTo.Remove(w);

            N8Tronic other = w.GetOtherSide(this);

            other.WiredTo.Remove(w);
        }

        public virtual void RemoveAllWires()
        {
            //First build a list of other tronics to which we're wired
            List<N8Tronic> connections = new List<N8Tronic>();

            foreach (Wire w in WiredTo)
            {
                connections.Add(w.GetOtherSide(this));
            }

            foreach (N8Tronic connectedTo in connections)
            {
                RemoveAllWires(connectedTo);
            }
        }

        public void RemoveAllWires(N8Tronic other)
        {
            WiredTo.RemoveAll((wire) => wire.Left.Owner == other || wire.Right.Owner == other);
            other.WiredTo.RemoveAll((wire) => wire.Left.Owner == this || wire.Right.Owner == this);
        }

        protected void SetupNodes(bool[] WantedNodes)
        {
            Node[] _raw_nodes = { new Node(NodeType.FlowIn, this), new Node(NodeType.FlowOutA, this), new Node(NodeType.FlowOutB, this), new Node(NodeType.DataInA, this), new Node(NodeType.DataInB, this), new Node(NodeType.DataOutA, this), new Node(NodeType.DataOutB, this), new Node(NodeType.DataBlock, this) };
            for(int i = 0; i < WantedNodes.Length; i++)
            {
                if (WantedNodes[i] == true)
                {
                    Nodes.Add(_raw_nodes[i]);
                }
                else
                {
                    Nodes.Add(null);
                }
            }
        }

        public bool HasNode(NodeType t)
        {
            return GetNode(t) != null;
        }        
    }
}
