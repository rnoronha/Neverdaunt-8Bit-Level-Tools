using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using N8Parser.Tronics;

namespace N8Parser.Level_Modifiers
{
    class EliteCellGenerator
    {
        public static void GenerateLevel()
        {
            string SavePath = @"C:\Program Files (x86)\N8\Saves\elite_world.ncd";
            N8Level Cell = new N8Level();

            Random rand = new Random(10);
            Vector3D Center = new Vector3D(0, 0, 1000);
            List<Tuple<Vector3D, Quaternion>> blocks = Utilities.GenerateSphere(Center, 500, 48).OrderBy((x) => x.Item1.Z).ToList();

            string[] colors = { "blue", "green", "orange", "purple", "red", "black"};

            string[] names = { "MYSTERY", "WHAT", "!!!!", "WHO", "WHERE", "WHEN", "OR SO YOU SAY" };

            TronicSequence ts = TronicSequence.StartFromButton(Cell.blocks);
            ts.GetFirst().position.Z = 100;
            ts.GetFirst().position.Y = -1800;

            List<Tuple<N8Block, DataBlock, DataBlock>> BlockAndData = new List<Tuple<N8Block, DataBlock, DataBlock>>(blocks.Count);
            

            for(int i = 0; i < blocks.Count; i++)
            {
                string color = colors[i%colors.Length];
                //string color = "black";
                Tuple<Vector3D, Quaternion> PosAndRot = blocks[i];
                N8Block CurrentBlock = Cell.blocks.GenerateBlock("simple." + color + ".land.mega", "temp");
                CurrentBlock.name = CurrentBlock.ID + "";//names[rand.Next(names.Length)];
                CurrentBlock.position = PosAndRot.Item1;
                CurrentBlock.rotation = PosAndRot.Item2;

                DataBlock MoverData = ts.NewDataBlock();
                DataBlock RotorData = ts.NewDataBlock();
                RotorData.position.Z = -35;
                RotorData.position.Y = 50;

                MoverData.position.Z = -35;
                MoverData.position.Y = -50;
                RotorData.name = RotorData.ID + "";
                MoverData.name = MoverData.ID + "";

                ts.Rotor(RotorData.In, RotorData.Out);
                Rotor r = (Rotor)ts.GetCurrent().Item1;
                r.position.Z = -35;
                r.position.X = 50;
                r.name = r.ID + "";

                

                ts.Mover(MoverData.In, MoverData.Out);
                Mover m = (Mover)ts.GetCurrent().Item1;
                m.position.Z = -35;
                m.position.X = -50;
                m.name = m.ID + "";

                CurrentBlock.AttachToMe(r);
                CurrentBlock.AttachToMe(m);
                CurrentBlock.AttachToMe(MoverData);
                CurrentBlock.AttachToMe(RotorData);

                BlockAndData.Add(Tuple.Create(CurrentBlock, MoverData, RotorData));
            }

            int RotationStepDegrees = 30;
            int NumRotationSteps = 360/RotationStepDegrees;

            int BlocksPerStep = BlockAndData.Count / NumRotationSteps;
            Spherical Rotation = new Spherical(0, 125, 0);
            Quaternion InitialRotation = new Quaternion(new Vector3D(0, 1, 0), 90);
            for (int i = 0; i < NumRotationSteps; i++)
            {
                Rotation.Phi = i * RotationStepDegrees * Utilities.DegToRad;
                var Line = Utilities.GenerateLine(Center, Rotation, BlocksPerStep, 400);
                for (int j = 0; j < BlocksPerStep; j++)
                {
                    int index = i * BlocksPerStep + j;
                    BlockAndData[index].Item1.name = "Line: " + i + " Block: " + j;
                    Vector3D RealVector = new Vector3D();
                    RealVector.X = Math.Round(Line.Item1[j].X + j);
                    RealVector.Y = Math.Round(Line.Item1[j].Y + j);
                    RealVector.Z = Math.Round(Line.Item1[j].Z);
                    BlockAndData[index].Item2.data = RealVector.ToData();
                    BlockAndData[index].Item3.data = (InitialRotation * Rotation.GetNormalRotation()).ToData();
                }
            }

            Utilities.MergeWithDefault(Cell);
            Utilities.Save(SavePath, Cell);
            
        }
    }
}
