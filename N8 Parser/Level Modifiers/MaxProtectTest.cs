using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media.Media3D;
using N8Parser.Tronics;
using N8Parser.Level_Modifiers;

namespace N8Parser
{
    public class MaxProtectTest
    {
        public static void GenerateProxyBimesh()
        {
            string SavePath = @"C:\Program Files (x86)\N8\Saves\proxybimesh.ncd";

            N8Level Level = new N8Level();
            

            N8BlockFactory LevelBlocks = Level.blocks;

            List<FlowTronic> proxies = new List<FlowTronic>();
            for (int i = -330; i <= 330; i += 60)
            {
                for (int j = -330; j <= 330; j += 60)
                {
                    int x = i;
                    int y = j;
                    if ((i/60) % 2 == 0)
                    {
                        y = j + 15;
                    }
                    else
                    {
                        y = j - 15;
                    }
                    if ((j / 60) % 2 == 0)
                    {
                        x = i + 15;
                    }
                    else
                    {
                        x = i - 15;
                    }

                    Vector3D pos = new Vector3D(x, y, 1000);
                    
                    FlowTronic TopProxy = LevelBlocks.Proxy(x + "," + y);
                    pos.Z = 270;
                    TopProxy.position = pos;

                    FlowTronic BottomProxy = LevelBlocks.Proxy(x + "," + y);
                    pos.Z = 0;
                    BottomProxy.position = pos;

                    proxies.Add(TopProxy);
                    proxies.Add(BottomProxy);
                }
            }
            FleeTronics(LevelBlocks, proxies);

            Utilities.Save(SavePath, Level);
        }

