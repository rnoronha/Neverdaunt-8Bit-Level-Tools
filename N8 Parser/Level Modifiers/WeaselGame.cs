using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N8Parser.Tronics;
using System.Windows.Media.Media3D;

namespace N8Parser.Level_Modifiers
{
    public static class WeaselGame
    {

        public static void GenerateLevel()
        {
            Utilities.Save(Utilities.GetDefaultSaveFolder() + "weaselgame", GetLevel());
        }
        public static N8Level GetLevel()
        {
            N8Level ret = new N8Level(Utilities.GetDefaultSaveFolder() + "default");

            List<N8Block> bases = new List<N8Block>();
            List<Rotor> resets = new List<Rotor>();
            List<Rotor> falls = new List<Rotor>();

            DataBlock ResetData = ret.blocks.DataBlock("Reset", new Quaternion(new Vector3D(0,0,1), -90).ToData());
            ResetData.position = new Vector3D(10, 0, 100);
            DataBlock FallData = ret.blocks.DataBlock("Fall", (new Quaternion(new Vector3D(0, 1, 0), 90) * new Quaternion(new Vector3D(0, 0, 1), -90)).ToData());
            FallData.position = new Vector3D(10, 0, 100);

            Add Plus = ret.blocks.Plus();
            Plus.position.X = 1000;
            Plus.position.Y = -100;
            Plus.position.Z = 70;

            for (int i = 0; i < 25; i++)
            {
                var Unit = GenerateUnit(ret.blocks, "street.sign.x", "Hit me!");
                bases.Add(Unit.Item3);
                resets.Add(Unit.Item1);
                falls.Add(Unit.Item2);
                falls.Last().FlowOutTo(Plus);
                falls.Last().DataInA(FallData.In);
                resets.Last().DataInA(ResetData.In);

            }

            Subtract Minus = ret.blocks.Subtract();
            Minus.position.X = 1000;
            Minus.position.Y = 100;
            Minus.position.Z = 70;

            for (int i = 0; i < 25; i++)
            {
                var Unit = GenerateUnit(ret.blocks, "street.sign.heart", "Don't hit me!");
                bases.Add(Unit.Item3);
                resets.Add(Unit.Item1);
                falls.Add(Unit.Item2);
                falls.Last().FlowOutTo(Minus);
                falls.Last().DataInA(FallData.In);
                resets.Last().DataInA(ResetData.In);
            }

            Divide Reset = ret.blocks.Divide();

            Reset.position.X = 1000;
            Reset.position.Z = 70;

            FlowTronic previous = Reset;
            foreach (Rotor r in resets)
            {
                previous.FlowOutTo(r);
                previous = r;
            }

            int x = -350;
            int y = -350;

            for (int i = 0; i < bases.Count; i++)
            {
                bases[i].position = new Vector3D(x, y, 60);
                x += 100;
                if (x >= 350)
                {
                    x = -350;
                    y += 100;
                }
            }
            


            return ret;
        }

        private static Tuple<Rotor, Rotor, N8Block> GenerateUnit(N8BlockFactory blocks, string blocktype, string blockname)
        {
            N8Block UnitBase = blocks.GenerateBlock(blocktype, blockname);
            Rotor reset = blocks.Rotor("Reset");
            Rotor fall = blocks.Rotor("Fall");
            Target t = blocks.Target("Hit me!");

            UnitBase.rotation = new Quaternion(new Vector3D(0, 0, 1), -90);
            

            t.FlowOutTo(fall);
            t.position.Z = 67;
            t.position.X = 2;
            t.rotation = new Quaternion(new Vector3D(0, 0, 1), -90) * new Quaternion(new Vector3D(0, 1, 0), 90);
            fall.position.X = 2;
            fall.position.Z = 5;

            fall.rotation = new Quaternion(new Vector3D(0, 1, 0), 90);

            UnitBase.AttachToMe(reset);
            UnitBase.AttachToMe(fall);
            UnitBase.AttachToMe(t);

            return Tuple.Create(reset, fall, UnitBase);
        }
    }
}
