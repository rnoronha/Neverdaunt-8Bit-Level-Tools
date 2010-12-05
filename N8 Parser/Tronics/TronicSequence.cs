using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N8Parser.Tronics;
using System.Windows.Media.Media3D;

namespace N8Parser
{
    class TronicSequence
    {
        public N8BlockFactory tronics;
        private FlowTronic CurrentTronic;
        private Node CurrentOut;
        private Stack<Tuple<FlowTronic, Node>> Branches;
        private List<FlowTronic> sequence;
        public List<DataBlock> data;

        public FlowTronic GetFirst()
        {
            return sequence[0];
        }

        public Tuple<FlowTronic, Node> GetCurrent()
        {
            return Tuple.Create(CurrentTronic, CurrentOut);
        }


        public TronicSequence()
        {
            data = new List<DataBlock>();
            tronics = new N8BlockFactory();
            sequence = new List<FlowTronic>();
            Branches = new Stack<Tuple<FlowTronic, Node>>();
        }

        public TronicSequence(N8Tronic Initial, NodeType InitialOut = NodeType.FlowOutA)
        {
            data = new List<DataBlock>();
            tronics = new N8BlockFactory();
            CurrentTronic = (FlowTronic)Initial;
            CurrentOut = Initial.GetNode(InitialOut);
            sequence = new List<FlowTronic>();
            Branches = new Stack<Tuple<FlowTronic, Node>>();
        }

        public void FlowInFrom(FlowTronic n, NodeType Out = NodeType.FlowOutA)
        {
            this.GetFirst().FlowInFrom(n, Out);
        }

        public TronicSequence FlowOutTo(TronicSequence other, NodeType Out = NodeType.FlowOutA)
        {
            other.GetFirst().FlowInFrom(this.CurrentTronic, this.CurrentOut);
            return this;
        }

        public TronicSequence Append(TronicSequence other)
        {
            if (this.CurrentTronic != null)
            {
                other.GetFirst().FlowInFrom(this.CurrentTronic, CurrentOut);
            }
            Tuple<FlowTronic, Node> temp = other.GetCurrent();

            this.CurrentTronic = temp.Item1;
            this.CurrentOut = temp.Item2;

            this.sequence.AddRange(other.sequence);
            this.data.AddRange(other.data);

            this.tronics.CopyFromDestructive(other.tronics);
            return this;
        }

        public TronicSequence AppendCopy(TronicSequence other)
        {

            return this.Append(Utilities.DeepClone(other));
        }

        private void WireArithmeticNode(FlowTronic next, DataNodeIn DataInA, DataNodeIn DataInB, DataNodeOut DataOut, NodeType NextOut = NodeType.FlowOutA)
        {
            next.DataInA(DataInA);
            next.DataInB(DataInB);
            next.DataOutA(DataOut);

            Append(next, next.GetNode(NextOut));
        }

        public TronicSequence Plus(DataNodeIn DataInA, DataNodeIn DataInB, DataNodeOut DataOut, string name="Adder")
        {
            CheckCanAppend();
            FlowTronic NextTronic = tronics.Plus(name);

            WireArithmeticNode(NextTronic, DataInA, DataInB, DataOut);

            return this;
        }

        public TronicSequence Subtract(DataNodeIn DataInA, DataNodeIn DataInB, DataNodeOut DataOut, string name = "Subtractor")
        {
            CheckCanAppend();
            FlowTronic NextTronic = tronics.Subtract(name);

            WireArithmeticNode(NextTronic, DataInA, DataInB, DataOut);

            return this;
        }

        public TronicSequence And(DataNodeIn Left, DataNodeIn Right, DataNodeOut DataOut, string name = "And")
        {
            CheckCanAppend();
            FlowTronic NextTronic = tronics.And(name);

            WireArithmeticNode(NextTronic, Left, Right, DataOut);

            return this;
        }

        public TronicSequence Multiply(DataNodeIn DataInA, DataNodeIn DataInB, DataNodeOut DataOut, string name = "Multiplyer")
        {
            CheckCanAppend();
            FlowTronic NextTronic = tronics.Multiply(name);

            WireArithmeticNode(NextTronic, DataInA, DataInB, DataOut);

            return this;
        }