        private static void FleeTronics(N8BlockFactory LevelBlocks, List<FlowTronic> alerts, bool rotate = true, bool AttachLevelBlocks = false, string password = null)
        {
            Quaternion UpsideDown = new Quaternion(new Vector3D(1, 0, 0), 180);

            FlowTronic InitTronic;
            if (password == null)
            {
                InitTronic = LevelBlocks.Keyboard("Set Cell Password");
                InitTronic.position = new Vector3D(30, -30, 5);
            }
            else
            {
                InitTronic = LevelBlocks.Button(1, "Setup Reciever");
            }

            
            TronicSequence Reciever = new TronicSequence(InitTronic);
            
            
            DataBlock Channel = Reciever.NewDataBlock("Channel", "1025");
            DataBlock UsernameRec = Reciever.NewDataBlock("Recieved Username");
            DataBlock MessageRec = Reciever.NewDataBlock("Recieved Message");

            DataBlock UsernameStore = Reciever.NewDataBlock("Stored Username", "Tacroy");
            //Not used atm
            //DataBlock AlternateUsernameStore = Reciever.NewDataBlock("Alternate Username", "nobody");
            DataBlock Password = Reciever.NewDataBlock("Password", password ?? "");
            if (password == null)
            {
                InitTronic.DataOutA(Password.Out);
            }

            Reciever.RadioReciever(Channel.In, UsernameRec.Out, MessageRec.Out);

            TronicSequence PasswordTest = new TronicSequence();

            PasswordTest.IfEqual(Password.In, MessageRec.In, "PasswordCheck");

            TronicSequence NameTest = new TronicSequence();

            NameTest.IfEqual(UsernameRec.In, UsernameStore.In, "NameCheck");



            TronicSequence FlipFlop = TronicsTesting.Ringbuffer(new List<string>(new string[] { "0", "1" }));
            DataBlock ControlBit = FlipFlop.data[FlipFlop.data.Count - 1];
            DataBlock ReturnPos = Reciever.NewDataBlock("Return Position", "v0,0,0");


            Reciever.Append(NameTest)
                    .Append(PasswordTest)
                    .Append(FlipFlop)
                    .Mover(ReturnPos.In, ReturnPos.Out, "Return Mover")
                    .RadioTransmit(Channel.In, ControlBit.In, "Yeller");

            TronicSequence RandomBottomVector = TronicsTesting.RandomXYVectorGenerator(-1000, 1000, -1000);
            TronicSequence RandomTopVector = TronicsTesting.RandomXYVectorGenerator(-2000, 2000, 2000);
            DataBlock RandVectTop = RandomTopVector.data[RandomTopVector.data.Count - 1];
            DataBlock RandVectBottom = RandomBottomVector.data[RandomBottomVector.data.Count - 1];
            
            
            Random rand = new Random();
            
            Vector3D AttachOffset = rand.NextVector(new Vector3D(60,60,0), new Vector3D(-60,-60,0));

            foreach (FlowTronic alert in alerts)
            {
                alert.position += AttachOffset;
                RandomTopVector.GetFirst().FlowInFrom(alert);
            }

            N8Block TronicAttach = LevelBlocks.GenerateBlock("letter.period", "Attach Point");
            TronicAttach.position = -AttachOffset;
            TronicAttach.position.Z = 500;

            TronicSequence MovementLogic = new TronicSequence();
            MovementLogic.Append(RandomTopVector);

            MovementLogic.IfGreater(ControlBit.In, null, "Control");

            if (rotate)
            {
                TronicSequence ProxyRotor = new TronicSequence();
                DataBlock Quantity = ProxyRotor.NewDataBlock("Amount", "q0,0,1,0");
                DataBlock Current = ProxyRotor.NewDataBlock("Current", "q1,0,0,0");

                ProxyRotor.Multiply(Quantity.In, Current.In, Current.Out, "Unit step")
                          .Rotor(Current.In, null, "Rotate1");
                MovementLogic.Append(ProxyRotor);
            }
            //Mega and vacubombs take 15 seconds to explode, regular 10. Add 2 seconds up in the sky for a buffer.
            //Also keep in mind that because we fly up then delay, once we're up there we're a random sky mover so that should be safe enough.
            DataBlock DelayTime = MovementLogic.NewDataBlock("Delay", "17");
            MovementLogic.Mover(RandVectTop.In, null, "Flee Mover 1")
                         .Delay(DelayTime.In)
                         .Append(RandomBottomVector)
                         .Mover(RandVectBottom.In, null, "Flee Mover 2");

            MovementLogic.GetCurrent().Item1.FlowOutTo((FlowTronic)Reciever.tronics.Tronics.Last());

            MovementLogic.LayoutDense(AttachOffset);
            Reciever.LayoutDense(AttachOffset);
            
            //Attach everything
            Reciever.AttachAllNonPositional(TronicAttach, false);
            MovementLogic.AttachAllNonPositional(TronicAttach, false);

            foreach (N8Tronic t in alerts)
            {
                TronicAttach.AttachToMe(t);
            }

            LevelBlocks.CopyFromDestructive(MovementLogic.tronics);
            LevelBlocks.CopyFromDestructive(Reciever.tronics);

            if (AttachLevelBlocks)
            {
                foreach (N8Block b in from N8Block t in LevelBlocks.Blocks where t.name != "Attach Point" select t)
                {
                    b.position += AttachOffset;
                    TronicAttach.AttachToMe(b);
                }
            }

            N8Tronic RetMover = (from N8Tronic t in LevelBlocks.Tronics where t.name == "Return Mover" select t).First();
            N8Tronic FleeMover1 = (from N8Tronic t in LevelBlocks.Tronics where t.name == "Flee Mover 1" select t).First();
            N8Tronic FleeMover2 = (from N8Tronic t in LevelBlocks.Tronics where t.name == "Flee Mover 2" select t).First();
            

            RetMover.position.Z = -1000;
            FleeMover1.position.Z = 0;
            FleeMover1.position.X = -30;
            FleeMover2.position.Z = 0;
            FleeMover2.position.X = 30;
            RetMover.Detach();
            FleeMover1.Detach();
            FleeMover2.Detach();

            if (rotate)
            {
                N8Tronic Rotor = (from N8Tronic t in LevelBlocks.Tronics where t.name == "Rotate1" select t).First();
                TronicAttach.AttachToMe(Rotor);
            }

            Console.WriteLine("Total block count: " + (LevelBlocks.Blocks.Count + LevelBlocks.Tronics.Count));
        }

