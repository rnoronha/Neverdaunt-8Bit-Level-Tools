using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.IO;

namespace N8Parser.Level_Modifiers
{
    static class AddMoon
    {
        public static void GenerateLevel(string LoadPath)
        {
            N8Level Level = new N8Level(LoadPath);

            N8Block ControlPoint = Level.blocks.GenerateBlock("letter.period", "Moon Control Point");
            ControlPoint.position.Z = 2000;
            //Load it as chibi, if available
            ControlPoint.Special = 2;
            Random rand = new Random();

            for (int i = 0; i < 50; i++)
            {
                N8Block moonblock = Level.blocks.GenerateBlock("simple.white.land.mega", "Moon");
                moonblock.position.Z = 1000;
                moonblock.rotation = rand.NextQuaternion();
                ControlPoint.AttachToMe(moonblock);
            }

            string SavePath = Path.GetDirectoryName(LoadPath) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(LoadPath) + "_moon.ncd";
            MinorModifiers.OrderLoading(Level, new Vector3D(0,0,1));
            Utilities.Save(SavePath, Level);
        }
    }
}