        public TronicSequence Divide(DataNodeIn DataInA, DataNodeIn DataInB, DataNodeOut DataOut, string name = "Divider")
        {
            CheckCanAppend();
            FlowTronic NextTronic = tronics.Divide(name);

            WireArithmeticNode(NextTronic, DataInA, DataInB, DataOut);


            return this;
        }

        public TronicSequence Distance(DataNodeIn DataInA, DataNodeIn DataInB, DataNodeOut DataOut, string name = "Distance")
        {
            CheckCanAppend();
            FlowTronic NextTronic = tronics.Distance(name);

            WireArithmeticNode(NextTronic, DataInA, DataInB, DataOut);


            return this;
        }

        public TronicSequence Rand(DataNodeIn DataInA, DataNodeIn DataInB, DataNodeOut DataOut, string name = "Random")
        {
            CheckCanAppend();
            FlowTronic NextTronic = tronics.Rand(name);

            WireArithmeticNode(NextTronic, DataInA, DataInB, DataOut);

            return this;
        }

        public TronicSequence IfEqual(DataNodeIn DataInA, DataNodeIn DataInB, string name = "IfEquals", TronicSequence Else = null)
        {
            CheckCanAppend();
            
            FlowTronic NextTronic = tronics.IfEqual(name);
            
            NextTronic.DataInA(DataInA);
            NextTronic.DataInB(DataInB);
            
            if (Else != null)
            {
                NextTronic.FlowOutTo(Else.GetFirst(), NodeType.FlowOutB);
                this.tronics.CopyFromDestructive(Else.tronics);
            }

            Append(NextTronic, NextTronic.GetNode(NodeType.FlowOutA));
            NoteBranch(NodeType.FlowOutB);

            return this;
        }

        private void NoteBranch(NodeType node)
        {
            Branches.Push(Tuple.Create(CurrentTronic, CurrentTronic.GetNode(node)));
        }

        public TronicSequence IfNotEqual(DataNodeIn DataInA, DataNodeIn DataInB, string name = "IfNotEquals", TronicSequence Else = null)
        {
            CheckCanAppend();

            FlowTronic NextTronic = tronics.IfEqual(name);

            NextTronic.DataInA(DataInA);
            NextTronic.DataInB(DataInB);

            if (Else != null)
            {
                NextTronic.FlowOutTo(Else.GetFirst(), NodeType.FlowOutA);
                this.tronics.CopyFromDestructive(Else.tronics);
            }

            Append(NextTronic, NextTronic.GetNode(NodeType.FlowOutB));
            NoteBranch(NodeType.FlowOutA);

            return this;
        }

        public TronicSequence IfGreater(DataNodeIn DataInA, DataNodeIn DataInB, string name = "IfGreater", TronicSequence Else = null)
        {
            CheckCanAppend();

            FlowTronic NextTronic = tronics.IfGreater(name);

            NextTronic.DataInA(DataInA);
            NextTronic.DataInB(DataInB);

            if (Else != null)
            {
                NextTronic.FlowOutTo(Else.GetFirst(), NodeType.FlowOutB);
                this.tronics.CopyFromDestructive(Else.tronics);
            }
            Append(NextTronic, NextTronic.GetNode(NodeType.FlowOutA));
            NoteBranch(NodeType.FlowOutB);
            return this;
        }

        /// <summary>
        /// Directs the flow to the next tronic if DataInA is less than or equal to DataInB
        /// </summary>
        /// <param name="DataInA"></param>
        /// <param name="DataInB"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public TronicSequence IfLTE(DataNodeIn DataInA, DataNodeIn DataInB, string name = "IfLTE", TronicSequence Else = null)
        {
            CheckCanAppend();

            FlowTronic NextTronic = tronics.IfGreater(name);

            NextTronic.DataInA(DataInA);
            NextTronic.DataInB(DataInB);
            if (Else != null)
            {
                NextTronic.FlowOutTo(Else.GetFirst(), NodeType.FlowOutA);
                this.tronics.CopyFromDestructive(Else.tronics);
            }

            Append(NextTronic, NextTronic.GetNode(NodeType.FlowOutB));
            NoteBranch(NodeType.FlowOutA);
            return this;
        }