        public static void GenerateProxyBubble()
        {
            string SavePath = @"C:\Program Files (x86)\N8\Saves\proxybubble.ncd";
            N8Level bubble = GetProxyBubble();
            if ((bubble.blocks.Blocks.Count + bubble.blocks.Tronics.Count) > 349)
            {
                Console.WriteLine("Warning! Block count is: " + (bubble.blocks.Blocks.Count + bubble.blocks.Tronics.Count));
                Console.Read();
            }
            Utilities.Save(SavePath, bubble);
        }

        public static N8Level GetMachoBubble()
        {

            N8Level Level = new N8Level();
            Quaternion UpsideDown = new Quaternion(new Vector3D(1, 0, 0), 180);

            N8BlockFactory LevelBlocks = Level.blocks;


            List<Tuple<Vector3D, Quaternion>> proxies = new List<Tuple<Vector3D, Quaternion>>();
            List<Tuple<Vector3D, Quaternion>> targets = new List<Tuple<Vector3D, Quaternion>>();

            proxies.Add(Tuple.Create(new Vector3D(120, 0, 250), new Quaternion()));
            proxies.Add(Tuple.Create(new Vector3D(-120, 0, 250), new Quaternion()));
            proxies.Add(Tuple.Create(new Vector3D(0, 120, 250), new Quaternion()));
            proxies.Add(Tuple.Create(new Vector3D(0, -120, 250), new Quaternion()));
            proxies.Add(Tuple.Create(new Vector3D(50, 50, 50), new Quaternion()));
            proxies.Add(Tuple.Create(new Vector3D(-50, 50, 50), new Quaternion()));
            proxies.Add(Tuple.Create(new Vector3D(50, -50, 50), new Quaternion()));
            proxies.Add(Tuple.Create(new Vector3D(-50, -50, 50), new Quaternion()));

            proxies.AddRange(Utilities.EvenSphere(new Vector3D(0, 0, 50), 60, 250, (double)8 / 16 * Math.PI));
            //points.AddRange(Utilities.EvenSphere(new Vector3D(0, 0, 50), 85, 175, (double)8 / 16 * Math.PI));
            proxies.AddRange(Utilities.EvenCircle(new Vector3D(0, 0, 490), 45, 150));
            proxies.AddRange(Utilities.EvenCircle(new Vector3D(0, 0, 445), 45, 75));
            proxies.AddRange(Utilities.EvenCircle(new Vector3D(0, 0, 400), 45, 20));
            //points.AddRange(Utilities.EvenCircle(new Vector3D(0, 0, 50), 90, 700));
            //*/

            
            targets.AddRange(Utilities.EvenSphere(new Vector3D(0, 0, 50), 45, 50, (double)10 / 16 * Math.PI));

            Quaternion ProxyRot = new Quaternion(new Vector3D(0, 0, 1), -90) * new Quaternion(new Vector3D(0, 1, 0), 90);
            Quaternion TargetRot = new Quaternion();//new Vector3D(0, 1, 0), -90);

            List<FlowTronic> alerts = new List<FlowTronic>();
            for (int i = 0; i < proxies.Count; i++)
            {
                Tuple<Vector3D, Quaternion> t = proxies[i];
                FlowTronic prox = LevelBlocks.Proxy("i=" + i);
                prox.position = t.Item1;

                alerts.Add(prox);
            }

            for (int i = 0; i < targets.Count; i++)
            {
                Tuple<Vector3D, Quaternion> t = targets[i];
                FlowTronic target = LevelBlocks.Target("i=" + i);
                target.position = t.Item1;
                target.rotation = TargetRot * t.Item2;

                alerts.Add(target);
            }

            FleeTronics(LevelBlocks, alerts);

            //Console.WriteLine("Total number of blocks used: " + (LevelBlocks.Tronics.Count + LevelBlocks.Blocks.Count));
            return Level;

        }

