using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using N8Parser.Tronics;

namespace N8Parser.Level_Modifiers
{
    static class TronicsTesting
    {

        public static void GenerateLevel()
        {
            string SavePath = @"C:\Program Files (x86)\N8\Saves\tronictest.ncd";
            string DefaultPath = @"C:\Program Files (x86)\N8\Saves\default.ncd";
            N8Level level = new N8Level(DefaultPath);

            TronicSequence ts = TronicSequence.StartFromButton();
            DataBlock Comma = ts.NewDataBlock("Comma");
            Comma.data = ",";
            DataBlock v = ts.NewDataBlock("V");
            v.data = "v";

            DataBlock OZ = ts.NewDataBlock("Z Offset", "1000");
            DataBlock OY = ts.NewDataBlock("Y Offset", "1000");
            DataBlock InitPos = ts.NewDataBlock("Initial Position", "v100,100,100");
            DataBlock DistanceOut = ts.NewDataBlock("Distance From Init");
            DataBlock ResetTimeout = ts.NewDataBlock("Reset Timeout", "10");

            DataBlock XYMin = ts.NewDataBlock("XY Min");
            XYMin.data = "-1000";

            DataBlock XYZMax = ts.NewDataBlock("XYZ Max");
            XYZMax.data = "1000";

            DataBlock ZMin = ts.NewDataBlock("Z Min");
            ZMin.data = "-1000";

            DataBlock PartialsA = ts.NewDataBlock("Partial Results A");
            DataBlock PartialsB = ts.NewDataBlock("Partial Results B");

            ts.Rand(XYMin.In, XYZMax.In, PartialsA.Out, "X Generator")
              .And(v.In, PartialsA.In, PartialsA.Out, "vX concat")
              .And(PartialsA.In, Comma.In, PartialsB.Out, "vX, concat")
              .Rand(ZMin.In, XYZMax.In, PartialsA.Out, "Z Generator")
              .Plus(PartialsA.In, OZ.In, PartialsA.Out, "Z Offset Adder")
              .And(PartialsB.In, PartialsA.In, PartialsB.Out, "vX,Z concat")
              .And(PartialsB.In, Comma.In, PartialsB.Out, "vX,Z, concat")
              .Rand(XYMin.In, XYZMax.In, PartialsA.Out, "Y Generator")
              .Subtract(PartialsA.In, OY.In, PartialsA.Out, "Y Offset Adder")
              .And(PartialsB.In, PartialsA.In, PartialsB.Out, "vX,Z,Y concat")
              .Display(PartialsB.In, "Final Result")
              .Mover(PartialsB.In, null, "Physical Location")
              .Distance(InitPos.In, PartialsB.In, DistanceOut.Out, "Distance Calculation")
              .Display(DistanceOut.In, "Distance Display")
              .Delay(ResetTimeout.In, "Reset timer")
              .Plus(InitPos.In, null, PartialsB.Out, "Reset swapper")
              .Mover(PartialsB.In, null, "Reset Mover");


            ts.LayoutLinear(new Vector3D(0, 0, 100));
            level.blocks.CopyFromDestructive(ts.tronics);
            Utilities.Save(SavePath, level);
            
        }

