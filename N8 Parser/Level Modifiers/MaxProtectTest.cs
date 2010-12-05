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
    class MaxProtectTest
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

        private static void FleeTronics(N8BlockFactory LevelBlocks, List<FlowTronic> proxies)
        {
            Quaternion UpsideDown = new Quaternion(new Vector3D(1, 0, 0), 180);

            if (proxies.Count > 300)
            {
                Console.WriteLine("Warning, " + proxies.Count + " proxies!");
                Console.ReadLine();
            }
            Keyboard kb = LevelBlocks.Keyboard("Set Cell Password");
            kb.position = new Vector3D(30, -30, 5);

            TronicSequence Reciever = new TronicSequence(kb);
            DataBlock Channel = Reciever.NewDataBlock("Channel", "1025");
            DataBlock UsernameRec = Reciever.NewDataBlock("Recieved Username");
            DataBlock MessageRec = Reciever.NewDataBlock("Recieved Message");

            DataBlock UsernameStore = Reciever.NewDataBlock("Stored Username", "Tacroy");
            DataBlock AlternateUsernameStore = Reciever.NewDataBlock("Alternate Username", "nobody");
            DataBlock Password = Reciever.NewDataBlock("Password");


            kb.DataOutA(Password.Out);


            Reciever.RadioReciever(Channel.In, UsernameRec.Out, MessageRec.Out);

            TronicSequence PasswordTest = new TronicSequence();

            PasswordTest.IfEqual(Password.In, MessageRec.In);

            TronicSequence NameTest = new TronicSequence();

            NameTest.IfEqual(UsernameRec.In, UsernameStore.In, "Comparator",
                             new TronicSequence().IfEqual(UsernameRec.In, AlternateUsernameStore.In).FlowOutTo(PasswordTest));



            TronicSequence FlipFlop = TronicsTesting.Ringbuffer(new List<string>(new string[] { "0", "1" }));
            DataBlock ControlBit = FlipFlop.data[FlipFlop.data.Count - 1];
            DataBlock ReturnPos = Reciever.NewDataBlock("Return Position", "v0,500,0");


            Reciever.Append(NameTest)
                    .Append(PasswordTest)
                    .Append(FlipFlop)
                    .Mover(ReturnPos.In, ReturnPos.Out, "Return Mover")
                    .RadioTransmit(Channel.In, ControlBit.In, "Yeller");

            TronicSequence RandomBottomVector = TronicsTesting.RandomXYVectorGenerator(-900, 900, -1000);
            TronicSequence RandomTopVector = TronicsTesting.RandomXYVectorGenerator(-2000, 2000, 2000);
            DataBlock RandVectTop = RandomTopVector.data[RandomTopVector.data.Count - 1];
            DataBlock RandVectBottom = RandomBottomVector.data[RandomBottomVector.data.Count - 1];

            foreach (FlowTronic prox in proxies)
            {
                RandomTopVector.GetFirst().FlowInFrom(prox);
            }

            N8Block TronicAttach = LevelBlocks.GenerateBlock("letter.period", "");
            TronicAttach.position.X = 100;
            TronicAttach.position.Z = 500;


            TronicSequence ProxyRotor = new TronicSequence();
            DataBlock Quantity = ProxyRotor.NewDataBlock("Amount", "q0.008,0,-0.999,0");
            DataBlock Current = ProxyRotor.NewDataBlock("Current", "q1,0,0,0");

            ProxyRotor.Multiply(Quantity.In, Current.In, Current.Out, "Unit step")
                      .Rotor(Current.In, null, "Rotate1");


            TronicSequence MovementLogic = new TronicSequence();
            MovementLogic.Append(RandomTopVector);

            MovementLogic.IfGreater(ControlBit.In, null, "Control")
                         .Append(ProxyRotor)
                         .Mover(RandVectTop.In, null, "Flee Mover 1")
                         .Delay()
                         .Append(RandomBottomVector)
                         .Mover(RandVectBottom.In, null, "Flee Mover 2");

            MovementLogic.LayoutRandGrid(new Vector3D(80, 0, -100), UpsideDown, 90, 90);
            Reciever.LayoutRandGrid(new Vector3D(80, 0, -100), UpsideDown, 90, 90);
            foreach (N8Tronic t in Reciever.tronics.TronicsByID.Values)
            {
                TronicAttach.AttachToMeAbsolute(t);
            }

            MovementLogic.AttachAllNonPositional(TronicAttach, true);

            foreach (N8Tronic t in proxies)
            {
                TronicAttach.AttachToMe(t);
            }

            foreach (N8Tronic t in LevelBlocks.TronicsByID.Values)
            {
                if (!(t.type == "rkeyboard"))
                {
                    t.rotation = UpsideDown;
                    TronicAttach.AttachToMe(t);
                }

            }

            LevelBlocks.CopyFromDestructive(MovementLogic.tronics);
            LevelBlocks.CopyFromDestructive(Reciever.tronics);

            N8Tronic RetMover = (from N8Tronic t in LevelBlocks.TronicsByID.Values where t.name == "Return Mover" select t).First();
            N8Tronic FleeMover1 = (from N8Tronic t in LevelBlocks.TronicsByID.Values where t.name == "Flee Mover 1" select t).First();
            N8Tronic FleeMover2 = (from N8Tronic t in LevelBlocks.TronicsByID.Values where t.name == "Flee Mover 2" select t).First();
            N8Tronic Rotor = (from N8Tronic t in LevelBlocks.TronicsByID.Values where t.name == "Rotate1" select t).First();

            RetMover.position.Z = -1000;
            FleeMover1.position.Z = 100;
            FleeMover2.position.X = -10;
            FleeMover2.position.Z = 100;
            FleeMover2.position.X = 10;
            RetMover.Detach();
            FleeMover1.Detach();
            FleeMover2.Detach();

            TronicAttach.AttachToMe(Rotor);
        }

        public static void GenerateProxyBubble()
        {
            string SavePath = @"C:\Program Files (x86)\N8\Saves\proxybubble.ncd";
            N8Level bubble = GetProxyBubble();
            Utilities.Save(SavePath, bubble);
        }

        public static N8Level GetProxyBubble()
        {

            N8Level Level = new N8Level();
            Quaternion UpsideDown = new Quaternion(new Vector3D(1, 0, 0), 180);

            N8BlockFactory LevelBlocks = Level.blocks;

            
            List<Tuple<Vector3D, Quaternion>> points = new List<Tuple<Vector3D,Quaternion>>();
            points.AddRange(Utilities.EvenCircle(new Vector3D(0, 0, -1), 45, 45));
            points.AddRange(Utilities.EvenCircle(new Vector3D(0, 0, 250), 45, 45));
            points.Add(Tuple.Create(new Vector3D(120, 0, 250), new Quaternion()));
            points.Add(Tuple.Create(new Vector3D(-120, 0, 250), new Quaternion()));
            points.Add(Tuple.Create(new Vector3D(0, 120, 250), new Quaternion()));
            points.Add(Tuple.Create(new Vector3D(0, -120, 250), new Quaternion()));
            points.Add(Tuple.Create(new Vector3D(50, 50, 50), new Quaternion()));
            points.Add(Tuple.Create(new Vector3D(-50, 50, 50), new Quaternion()));
            points.Add(Tuple.Create(new Vector3D(50, -50, 50), new Quaternion()));
            points.Add(Tuple.Create(new Vector3D(-50, -50, 50), new Quaternion()));
            //*
            points.AddRange(Utilities.EvenSphere(new Vector3D(0, 0, 50), 90, 500, (double)8 / 16 * Math.PI));
            //*/

            Quaternion InitialRotation = new Quaternion(new Vector3D(0, 0, 1), -90) * new Quaternion(new Vector3D(0,1,0), 90);

            List<FlowTronic> proxies = new List<FlowTronic>();
            for(int i = 0; i < points.Count; i++)
            {
                Tuple<Vector3D, Quaternion> t = points[i];
                FlowTronic prox = LevelBlocks.Proxy("i=" + i);
                prox.position = t.Item1;
                prox.rotation = InitialRotation * t.Item2;

                proxies.Add(prox);
            }
            FleeTronics(LevelBlocks, proxies);
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

            N8Tronic MoverGateway = (from N8Tronic b in proxies.blocks.TronicsByID.Values where b.type == "cifgreat" select b).First();
            
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
            var NPDTronics = from N8Tronic t in LevelBlocks.TronicsByID.Values
                                        where !(t.type == "rproximity" || t.type == "rkeyboard" || t.type == "tmover")
                                        select t;
            N8Block AttachmentPoint = (from N8Block b in LevelBlocks.BlocksByID.Values
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