        public static N8Level GetHutProxies(string password=null)
        {
            N8Level Level = new N8Level(Utilities.GetDefaultSaveFolder() + "shrine_hut_final.ncd");
            int counter = 500;
            foreach (N8Block b in Level.blocks.Blocks)
            {
                b.ID = counter;
                counter++;
            }

            Level.blocks.SetNextID(counter);

            Quaternion UpsideDown = new Quaternion(new Vector3D(1, 0, 0), 180);

            N8BlockFactory LevelBlocks = Level.blocks;

            List<Tuple<Vector3D, Quaternion>> targets = new List<Tuple<Vector3D, Quaternion>>();
            List<Tuple<Vector3D, Quaternion>> proxies = new List<Tuple<Vector3D, Quaternion>>();
            proxies.AddRange(Utilities.EvenCircle(new Vector3D(0, 0, -1), 45, 45));
            proxies.Add(Tuple.Create(new Vector3D(50, 50, 50), new Quaternion()));
            proxies.Add(Tuple.Create(new Vector3D(-50, 50, 50), new Quaternion()));
            proxies.Add(Tuple.Create(new Vector3D(50, -50, 50), new Quaternion()));
            proxies.Add(Tuple.Create(new Vector3D(-50, -50, 50), new Quaternion()));
            proxies.Add(Tuple.Create(new Vector3D(-80, -80, 90), new Quaternion()));
            proxies.Add(Tuple.Create(new Vector3D(80, -80, 90), new Quaternion()));
            proxies.Add(Tuple.Create(new Vector3D(-80, 80, 90), new Quaternion()));
            proxies.Add(Tuple.Create(new Vector3D(80, 80, 90), new Quaternion()));
            //proxies.AddRange(Utilities.EvenSphere(new Vector3D(0, 0, 50), 60, 250, (double)8 / 16 * Math.PI));

            for (int i = -160; i <= 160; i += 40)
            {
                for (int j = -40; j <= 40; j += 40)
                {
                    proxies.Add(Tuple.Create(new Vector3D(i, j, 270), new Quaternion()));
                }
            }

            for (int i = -130; i <= 140; i += 45)
            {
                for (int j = -90; j <= 90; j += 45)
                {
                    targets.Add(Tuple.Create(new Vector3D(i, j, 300), new Quaternion()));
                }
            }

            
            targets.AddRange(Utilities.EvenSphere(new Vector3D(0, 0, 50), 45, 50, (double)10 / 16 * Math.PI));

            Quaternion ProxyRot = new Quaternion(new Vector3D(0, 0, 1), -90) * new Quaternion(new Vector3D(0, 1, 0), 90);
            Quaternion TargetRot = new Quaternion();//new Vector3D(0, 1, 0), -90);

            List<FlowTronic> alerts = new List<FlowTronic>();
            for (int i = 0; i < proxies.Count; i++)
            {
                Tuple<Vector3D, Quaternion> t = proxies[i];
                FlowTronic prox = LevelBlocks.Proxy("i=" + i);
                prox.position = t.Item1;

                alerts.Add(prox);
            }

            for (int i = 0; i < targets.Count; i++)
            {
                Tuple<Vector3D, Quaternion> t = targets[i];
                FlowTronic target = LevelBlocks.Target("i=" + i);
                target.position = t.Item1;
                target.rotation = TargetRot * t.Item2;

                alerts.Add(target);
            }

            FleeTronics(LevelBlocks, alerts, false, true, password);

            //Console.WriteLine("Total number of blocks used: " + (LevelBlocks.Tronics.Count + LevelBlocks.Blocks.Count));
            return Level;
        }