        public static TronicSequence RandomVectorGenerator()
        {
            TronicSequence ts = new TronicSequence();
            DataBlock Comma = ts.NewDataBlock("Comma");
            Comma.data = ",";
            DataBlock v = ts.NewDataBlock("V");
            v.data = "v";

            DataBlock XYMin = ts.NewDataBlock("XY Min");
            XYMin.data = "-2000";

            DataBlock XYZMax = ts.NewDataBlock("XYZ Max");
            XYZMax.data = "2000";

            DataBlock ZMin = ts.NewDataBlock("Z Min");
            ZMin.data = "-1000";

            DataBlock PartialsA = ts.NewDataBlock("Partial Results A");
            DataBlock PartialsB = ts.NewDataBlock("Partial Results B");

            ts.Rand(XYMin.In, XYZMax.In, PartialsA.Out, "X Generator")
              .And(v.In, PartialsA.In, PartialsA.Out, "vX concat")
              .And(PartialsA.In, Comma.In, PartialsB.Out, "vX, concat")
              .Rand(ZMin.In, XYZMax.In, PartialsA.Out, "Z Generator")
              .And(PartialsB.In, PartialsA.In, PartialsB.Out, "vX,Z concat")
              .And(PartialsB.In, Comma.In, PartialsB.Out, "vX,Z, concat")
              .Rand(XYMin.In, XYZMax.In, PartialsA.Out, "Y Generator")
              .And(PartialsB.In, PartialsA.In, PartialsB.Out, "vX,Z,Y concat");

            return ts;
        }

        public static TronicSequence RandomXYVectorGenerator()
        {
            TronicSequence ts = new TronicSequence();

            DataBlock v = ts.NewDataBlock("V");
            v.data = "v";

            DataBlock XYMin = ts.NewDataBlock("XY Min");
            XYMin.data = "-2000";

            DataBlock XYMax = ts.NewDataBlock("XYZ Max");
            XYMax.data = "2000";

            DataBlock Z = ts.NewDataBlock("Z Value", ",-1000,");

            DataBlock PartialsA = ts.NewDataBlock("Partial Results A");
            DataBlock PartialsB = ts.NewDataBlock("Partial Results B");

            ts.Rand(XYMin.In, XYMax.In, PartialsA.Out, "X Generator")
              .And(v.In, PartialsA.In, PartialsA.Out, "v X concat")
              .And(PartialsA.In, Z.In, PartialsB.Out, "vX ,Z, concat")
              .Rand(XYMin.In, XYMax.In, PartialsA.Out, "Y Generator")
              .And(PartialsB.In, PartialsA.In, PartialsB.Out, "vX,Z, Y concat");

            return ts;
        }

        public static TronicSequence Ringbuffer(List<string> values)
        {
            TronicSequence ts = new TronicSequence();
            DataBlock current = ts.NewDataBlock("0", values[0]);
            DataBlock next = ts.NewDataBlock("1", values[0]);
            ts.And(next.In, null, current.Out, "0");
            for (int i = 1; i < values.Count; i++)
            {
                current = next;
                next = ts.NewDataBlock(i+1 + "", values[i]);
                ts.And(next.In, null, current.Out, i + "");
            }
            ts.And(ts.data[0].In, null, next.Out, values.Count + "");

            return ts;
        }

        public static Tuple<TronicSequence, List<And>> FlowBank(int MaxFlowStorage, int MinDelay, int MaxDelay)
        {
            TronicSequence ts = new TronicSequence();
            List<And> stubs = new List<And>(MaxFlowStorage);

            DataBlock Zero = ts.NewDataBlock("Zero", "0");
            DataBlock One = ts.NewDataBlock("One", "1");

            int DelayDelta = MinDelay - MaxDelay/ MaxFlowStorage;
            if (DelayDelta < 1)
            {
                DelayDelta = 1;
            }
            
            
            for (int i = 0; i < MaxFlowStorage; i++)
            {
                DataBlock Timespan = ts.NewDataBlock("Timespan", MinDelay + i * DelayDelta + "");
                TronicSequence Else = new TronicSequence();
                DataBlock ElseGate = Else.NewDataBlock("Gate " + i, "1");
                Else.And(Zero.In, null, ElseGate.Out)
                    .Delay(Timespan.In)
                    .And(One.In, null, ElseGate.Out, "Flow Bank Stub");
                stubs.Add((And)Else.GetCurrent().Item1);

                ts.IfLTE(ElseGate.In, null, "Choice " + i, Else);
            }

            return Tuple.Create(ts, stubs);
        }
    }
}
