using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using N8Parser.Tronics;

namespace N8Parser
{
    class N8Level
    {
        public N8BlockFactory blocks;

        public N8Level()
        {
            blocks = new N8BlockFactory();
        }

        public N8Level(string SavePath) 
        {
            int lineNum = 0;
            blocks = new N8BlockFactory();
            using (StreamReader sr = new StreamReader(File.OpenRead(SavePath)))
            {
                string input;

                //Parse blocks
                while ((input = sr.ReadLine()) != "tronics" && input != null)
                {
                    lineNum++;
                    blocks.AddBlockFromSave(input);
                }

                //Parse tronics (not sure why they're different, the format is the same
                //But Aion put them in their own section so I'll do the same
                while ((input = sr.ReadLine()) != "attach" && input != null)
                {
                    lineNum++;
                    blocks.AddTronicFromSave(input);
                }

                //Parse attachments
                while ((input = sr.ReadLine()) != "wire" && input != null)
                {
                    lineNum++;
                    N8Block Attachee;
                    N8Block Attacher;
                    int AttacheeID = int.Parse(input.Split(':')[0]);
                    int AttacherID = int.Parse(input.Split(':')[1]);

                    bool foundAttachee = blocks.GetBlock(AttacheeID, out Attachee);
                    bool foundAttacher = blocks.GetBlock(AttacherID, out Attacher);

                    if (!foundAttachee || !foundAttacher) //Screw you DeMorgan!
                    {
                        string message = "I found you an attachment warning but I forgotted it";

                        if (!foundAttachee)
                        {
                            message = "Could not find attachee, id = " + AttacheeID;
                        }
                        if (!foundAttacher)
                        {
                            message = "Could not find attacher, id = " + AttacherID + "; this is probably the shrine unless it happens with more than one ID";
                        }
                        if (!foundAttacher && !foundAttachee)
                        {
                            message = "Could not find either attacher, id = " + AttacherID + " or attachee, id = " + AttacheeID;
                        }

                        warn(message, lineNum);


                        //If it's just the attacher and not the attachee, this is probably stuff that was attached to the shrine. 
                        //Make a fake block for it.

                        if (foundAttachee && !foundAttacher)
                        {
                            Attacher = blocks.GenerateShrine(AttacherID);
                        }
                        else
                        {
                            continue;
                        }
                    }

                    //Found the attacher and the attachee, do eeit.
                    Attacher.AttachToMe(Attachee);
                }

                //And then do the wires
                while ((input = sr.ReadLine()) != null)
                {
                    lineNum++;

                    string[] parts = input.Split(',');
                    bool FoundLeft;
                    N8Tronic Left;
                    int LeftID = int.Parse(parts[0]);
                    NodeType LeftNode = (NodeType)int.Parse(parts[1]);

                    bool FoundRight;
                    N8Tronic Right;
                    int RightID = int.Parse(parts[2]);
                    NodeType RightNode = (NodeType) int.Parse(parts[3]);

                    //only tronics have wires, so only look in the tronics
                    FoundLeft = blocks.TronicsByID.TryGetValue(LeftID, out Left);
                    FoundRight = blocks.TronicsByID.TryGetValue(RightID, out Right);

                    if (!FoundRight || !FoundLeft)
                    {
                        string message = "I found you a wire warning but I forgotted it";

                        if (!FoundRight)
                        {
                            message = "Could not find tronic id = " + RightID;
                        }
                        if (!FoundLeft)
                        {
                            message = "Could not find tronic id = " + LeftID;
                        }
                        if (!FoundLeft && !FoundRight)
                        {
                            message = "Could not find tronic id = " + LeftID + " or id = " + RightID;
                        }

                        warn(message, lineNum);
                        continue;
                    }
                    Left.WireTo(Left.GetNode(LeftNode), Right.GetNode(RightNode));
                }
            }
        }

        public string GenerateSaveFile()
        {
            //For sanity checking - throw an exception if we see the same ID twice
            HashSet<int> SeenIDs = new HashSet<int>();

            StringBuilder ret = new StringBuilder("");

            foreach(N8Block block in blocks.BlocksByID.Values)
            {
                if (!SeenIDs.Add(block.ID))
                {
                    throw new Exception("Adding the same ID twice! Abjort!");
                }
                ret.AppendLine(block.ToString());
            }

            ret.AppendLine("tronics");

            foreach (N8Block tronic in blocks.TronicsByID.Values)
            {
                if (!SeenIDs.Add(tronic.ID))
                {
                    throw new Exception("Adding the same ID twice! Abjort!");
                }
                ret.AppendLine(tronic.ToString());
            }

            ret.AppendLine("attach");

            //Note: nothing can attach to a tronic, so I'm not going through them.
            //If this ever gets updated, consider concatenating BlocksByID and TronicsByID.
            foreach (N8Block block in blocks.BlocksByID.Values)
            {
                if (block.Attachees != null)
                {
                    foreach (N8Block attachee in block.Attachees)
                    {
                        ret.AppendLine(attachee.ID + ":" + block.ID);
                    }
                }
            }

            ret.AppendLine("wire");

            //This is trickier since wires are a two-way connection, so there's gonna be duplicate data.
            //Fortunately, we should be able to just put it into a properly configured Set and then pull everything out,
            //since Sets don't store duplicate information.

            SortedSet<Wire> wiring = new SortedSet<Wire>();
                    

            foreach (N8Tronic tronic in blocks.TronicsByID.Values)
            {
                if (tronic.WiredTo != null)
                {
                    wiring.UnionWith(tronic.WiredTo);
                }
            }

            foreach (var wire in wiring)
            {
                ret.AppendLine(wire.ToString());
            }
            ret.Length = ret.Length - 2;
            return ret.ToString();
        }

        public void MergeWithDestructive(N8Level other)
        {
            this.blocks.CopyFromDestructive(other.blocks);
        }

        public void MergeWithSafe(N8Level other)
        {
            this.blocks.CopyFromNonDestructive(other.blocks);
        }

        public void warn(string warning, int lineNum)
        {
            Console.WriteLine("Warning on line " + lineNum + ": " + warning);
        }
    }


}
