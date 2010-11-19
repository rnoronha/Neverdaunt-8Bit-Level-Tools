using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N8Parser.Tronics
{
    /// <summary>
    /// Flow nodes in general are nodes that have either a flow in or a flow out node; this encompasses everything except data blocks.
    /// They also have data nodes, but I think what really separates them from data blocks is that they /do/ things, 
    /// whereas data blocks /store/ things.
    /// 
    /// Note that not all specific instances of a FlowNode will have all availale nodes, even though they're available as functions.
    /// This will cause exceptions to happen if you try to (say) wire an And block's FlowOutB to something.
    /// </summary>
    public abstract class FlowTronic : N8Tronic
    {
        public FlowTronic(int id, string type, string name) : base(id, type, name) { }
        
        /// <summary>
        /// Wires one of this tronic's FlowOut nodes to another tronic's FlowIn node
        /// </summary>
        /// <param name="other">Which tronic to wire to</param>
        /// <param name="which">Which FlowOut node to use, out of FlowOutA or FlowOutB. Most nodes just have FlowOutA</param>
        public virtual void FlowOutTo(FlowTronic other, NodeType which = NodeType.FlowOutA)
        {
            if (!(which == NodeType.FlowOutA || which == NodeType.FlowOutB))
            {
                throw new Exception("Expected a FlowOut node, got something else");
            }

            this.WireTo(this.GetNode(which), other.GetNode(NodeType.FlowIn));
        }

        /// <summary>
        /// Wires this tronic's FlowIn node to one of the other tronic's FlowOut nodes
        /// </summary>
        /// <param name="other">The other tronic that's being wired</param>
        /// <param name="which">Which FlowOut node to use</param>
        public virtual void FlowInFrom(FlowTronic other, NodeType which = NodeType.FlowOutA)
        {
            other.FlowOutTo(this, which);
        }

        public virtual void FlowInFrom(FlowTronic other, Node which)
        {
            other.FlowOutTo(this, which.Type);
        }

        /// <summary>
        /// Wires this tronic's DataInA node to a data block
        /// </summary>
        /// <param name="other">The DataNode to wire to, in In format</param>
        public virtual void DataInA(DataNodeIn other)
        {
            if (other == null)
                return;

            this.WireTo(this.GetNode(NodeType.DataInA), other.Parent.GetNode(NodeType.DataBlock));
        }

        /// <summary>
        /// Wires this tronic's DataInB node to a data block
        /// </summary>
        /// <param name="other">The DataNode to wire to, in In format</param>
        public virtual void DataInB(DataNodeIn other)
        {
            if (other == null)
                return;

            this.WireTo(this.GetNode(NodeType.DataInB), other.Parent.GetNode(NodeType.DataBlock));
        }

        /// <summary>
        /// Wires this tronic's DataOutA node to a data block
        /// </summary>
        /// <param name="other">The DataNode to wire to, in Out format</param>
        public virtual void DataOutA(DataNodeOut other)
        {
            if (other == null)
                return;
            this.WireTo(this.GetNode(NodeType.DataOutA), other.Parent.GetNode(NodeType.DataBlock));
        }

        /// <summary>
        /// Wires this tronic's DataOutA node to a data block
        /// </summary>
        /// <param name="other">The DataNode to wire to, in Out format</param>
        public virtual void DataOutB(DataNodeOut other)
        {
            this.WireTo(this.GetNode(NodeType.DataOutB), other.Parent.GetNode(NodeType.DataBlock));
        }
    }

    public class And : FlowTronic
    {

        public And(int id) : base(id, "cand", "And") { }
        public And(int id, string name) : base(id, "cand", name) { }


        protected override void SetNodes()
        {
            SetupNodes(new bool[]
                //FlowIn, FlowOutA, FlowOutB, DataInA, DataInB, DataOutA, DataOutB, DataBlock
            { true, true, false, true, true, true, false, false }
            );
        }
    }

    public class Add : FlowTronic
    {

        public Add(int id) : base(id, "cplus", "Adder") { }
        public Add(int id, string name) : base(id, "cplus", name) { }
        
         
        protected override void SetNodes()
        {
            SetupNodes(new bool[]
            //FlowIn, FlowOutA, FlowOutB, DataInA, DataInB, DataOutA, DataOutB, DataBlock
            { true,   true,     false,    true,    true,    true,     false,    false }
            );
        }
    }

    public class Subtract : FlowTronic
    {

        public Subtract(int id) : base(id, "cminus", "Subtracter") { }
        public Subtract(int id, string name) : base(id, "cminus", name) { }


        protected override void SetNodes()
        {
            SetupNodes(new bool[]
                //FlowIn, FlowOutA, FlowOutB, DataInA, DataInB, DataOutA, DataOutB, DataBlock
            { true, true, false, true, true, true, false, false }
            );
        }
    }

    public class Multiply : FlowTronic
    {

        public Multiply(int id) : base(id, "cmulti", "Multiplier") { }
        public Multiply(int id, string name) : base(id, "cmulti", name) { }


        protected override void SetNodes()
        {
            SetupNodes(new bool[]
                //FlowIn, FlowOutA, FlowOutB, DataInA, DataInB, DataOutA, DataOutB, DataBlock
            { true, true, false, true, true, true, false, false }
            );
        }
    }

    public class Divide : FlowTronic
    {

        public Divide(int id) : base(id, "cdiv", "Divider") { }
        public Divide(int id, string name) : base(id, "cdiv", name) { }


        protected override void SetNodes()
        {
            SetupNodes(new bool[]
                //FlowIn, FlowOutA, FlowOutB, DataInA, DataInB, DataOutA, DataOutB, DataBlock
            { true, true, false, true, true, true, false, false }
            );
        }
    }

    public class Distance : FlowTronic
    {

        public Distance(int id) : base(id, "cdistance", "Distance") { }
        public Distance(int id, string name) : base(id, "cdistance", name) { }


        protected override void SetNodes()
        {
            SetupNodes(new bool[]
                //FlowIn, FlowOutA, FlowOutB, DataInA, DataInB, DataOutA, DataOutB, DataBlock
            { true, true, false, true, true, true, false, false }
            );
        }
    }

    //"Random" conflicts with System.Random, so I use "rand"
    public class Rand : FlowTronic
    {

        public Rand(int id) : base(id, "crandom", "Rand") { }
        public Rand(int id, string name) : base(id, "crandom", name) { }
        public Node GetLow()
        {
            return this.GetNode(NodeType.DataInA);
        }

        public Node GetHigh()
        {
            return this.GetNode(NodeType.DataInB);
        }

        protected override void SetNodes()
        {
            SetupNodes(new bool[]
                //FlowIn, FlowOutA, FlowOutB, DataInA, DataInB, DataOutA, DataOutB, DataBlock
            { true, true, false, true, true, true, false, false }
            );
        }
    }

    public class Delay : FlowTronic
    {

        public Delay(int id) : base(id, "cdelay", "Delay") { }
        public Delay(int id, string name) : base(id, "cdelay", name) { }


        protected override void SetNodes()
        {
            SetupNodes(new bool[]
                //FlowIn, FlowOutA, FlowOutB, DataInA, DataInB, DataOutA, DataOutB, DataBlock
            { true, true, false, true, false, false, false, false }
            );
        }
    }

    public class IfEqual : FlowTronic
    {

        public IfEqual(int id) : base(id, "cifequal", "IfEqual") { }
        public IfEqual(int id, string name) : base(id, "cifequal", name) { }


        protected override void SetNodes()
        {
            SetupNodes(new bool[]
                //FlowIn, FlowOutA, FlowOutB, DataInA, DataInB, DataOutA, DataOutB, DataBlock
            {     true,   true,     true,     true,    true,    false,    false,    false }
            );
        }
    }

    public class IfGreater : FlowTronic
    {

        public IfGreater(int id) : base(id, "cifgreat", "IfGreater") { }
        public IfGreater(int id, string name) : base(id, "cifgreat", name) { }


        protected override void SetNodes()
        {
            SetupNodes(new bool[]
                //FlowIn, FlowOutA, FlowOutB, DataInA, DataInB, DataOutA, DataOutB, DataBlock
            { true, true, true, true, true, false, false, false }
            );
        }
    }

    public class Proxy : FlowTronic
    {

        public Proxy(int id) : base(id, "rproximity", "Proximity") { }
        public Proxy(int id, string name) : base(id, "rproximity", name) { }


        protected override void SetNodes()
        {
            SetupNodes(new bool[]
                //FlowIn, FlowOutA, FlowOutB, DataInA, DataInB, DataOutA, DataOutB, DataBlock
           { false, true, false, false, false, false, false, false }
            );
        }
    }

    public class Button : FlowTronic
    {
        public Button(int id) : base(id, "ryellowswitch", "Button") { }
        public Button(int id, string name) : base(id, "ryellowswitch", name) { }

        public Button(int id, int number) : base(id, NumberToType(number), "Button") { }
        public Button(int id, string name, int number) : base(id, NumberToType(number), name) { }

        private static string NumberToType(int number)
        {
            switch (number)
            {
                case 1:
                    return "ryellowswitch";
                case 2:
                    return "rblueswitch";
                case 3:
                    return "rgreenswitch";
                case 4:
                    return "rswitch";
                default:
                    throw new Exception("Invalid switch specified; saw: " + number + " but I can only accept 1 - 4");
            }
        }


        protected override void SetNodes()
        {
            SetupNodes(new bool[]
                //FlowIn, FlowOutA, FlowOutB, DataInA, DataInB, DataOutA, DataOutB, DataBlock
            { false, true, false, false, false, false, false, false }
            );
        }
    }

    public class Keyboard : FlowTronic
    {

        public Keyboard(int id) : base(id, "rkeyboard", "Keyboard") { }
        public Keyboard(int id, string name) : base(id, "rkeyboard", name) { }


        protected override void SetNodes()
        {
            SetupNodes(new bool[]
           //FlowIn, FlowOutA, FlowOutB, DataInA, DataInB, DataOutA, DataOutB, DataBlock
           { false,  true,     false,    false,   false,   true, false, false }
            );
        }
    }

    public class Display : FlowTronic
    {

        public Display(int id) : base(id, "tdisplay", "Display") { }
        public Display(int id, string name) : base(id, "tdisplay", name) { }


        protected override void SetNodes()
        {
            SetupNodes(new bool[]
                //FlowIn, FlowOutA, FlowOutB, DataInA, DataInB, DataOutA, DataOutB, DataBlock
            { true, true, false, true, false, false, false, false }
            );
        }
    }

    public class Mover : FlowTronic
    {

        public Mover(int id) : base(id, "tmover", "Mover") { }
        public Mover(int id, string name) : base(id, "tmover", name) { }


        protected override void SetNodes()
        {
            SetupNodes(new bool[]
                //FlowIn, FlowOutA, FlowOutB, DataInA, DataInB, DataOutA, DataOutB, DataBlock
            {     true,   true,    false,     true,    false,   true, false, false }
            );
        }
    }

    public class Rotor : FlowTronic
    {

        public Rotor(int id) : base(id, "troter", "Rotor") { }
        public Rotor(int id, string name) : base(id, "troter", name) { }


        protected override void SetNodes()
        {
            SetupNodes(new bool[]
                //FlowIn, FlowOutA, FlowOutB, DataInA, DataInB, DataOutA, DataOutB, DataBlock
            { true, true, false, true, false, true, false, false }
            );
        }
    }

    public class Reciever : FlowTronic
    {

        public Reciever(int id) : base(id, "rradioreciver", "Reciever") { }
        public Reciever(int id, string name) : base(id, "rradioreciver", name) { }

        public Node GetChannel()
        {
            return this.GetNode(NodeType.DataInA);
        }

        public Node GetMessage()
        {
            return this.GetNode(NodeType.DataOutA);
        }

        public Node GetSender()
        {
            return this.GetNode(NodeType.DataOutB);
        }




        protected override void SetNodes()
        {
            SetupNodes(new bool[]
                //FlowIn, FlowOutA, FlowOutB, DataInA, DataInB, DataOutA, DataOutB, DataBlock
            {     true,   true,     false,    true,    false,   true,     true,    false }
            );
        }
    }

    public class Transmitter : FlowTronic
    {

        public Transmitter(int id) : base(id, "tradiotransmiter", "Transmitter") { }
        public Transmitter(int id, string name) : base(id, "tradiotransmiter", name) { }

        public Node GetChannel()
        {
            return this.GetNode(NodeType.DataInA);
        }

        public Node GetMessage()
        {
            return this.GetNode(NodeType.DataInB);
        }


        protected override void SetNodes()
        {
            SetupNodes(new bool[]
                //FlowIn, FlowOutA, FlowOutB, DataInA, DataInB, DataOutA, DataOutB, DataBlock
            {     true,   false,    false,    true,    true,    false,     false,     false }
            );
        }
    }

    public static class TronicExtensions
    {
        public static bool IsPositionDependent(this FlowTronic Input)
        {
            switch (Input.type)
            {
                case ("troter"):
                    return true;
                case ("tmover"):
                    return true;
                case ("tdisplay"):
                    return true;
                case ("rkeyboard"):
                    return true;
                case ("rproximity"):
                    return true;

                case ("ryellowswitch"):
                    return true;
                case ("rblueswitch"):
                    return true;
                case ("rgreenswitch"):
                    return true;
                case ("rswitch"):
                    return true;
            }

            return false;
        }
    }
}
