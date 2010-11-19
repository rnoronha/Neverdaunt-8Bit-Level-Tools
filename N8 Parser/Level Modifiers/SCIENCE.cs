using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace N8Parser.Level_Modifiers
{
    class SCIENCE
    {
        public static void GenerateLevel()
        {
            string SavePath = @"C:\Program Files (x86)\N8\Saves\science.ncd";
            N8Level level = new N8Level(@"C:\Program Files (x86)\N8\Saves\default.ncd");
            var center = new Vector3D(1000, 1000, 70);
            int BlockCounter = 0;
            List<N8Block> MarkerXs = new List<N8Block>();
            List<N8Block> MarkerYs = new List<N8Block>();
            List<N8Block> MarkerZs = new List<N8Block>();
            for (int i = 0; i < 1000; i+= 5)
            {
                if (BlockCounter + 25 > 324)
                {
                    break;
                }
                BlockCounter += 25;

                for (int j = 0; j < 5; j++)
                {
                    for (int k = 0; k < 5; k++)
                    {
                        N8Block marker = level.blocks.GenerateBlock("snowmancoal", "Depth=" + i);
                        marker.position.X = j*5;
                        marker.position.Y = k*5;
                        marker.position.Z = i*2;
                        marker.position += center;

                    }
                }
            }

            /*
            var LastMarkerX = MarkerXs[MarkerXs.Count - 1];
            MarkerXs.Remove(LastMarkerX);
            LastMarkerX.position.X = -LastMarkerX.position.X + center.X;
            LastMarkerX.position.Y = center.Y;
            LastMarkerX.position.Z = center.Z;

            var LastMarkerY = MarkerYs[MarkerYs.Count - 1];
            MarkerYs.Remove(LastMarkerY);
            LastMarkerY.position.X = center.X;
            LastMarkerY.position.Y = -LastMarkerY.position.Y + center.Y;
            LastMarkerY.position.Z = center.Z;

            var LastMarkerZ = MarkerZs[MarkerZs.Count - 1];
            MarkerZs.Remove(LastMarkerZ);
            LastMarkerZ.position.X = center.X;
            LastMarkerZ.position.Y = center.Y;
            LastMarkerZ.position.Z = -LastMarkerZ.position.Z + center.Z;

            foreach (var b in MarkerXs)
            {
                LastMarkerX.AttachToMe(b);
            }

            foreach (var b in MarkerYs)
            {
                LastMarkerY.AttachToMe(b);
            }

            foreach (var b in MarkerZs)
            {
                LastMarkerZ.AttachToMe(b);
            }*/
            

            Utilities.Save(SavePath, level);
        }
    }
}
