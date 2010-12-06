using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using N8Parser.Tronics;


namespace N8Parser
{
    [Serializable]
    public class N8BlockFactory
    {
        public SortedDictionary<int, N8Block> BlocksByID;
        public SortedDictionary<int, N8Tronic> TronicsByID;
        private int MaxID = 1;
        public bool GeneratedShrine = false;
        public int BlockCount = 0;
        public static int MaxBlockCount = 349;

        public N8BlockFactory() 
        {
            BlocksByID = new SortedDictionary<int, N8Block>();
            TronicsByID = new SortedDictionary<int, N8Tronic>();
        }

        public N8Block AddBlockFromSave(string raw)
        {
            IncrementBlockCount();
            string[] parts = raw.Split(':');
            N8Block current = new N8Block(parts);

            BlocksByID.Add(current.ID, current);
            MaxID = Math.Max(current.ID, MaxID);
            return current;
        }

        public N8Tronic AddTronicFromSave(string raw)
        {
            IncrementBlockCount();
            string[] parts = raw.Split(':');
            N8Tronic current = new N8Tronic(parts);

            TronicsByID.Add(current.ID, current);
            MaxID = Math.Max(current.ID, MaxID);
            return current;
        }

        public N8Block GenerateShrine(int shrineID)
        {
            if(!GeneratedShrine)
            {
                GeneratedShrine = true;
                N8Block temp = new N8Block(shrineID);
                BlocksByID.Add(shrineID, temp);
                return temp;
            }
            else
            {
                throw new Exception("I've already generated the shrine and you're asking me to generate it again!");
            }

        }

        public N8Block CloneBlock(N8Block b)
        {
            IncrementBlockCount();
            int NewID = GetNewID();
            N8Block temp = new N8Block(b, NewID);
            BlocksByID.Add(NewID, temp);
            return temp;
        }

        public N8Tronic CloneTronic(N8Tronic b)
        {
            IncrementBlockCount();
            int NewID = GetNewID();
            N8Tronic temp = new N8Tronic(b, NewID);
            TronicsByID.Add(NewID, temp);
            return temp;
        }

        public N8Block GenerateBlock(string type, string name)
        {
            IncrementBlockCount();
            int NewID = GetNewID();
            N8Block temp = new N8Block(NewID, type, name);
            BlocksByID.Add(NewID, temp);
            return temp;
        }

        public N8Tronic GenerateTronic(string type, string name)
        {
            IncrementBlockCount();
            int NewID = GetNewID();
            N8Tronic temp = new N8Tronic(NewID, type, name);
            TronicsByID.Add(NewID, temp);
            return temp;
        }

        private void IncrementBlockCount()
        {
            BlockCount++;
        }

        public bool GetBlock(int id, out N8Block result)
        {
            bool inTronics = false;
            bool inBlocks = BlocksByID.TryGetValue(id, out result);
            if (!inBlocks)
            {
                N8Tronic temp;
                inTronics = TronicsByID.TryGetValue(id, out temp);
                result = temp;
            }

            if (inBlocks && inTronics)
            {
                throw new Exception("what the heck, block id " + id + " is used by both a block and a tronic!");
            }

            return inBlocks || inTronics;
        }

        private int GetNewID()
        {
            MaxID += 1;
            return MaxID;
        }

        public bool IDInUse(int id)
        {
            return this.BlocksByID.ContainsKey(id) || this.TronicsByID.ContainsKey(id);
        }

        public static N8BlockFactory operator + (N8BlockFactory me, Vector3D offset)
        {
            foreach(N8Block b in me.BlocksByID.Values)
            {
                b.position += offset;
            }
            foreach(N8Tronic t in me.TronicsByID.Values)
            {
                t.position += offset;
            }
            return me;
        }

        internal void CopyFromNonDestructive(N8BlockFactory copyee)
        {
            N8BlockFactory copy = Utilities.DeepClone(copyee);
            CopyFromDestructive(copy);
        }

        public bool ChangeID(N8Block block, int newID = -1)
        {
            if(newID == -1)
                newID = GetNewID();

            if (IDInUse(newID))
                return false;
            else
            {
                if (this.BlocksByID.ContainsKey(block.ID))
                {
                    this.BlocksByID.Remove(block.ID);
                }
                else if (this.TronicsByID.ContainsKey(block.ID))
                {
                    this.TronicsByID.Remove(block.ID);
                }

                block.ChangeID(newID);

                if (block is N8Tronic)
                {
                    this.TronicsByID.Add(newID, (N8Tronic)block);
                }
                else
                {
                    this.BlocksByID.Add(newID, block);
                }

                return true;
            }
        }
        