        public TronicSequence Delay(DataNodeIn DataInA = null, string name = "Delay")
        {
            CheckCanAppend();

            FlowTronic NextTronic = tronics.Delay(name);

  
            NextTronic.DataInA(DataInA);

            Append(NextTronic, NextTronic.GetNode(NodeType.FlowOutA));

            return this;
        }

        public TronicSequence Display(DataNodeIn DataInA, string name = "Display")
        {
            CheckCanAppend();
            FlowTronic NextTronic = tronics.Display(name);

            NextTronic.DataInA(DataInA);

            Append(NextTronic, NextTronic.GetNode(NodeType.FlowOutA));
            return this;
        }

        public TronicSequence Mover(DataNodeIn DataInA, DataNodeOut DataOutA, string name = "Mover")
        {
            CheckCanAppend();

            FlowTronic NextTronic = tronics.Mover(name);

            NextTronic.DataInA(DataInA);
            NextTronic.DataOutA(DataOutA);

            Append(NextTronic, NextTronic.GetNode(NodeType.FlowOutA));

            return this;
        }

        public TronicSequence Rotor(DataNodeIn DataInA, DataNodeOut DataOutA, string name = "Rotor")
        {
            CheckCanAppend();

            FlowTronic NextTronic = tronics.Rotor(name);

            NextTronic.DataInA(DataInA);
            NextTronic.DataOutA(DataOutA);

            Append(NextTronic, NextTronic.GetNode(NodeType.FlowOutA));
            return this;
        }

        public TronicSequence RadioTransmit(DataNodeIn Channel, DataNodeIn Message, string name = "Transmitter")
        {
            CheckCanAppend();

            FlowTronic NextTronic = tronics.Transmitter(name);

            NextTronic.DataInA(Channel);
            NextTronic.DataInB(Message);

            Append(NextTronic, null);

            return this;
            
        }

        public DataBlock NewDataBlock(string name = "data", string data = "")
        {
            DataBlock ret = tronics.DataBlock(name);
            this.data.Add(ret);
            ret.data = data;
            return ret;
        }

        protected TronicSequence Button(int type = 1, string name = "Button")
        {
            Button b = tronics.Button(type, name);
            CurrentTronic = b;
            CurrentOut = b.GetNode(NodeType.FlowOutA);
            sequence.Add(b);

            return this;
        }

        protected TronicSequence Proxy(string name = "Button")
        {
            Proxy p = tronics.Proxy(name);
            CurrentTronic = p;
            CurrentOut = p.GetNode(NodeType.FlowOutA);
            sequence.Add(p);

            return this;
        }

        public TronicSequence RadioReciever(DataNodeIn Channel, DataNodeOut Username, DataNodeOut Message, string name = "Reciever")
        {
            Reciever r = tronics.Reciever(name);
            r.DataInA(Channel);
            r.DataOutB(Username);
            r.DataOutA(Message);
            Append(r, r.GetNode(NodeType.FlowOutA));

            return this;
        }

        protected TronicSequence Keyboard(DataNodeOut Out, string name = "Keyboard")
        {
            Keyboard k = tronics.Keyboard(name);
            k.DataOutA(Out);
            CurrentTronic = k;
            CurrentOut = k.GetNode(NodeType.FlowOutA);
            sequence.Add(k);

            return this;
        }

        public static TronicSequence StartFromKeyboard(DataNodeOut DataOut = null, string name = "Keyboard")
        {
            TronicSequence ts = new TronicSequence();

            ts.Keyboard(DataOut, name);

            return ts;
        }

        public static TronicSequence StartFromButton(int type = 1, string name = "Button")
        {
            TronicSequence ts = new TronicSequence();

            ts.Button(type, name);

            return ts;
        }

        public static TronicSequence StartFromProxy(string name = "Proxy")
        {
            TronicSequence ts = new TronicSequence();

            ts.Proxy(name);

            return ts;
        }