        public static N8Level GetProxyBubble()
        {

            N8Level Level = new N8Level();
            Quaternion UpsideDown = new Quaternion(new Vector3D(1, 0, 0), 180);

            N8BlockFactory LevelBlocks = Level.blocks;

            
            List<Tuple<Vector3D, Quaternion>> proxies = new List<Tuple<Vector3D,Quaternion>>();
            //points.AddRange(Utilities.EvenCircle(new Vector3D(0, 0, -1), 45, 45));
            //points.AddRange(Utilities.EvenCircle(new Vector3D(0, 0, 250), 45, 45));
            
            proxies.Add(Tuple.Create(new Vector3D(120, 0, 250), new Quaternion()));
            proxies.Add(Tuple.Create(new Vector3D(-120, 0, 250), new Quaternion()));
            proxies.Add(Tuple.Create(new Vector3D(0, 120, 250), new Quaternion()));
            proxies.Add(Tuple.Create(new Vector3D(0, -120, 250), new Quaternion()));
            proxies.Add(Tuple.Create(new Vector3D(50, 50, 50), new Quaternion()));
            proxies.Add(Tuple.Create(new Vector3D(-50, 50, 50), new Quaternion()));
            proxies.Add(Tuple.Create(new Vector3D(50, -50, 50), new Quaternion()));
            proxies.Add(Tuple.Create(new Vector3D(-50, -50, 50), new Quaternion()));
            
            proxies.AddRange(Utilities.EvenSphere(new Vector3D(0, 0, 50), 85, 480, (double)8 / 16 * Math.PI));
            //points.AddRange(Utilities.EvenSphere(new Vector3D(0, 0, 50), 85, 175, (double)8 / 16 * Math.PI));
            proxies.AddRange(Utilities.EvenCircle(new Vector3D(0, 0, 0), 90, 300));
            proxies.AddRange(Utilities.EvenCircle(new Vector3D(0, 0, 0), 80, 250));
            proxies.AddRange(Utilities.EvenCircle(new Vector3D(0, 0, 0), 70, 200));
            proxies.AddRange(Utilities.EvenCircle(new Vector3D(0, 0, 0), 60, 150));
            proxies.AddRange(Utilities.EvenCircle(new Vector3D(0, 0, 0), 50, 75));
            proxies.AddRange(Utilities.EvenCircle(new Vector3D(0, 0, 0), 45, 20));
            //points.AddRange(Utilities.EvenCircle(new Vector3D(0, 0, 50), 90, 700));
            //*/

            List<Tuple<Vector3D, Quaternion>> targets = new List<Tuple<Vector3D, Quaternion>>();
            targets.AddRange(Utilities.EvenSphere(new Vector3D(0, 0, 50), 45, 50, (double)10 / 16 * Math.PI));

            Quaternion ProxyRot = new Quaternion(new Vector3D(0, 0, 1), -90) * new Quaternion(new Vector3D(0,1,0), 90);
            Quaternion TargetRot = new Quaternion();//new Vector3D(0, 1, 0), -90);

            List<FlowTronic> alerts = new List<FlowTronic>();
            for(int i = 0; i < proxies.Count; i++)
            {
                Tuple<Vector3D, Quaternion> t = proxies[i];
                FlowTronic prox = LevelBlocks.Proxy("i=" + i);
                prox.position = t.Item1;

                alerts.Add(prox);
            }

            for (int i = 0; i < targets.Count; i++)
            {
                Tuple<Vector3D, Quaternion> t = targets[i];
                FlowTronic target = LevelBlocks.Target("i=" + i);
                target.position = t.Item1;
                target.rotation = TargetRot * t.Item2;

                alerts.Add(target);
            }

            FleeTronics(LevelBlocks, alerts);

            //Console.WriteLine("Total number of blocks used: " + (LevelBlocks.Tronics.Count + LevelBlocks.Blocks.Count));
            return Level;

        }