        internal void CopyFromDestructive(N8BlockFactory copy)
        {
            //We're merging the other guy with us, so when it comes down to it it's his shit that needs to change
            foreach (N8Block b in copy.BlocksByID.Values)
            {
                ChangeID(b);
            }

            //If a block and a tronic conflict, we'll change the block.
            foreach (N8Tronic t in copy.TronicsByID.Values)
            {
                if (IDInUse(t.ID))
                {
                    //Is it being used by a block?
                    if (this.BlocksByID.ContainsKey(t.ID))
                    {
                        N8Block conflictor;
                        //Then have him change, the lazy bastard
                        this.BlocksByID.TryGetValue(t.ID, out conflictor);

                        ChangeID(conflictor);
                    }
                    else
                    {
                        ChangeID(t);
                    }
                }
            }


            if (TronicsByID.Count > 0 && BlocksByID.Count > 0)
            {
                MaxID = Math.Max(TronicsByID.Keys.Max(), BlocksByID.Keys.Max());
            }
            else if (TronicsByID.Count > 0)
            {
                MaxID = TronicsByID.Keys.Max();
            }
            else if (BlocksByID.Keys.Count > 0)
            {
                MaxID = BlocksByID.Keys.Max();
            }
            else
            {
                MaxID = 1;
            }


            //And then make sure this isn't used again
            copy.BlocksByID = null;
            copy.TronicsByID = null;
        }

        //And then type-specific tronic generators, so we have type-safe tronics! Hooray!
        public Add Plus(string name = "Adder")
        {
            int NewID = GetNewID();
            Add temp = new Add(NewID, name);
            TronicsByID.Add(NewID, temp);
            return temp;

        }

        public Subtract Subtract(string name = "Subtractor")
        {
            int NewID = GetNewID();
            Subtract temp = new Subtract(NewID, name);
            TronicsByID.Add(NewID, temp);
            return temp;

        }

        public Multiply Multiply(string name = "Multiplyer")
        {
            int NewID = GetNewID();
            Multiply temp = new Multiply(NewID, name);
            TronicsByID.Add(NewID, temp);
            return temp;

        }

        public Divide Divide(string name = "Divider")
        {
            int NewID = GetNewID();
            Divide temp = new Divide(NewID, name);
            TronicsByID.Add(NewID, temp);
            return temp;

        }

        public And And(string name = "And")
        {
            int NewID = GetNewID();
            And temp = new And(NewID, name);
            TronicsByID.Add(NewID, temp);
            return temp;

        }

        public Rand Rand(string name = "Rand")
        {
            int NewID = GetNewID();
            Rand temp = new Rand(NewID, name);
            TronicsByID.Add(NewID, temp);
            return temp;

        }

        public Button Button(int type = 1, string name = "Button")
        {
            int NewID = GetNewID();
            Button temp = new Button(NewID, name, type);
            TronicsByID.Add(NewID, temp);
            return temp;

        }

        public Proxy Proxy(string name = "Proxy")
        {
            int NewID = GetNewID();
            Proxy temp = new Proxy(NewID, name);
            TronicsByID.Add(NewID, temp);
            return temp;

        }

        public Delay Delay(string name = "Delay")
        {
            int NewID = GetNewID();
            Delay temp = new Delay(NewID, name);
            TronicsByID.Add(NewID, temp);
            return temp;

        }

        public Display Display(string name = "Display", string DefaultMessage = "")
        {
            int NewID = GetNewID();
            Display temp = new Display(NewID, name);
            temp.data = DefaultMessage;
            TronicsByID.Add(NewID, temp);
            return temp;
        }

        public Mover Mover(string name = "Mover")
        {
            int NewID = GetNewID();
            Mover temp = new Mover(NewID, name);
            TronicsByID.Add(NewID, temp);
            return temp;

        }

        public Rotor Rotor(string name = "Rotor")
        {
            int NewID = GetNewID();
            Rotor temp = new Rotor(NewID, name);
            TronicsByID.Add(NewID, temp);
            return temp;

        }

        public Transmitter Transmitter(string name = "Transmitter")
        {
            int NewID = GetNewID();
            Transmitter temp = new Transmitter(NewID, name);
            TronicsByID.Add(NewID, temp);
            return temp;

        }

        public Reciever Reciever(string name = "Reciever")
        {
            int NewID = GetNewID();
            Reciever temp = new Reciever(NewID, name);
            TronicsByID.Add(NewID, temp);
            return temp;

        }

        public Keyboard Keyboard(string name = "Keyboard")
        {
            int NewID = GetNewID();
            Keyboard temp = new Keyboard(NewID, name);
            TronicsByID.Add(NewID, temp);
            return temp;

        }

        public IfEqual IfEqual(string name = "IfEqual")
        {
            int NewID = GetNewID();
            IfEqual temp = new IfEqual(NewID, name);
            TronicsByID.Add(NewID, temp);
            return temp;

        }

        public IfGreater IfGreater(string name = "IfGreater")
        {
            int NewID = GetNewID();
            IfGreater temp = new IfGreater(NewID, name);
            TronicsByID.Add(NewID, temp);
            return temp;

        }

        public Distance Distance(string name = "Distance")
        {
            int NewID = GetNewID();
            Distance temp = new Distance(NewID, name);
            TronicsByID.Add(NewID, temp);
            return temp;

        }

        public DataBlock DataBlock(string name = "DataNode", string data = "")
        {
            int NewID = GetNewID();
            DataBlock temp = new DataBlock(NewID, name);
            temp.data = data;
            TronicsByID.Add(NewID, temp);
            return temp;
        }

    }
}