        public static TronicSequence StartFromReciever(DataNodeIn Channel, DataNodeOut Username, DataNodeOut Message, string name = "Reciever")
        {
            TronicSequence ts = new TronicSequence();
            ts.Button(1, "Reciever Setup")
              .RadioReciever(Channel, Username, Message, name);

            return ts;
        }

        

        private void Append(FlowTronic NextTronic, Node NextOut)
        {
            if (CurrentTronic != null)
            {

                NextTronic.FlowInFrom(CurrentTronic, CurrentOut);
            }
            CurrentTronic = NextTronic;
            CurrentOut = NextOut;
            sequence.Add(NextTronic);
        }

        public void CheckCanAppend()
        {
            if (CurrentTronic != null && CurrentOut == null)
            {
                throw new Exception("Cannot append to the current tronic sequence");
            }
            else if (CurrentTronic != null && (!(CurrentTronic.HasNode(NodeType.FlowOutA) || CurrentTronic.HasNode(NodeType.FlowOutB))))
            {
                throw new Exception("Trying to add to a tronic sequence that has already terminated on this tronic: " + CurrentTronic);
            }
        }

        public void CheckCanPrefix()
        {
            N8Tronic FirstTronic = sequence[0];
            if (!FirstTronic.HasNode(NodeType.FlowIn))
            {
                throw new Exception("Trying to append to this tronic sequence when there is nothing to append with! First tronic is: " + FirstTronic);
            }
        }

        public void LayoutLinear(Vector3D start)
        {
            int counter = 0;
            foreach (N8Tronic t in sequence)
            {
                counter++;
                t.position += start;
                t.position.X += counter * 20;
            }

            counter = 0;
            foreach (N8Tronic d in data)
            {
                counter++;
                d.position += start;
                d.position.X += counter * 20;
                d.position.Z += 20;
            }
        }

        public void LayoutDense(Vector3D Where, Quaternion rotation)
        {
            foreach (FlowTronic f in sequence)
            {
                if (!f.IsPositionDependent())
                {
                    f.position = Where;
                    f.rotation = rotation;
                }
            }

            foreach (DataBlock d in data)
            {
                d.position = Where;
                d.rotation = rotation;
            }
        }

        public void AttachAllTo(N8Block which, bool absolute)
        {
            foreach (N8Tronic t in tronics.TronicsByID.Values)
            {
                if (absolute)
                {
                    which.AttachToMeAbsolute(t);
                }
                else
                {
                    which.AttachToMe(t);
                }
            }
        }

        public void AttachAllNonPositional(N8Block which, bool absolute)
        {
            foreach (FlowTronic t in sequence)
            {
                if (!t.IsPositionDependent())
                {
                    if (absolute)
                    {
                        which.AttachToMeAbsolute(t);
                    }
                    else
                    {
                        which.AttachToMe(t);
                    }
                }
            }
            foreach (DataBlock db in data)
            {
                if (absolute)
                {
                    which.AttachToMeAbsolute(db);
                }
                else
                {
                    which.AttachToMe(db);
                }
            }
        }

        public void LayoutDense(Vector3D Where)
        {
            LayoutDense(Where, new Quaternion(0, 0, 0, 1));
        }

        public void LayoutDense()
        {
            LayoutDense(new Vector3D(0,0,0), new Quaternion(0, 0, 0, 1));
        }

        public void LayoutRandGrid(Vector3D origin, Quaternion rotation, int XSize, int YSize)
        {
            Random rand = new Random();

            int xmin = -XSize / 2;
            int xmax = XSize / 2;
            int ymin = -YSize / 2;
            int ymax = YSize / 2;

            foreach (N8Tronic t in tronics.TronicsByID.Values)
            {
                int yOffset = rand.Next(ymin, ymax);
                int xOffset = rand.Next(xmin, xmax);
                t.position.X = origin.X + xOffset;
                t.position.Y = origin.Y + yOffset;
                t.position.Z = origin.Z;
                t.rotation = rotation;
            }
        }

        internal void ElseInternal(TronicSequence ElseBranch)
        {
            var LastBranch = Branches.Pop();
            ElseBranch.GetFirst().FlowInFrom(LastBranch.Item1, LastBranch.Item2);
        }
    }
}
