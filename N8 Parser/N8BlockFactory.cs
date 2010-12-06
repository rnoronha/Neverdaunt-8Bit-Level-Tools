using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using N8Parser.Tronics;
using System.Collections;


namespace N8Parser
{
    [Serializable]
    public class N8BlockFactory
    {
        public List<N8Block> BlocksByID;
        public List<N8Tronic> TronicsByID;
        private int MaxID = 1;
        public bool GeneratedShrine = false;
        public int BlockCount = 0;
        public static int MaxBlockCount = 349;

        public N8BlockFactory() 
        {
            BlocksByID = new List<N8Block>();
            TronicsByID = new List<N8Tronic>();
        }

        public N8Block AddBlockFromSave(string raw)
        {
            IncrementBlockCount();
            string[] parts = raw.Split(':');
            N8Block current = new N8Block(parts);

            BlocksByID.Add(current);
            MaxID = Math.Max(current.ID, MaxID);
            return current;
        }

        public N8Tronic AddTronicFromSave(string raw)
        {
            IncrementBlockCount();
            string[] parts = raw.Split(':');
            N8Tronic current = new N8Tronic(parts);

            TronicsByID.Add(current);
            MaxID = Math.Max(current.ID, MaxID);
            return current;
        }

        public N8Block GenerateShrine(int shrineID)
        {
            if(!GeneratedShrine)
            {
                GeneratedShrine = true;
                N8Block temp = new N8Block(shrineID);
                BlocksByID.Add(temp);
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
            BlocksByID.Add(temp);
            return temp;
        }

        public N8Tronic CloneTronic(N8Tronic b)
        {
            IncrementBlockCount();
            int NewID = GetNewID();
            N8Tronic temp = new N8Tronic(b, NewID);
            TronicsByID.Add(temp);
            return temp;
        }

        public N8Block GenerateBlock(string type, string name)
        {
            IncrementBlockCount();
            int NewID = GetNewID();
            N8Block temp = new N8Block(NewID, type, name);
            BlocksByID.Add(temp);
            return temp;
        }

        public N8Tronic GenerateTronic(string type, string name)
        {
            IncrementBlockCount();
            int NewID = GetNewID();
            N8Tronic temp = new N8Tronic(NewID, type, name);
            TronicsByID.Add(temp);
            return temp;
        }

        private void IncrementBlockCount()
        {
            BlockCount++;
        }

        public bool GetBlock(int id, out N8Block result)
        {
            bool inBlocks = false;

            var blocks = from N8Block b in BlocksByID where b.ID == id select b;
            result = null;

            if (blocks.Count() > 1)
            {
                throw new Exception("Block ID " + id + " is used by more than one block!");
            }

            if (blocks.Count() == 1)
            {
                inBlocks = true;
                result = blocks.First();
            }

            return inBlocks;
        }

        public bool GetTronic(int id, out N8Tronic result)
        {
            result = null;
            bool inTronics = false;
            
            var tronics = from N8Tronic t in TronicsByID where t.ID == id select t;

            if (tronics.Count() > 1)
            {
                throw new Exception("Tronic ID " + id + " is used by more than one tronic!");
            }
            else if (tronics.Count() == 1)
            {
                inTronics = true;
                result = tronics.First();
            }

            return inTronics;
        }

        public bool GetItem(int id, out N8Block result)
        {
            bool inTronics = false;
            bool inBlocks = false;

            var blocks = from N8Block b in BlocksByID where b.ID == id select b;
            result = null;

            if (blocks.Count() > 1)
            {
                throw new Exception("Block ID " + id + " is used by more than one block!");
            }

            if (blocks.Count() == 1)
            {
                inBlocks = true;
                result = blocks.First();
            }

            var tronics = from N8Tronic t in TronicsByID where t.ID == id select t;

            if (tronics.Count() > 1)
            {
                throw new Exception("Tronic ID " + id + " is used by more than one tronic!");
            }
            else if (tronics.Count() == 1)
            {
                inTronics = true;
                result = tronics.First();
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
            var tronics = from N8Tronic t in TronicsByID where t.ID == id select t;
            var blocks = from N8Block b in BlocksByID where b.ID == id select b;

            if (blocks.Count() > 1)
            {
                throw new Exception("More than one block has ID " + id);
            }
            if (tronics.Count() > 1)
            {
                throw new Exception("More than one tronic has ID " + id);
            }

            return tronics.Count() == 1 || blocks.Count() == 1;
        }

        public static N8BlockFactory operator + (N8BlockFactory me, Vector3D offset)
        {
            foreach(N8Block b in me.BlocksByID)
            {
                b.position += offset;
            }
            foreach(N8Tronic t in me.TronicsByID)
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
                block.ChangeID(newID);
                return true;
            }
        }
        
        //This is still kind of broken, so don't trust it to re-id blocks properly, especially when they conflict with tronics.
        internal void CopyFromDestructive(N8BlockFactory copy)
        {
            //We're merging the other guy with us, so when it comes down to it it's his shit that needs to change
            foreach (N8Block b in copy.BlocksByID)
            {
                ChangeID(b);
            }

            //If a block and a tronic conflict, we'll change the block.
            foreach (N8Tronic t in copy.TronicsByID)
            {
                if (IDInUse(t.ID))
                {
                    //Is it being used by a block?
                    N8Block conflictor;
                    if (GetBlock(t.ID, out conflictor))
                    {
                        //Then have him change, the lazy bastard
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
                MaxID = Math.Max(TronicsByID.Max((x) => x.ID), BlocksByID.Max((x) => x.ID));
            }
            else if (TronicsByID.Count > 0)
            {
                MaxID = TronicsByID.Max((x) => x.ID);
            }
            else if (BlocksByID.Count > 0)
            {
                MaxID = BlocksByID.Max((x) => x.ID);
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
            TronicsByID.Add(temp);
            return temp;

        }

        public Subtract Subtract(string name = "Subtractor")
        {
            int NewID = GetNewID();
            Subtract temp = new Subtract(NewID, name);
            TronicsByID.Add(temp);
            return temp;

        }

        public Multiply Multiply(string name = "Multiplyer")
        {
            int NewID = GetNewID();
            Multiply temp = new Multiply(NewID, name);
            TronicsByID.Add(temp);
            return temp;

        }

        public Divide Divide(string name = "Divider")
        {
            int NewID = GetNewID();
            Divide temp = new Divide(NewID, name);
            TronicsByID.Add(temp);
            return temp;

        }

        public And And(string name = "And")
        {
            int NewID = GetNewID();
            And temp = new And(NewID, name);
            TronicsByID.Add(temp);
            return temp;

        }

        public Rand Rand(string name = "Rand")
        {
            int NewID = GetNewID();
            Rand temp = new Rand(NewID, name);
            TronicsByID.Add(temp);
            return temp;

        }

        public Button Button(int type = 1, string name = "Button")
        {
            int NewID = GetNewID();
            Button temp = new Button(NewID, name, type);
            TronicsByID.Add(temp);
            return temp;

        }

        public Proxy Proxy(string name = "Proxy")
        {
            int NewID = GetNewID();
            Proxy temp = new Proxy(NewID, name);
            TronicsByID.Add(temp);
            return temp;

        }

        public Delay Delay(string name = "Delay")
        {
            int NewID = GetNewID();
            Delay temp = new Delay(NewID, name);
            TronicsByID.Add(temp);
            return temp;

        }

        public Display Display(string name = "Display", string DefaultMessage = "")
        {
            int NewID = GetNewID();
            Display temp = new Display(NewID, name);
            temp.data = DefaultMessage;
            TronicsByID.Add(temp);
            return temp;
        }

        public Mover Mover(string name = "Mover")
        {
            int NewID = GetNewID();
            Mover temp = new Mover(NewID, name);
            TronicsByID.Add(temp);
            return temp;

        }

        public Rotor Rotor(string name = "Rotor")
        {
            int NewID = GetNewID();
            Rotor temp = new Rotor(NewID, name);
            TronicsByID.Add(temp);
            return temp;

        }

        public Transmitter Transmitter(string name = "Transmitter")
        {
            int NewID = GetNewID();
            Transmitter temp = new Transmitter(NewID, name);
            TronicsByID.Add(temp);
            return temp;

        }

        public Reciever Reciever(string name = "Reciever")
        {
            int NewID = GetNewID();
            Reciever temp = new Reciever(NewID, name);
            TronicsByID.Add(temp);
            return temp;

        }

        public Keyboard Keyboard(string name = "Keyboard")
        {
            int NewID = GetNewID();
            Keyboard temp = new Keyboard(NewID, name);
            TronicsByID.Add(temp);
            return temp;

        }

        public IfEqual IfEqual(string name = "IfEqual")
        {
            int NewID = GetNewID();
            IfEqual temp = new IfEqual(NewID, name);
            TronicsByID.Add(temp);
            return temp;

        }

        public IfGreater IfGreater(string name = "IfGreater")
        {
            int NewID = GetNewID();
            IfGreater temp = new IfGreater(NewID, name);
            TronicsByID.Add(temp);
            return temp;

        }

        public Distance Distance(string name = "Distance")
        {
            int NewID = GetNewID();
            Distance temp = new Distance(NewID, name);
            TronicsByID.Add(temp);
            return temp;

        }

        public DataBlock DataBlock(string name = "DataNode", string data = "")
        {
            int NewID = GetNewID();
            DataBlock temp = new DataBlock(NewID, name);
            temp.data = data;
            TronicsByID.Add(temp);
            return temp;
        }

    }
}