        public static void GenerateProxyMatrix()
        {
            string TronicsPath = @"C:\Program Files (x86)\N8\Saves\maxprotect_tronics_base.ncd";
            string SavePath = @"C:\Program Files (x86)\N8\Saves\maxprotect_tronics_base_proxies.ncd";

            N8Level tronics = new N8Level(TronicsPath);
            N8Level proxies = new N8Level();

            N8BlockFactory LevelBlocks = proxies.blocks;
            List<N8Tronic> ProxyBlocks = new List<N8Tronic>();

            int stepsize = 36;

            for (int i = -90 + stepsize; i <= 90 - stepsize; i += stepsize)
            {
                for (int j = -90 + stepsize; j <= 90 - stepsize; j += stepsize)
                {
                    N8Tronic LowerProxy = LevelBlocks.GenerateTronic("rproximity", "Detector Mesh");
                    LowerProxy.position.X = i;
                    LowerProxy.position.Y = j;
                    LowerProxy.position.Z = 45;

                    N8Tronic UpperProxy = LevelBlocks.GenerateTronic("rproximity", "Detector Mesh");
                    UpperProxy.position.X = i * 2.5;
                    UpperProxy.position.Y = j * 2.5;
                    UpperProxy.position.Z = 335;
                    UpperProxy.rotation = new Quaternion(new Vector3D(1, 0, 0), 90);

                    ProxyBlocks.Add(LowerProxy);
                    ProxyBlocks.Add(UpperProxy);
                }
            }

            proxies.MergeWithDestructive(tronics);

            N8Tronic MoverGateway = (from N8Tronic b in proxies.blocks.Tronics where b.type == "cifgreat" select b).First();
            
            N8Block ControlPoint = LevelBlocks.GenerateBlock("snowmancoal", "Control Point");
            ControlPoint.position.X = -100;

            foreach (N8Tronic prox in ProxyBlocks)
            {
                ControlPoint.AttachToMe(prox);
                //MoverGateway.WireTo(prox, Tronics.NodeType.FlowIn, Tronics.NodeType.FlowOutA);
            }


            Console.Read();
            if (!File.Exists(SavePath))
            {
                using (File.Create(SavePath)) { }
            }

            using (StreamWriter sw = new StreamWriter(File.Open(SavePath, FileMode.Truncate, FileAccess.Write, FileShare.None)))
            {
                sw.WriteLine(proxies.GenerateSaveFile());
            }
        }
        public static void GenerateLevel()
        {
            string SavePath = @"C:\Program Files (x86)\N8\Saves\maxprotecttest.ncd";
            string TronicsPath = @"C:\Program Files (x86)\N8\Saves\maxprotect_tronics_base_proxies.ncd";
            //I like black best, so make it more likely to show up
            //string[] colors = { "blue", "green", "orange", "purple", "red", "black", "black" };
            //Apparently nobody else likes the colors, so we're back to just black.
            string[] colors = { "black" };
            N8Level Level = new N8Level();
            N8Level tronics = new N8Level(TronicsPath);


            N8BlockFactory LevelBlocks = Level.blocks;

            Random rand = new Random();


            for (int i = -1000; i < 1380; i = i + 70)
            {
                for (int j = -1333; j <= 2000; j = j + 1333)
                {
                    for(int k = -1333; k <= 2000; k = k + 1333)
                    {
                        if (!(k == 0 && j == 0))
                        {
                            string color = colors[rand.Next(0, colors.Length)];
                            N8Block CurrentBlock = LevelBlocks.GenerateBlock("simpleland" + color, "Vault of the Heavens");
                            CurrentBlock.position.Z = i;
                            CurrentBlock.position.X = j;
                            CurrentBlock.position.Y = k;


                        }
                    }
                }
            }

            Level.MergeWithDestructive(tronics);
            

            //Non position dependent tronics - tronics whose position doesn't matter
            var NPDTronics = from N8Tronic t in LevelBlocks.Tronics
                                        where !(t.type == "rproximity" || t.type == "rkeyboard" || t.type == "tmover")
                                        select t;
            N8Block AttachmentPoint = (from N8Block b in LevelBlocks.Blocks
                                       where b.name == "Attach Point"
                                       select b).First();

            foreach (N8Tronic t in NPDTronics)
            {
                t.Detach();
                t.position.X = 0;
                t.position.Y = 0;
                t.position.Z = -250;
                AttachmentPoint.AttachToMe(t);
            }

            string ret = Level.GenerateSaveFile();
            Console.Read();

            if (!File.Exists(SavePath))
            {
                using (File.Create(SavePath)) { }
            }

            using (StreamWriter sw = new StreamWriter(File.Open(SavePath, FileMode.Truncate, FileAccess.Write, FileShare.None)))
            {
                sw.WriteLine(ret);
            }

            
            Console.WriteLine(ret);
            
        }
    }
}
