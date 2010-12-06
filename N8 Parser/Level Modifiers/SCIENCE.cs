using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using N8Parser.Tronics;

namespace N8Parser.Level_Modifiers
{
    class SCIENCE
    {
        public static void GenerateLevel()
        {
            string SavePath = @"C:\Program Files (x86)\N8\Saves\science.ncd";
            N8Level Level = new N8Level();


            TronicSequence ts = TronicSequence.StartFromButton(Level.blocks);
            ts.GetFirst().position.Z = 300;

            int counter = 0;
            for (int i = -4; i < 4; i++)
            {
                for (int j = -4; j < 4; j++)
                {
                    counter++;
                    int x = i * 250;
                    int y = j * 250;
                    N8Block pixel = Level.blocks.GenerateBlock("White QuadPixel", "" + counter);

                    DataBlock MoverData = ts.NewDataBlock("Mover Data", "v" + x + ",1000," + y);
                    DataBlock RotorData = ts.NewDataBlock("Rotor Data", "q0.7071069,0,-0.7071065,0");
                    RotorData.position.Z = 35;
                    MoverData.position.Z = 35;
                    
                    ts.Rotor(RotorData.In, RotorData.Out, "" + counter);
                    Rotor r = (Rotor)ts.GetCurrent().Item1;
                    r.position.Z = 35;

                    ts.Mover(MoverData.In, MoverData.Out, "" + counter);
                    Mover m = (Mover)ts.GetCurrent().Item1;
                    m.position.Z = 35;

                    
                    
                    pixel.position.X = x;
                    pixel.position.Y = y;

                    pixel.AttachToMe(r);
                    pixel.AttachToMe(m);
                    pixel.AttachToMe(MoverData);
                    pixel.AttachToMe(RotorData);
                }
            }

            Utilities.Save(SavePath, Level);
            
        }
    }
}
