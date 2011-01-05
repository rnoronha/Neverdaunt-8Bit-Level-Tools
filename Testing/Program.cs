using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N8Parser.Level_Modifiers;
using N8Parser;
using System.Windows.Media.Media3D;
using N8Parser.Geometry;
using N8Parser.Tronics;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            //Chibifier.ChibifyLevel(@"C:\Program Files (x86)\N8\Saves\n8star.ncd");

            //AddMoon.GenerateLevel(@"C:\Program Files (x86)\N8\Saves\customs.ncd");
            //EliteCellGenerator.GenerateLevel();


            //DeIsland.GenerateLevel(@"C:\Program Files (x86)\N8\Saves\mosquito.ncd");
            //GeometriesTest.GenerateLevel();
            //MaxProtectTest.GenerateProxyBubble();
            //MaxProtectTest.GenerateProxyBimesh();
            //MaxProtectTest.GenerateLevel();
            //Flies.GenerateLevel();
            //Eyestrain.GenerateLevel();
            //Crazy.GenerateLevel();
            //SCIENCE.GenerateLevel();
            //RotateSave.GenerateLevel(@"C:\Program Files (x86)\N8\Saves\arrow+y.ncd");

            //TronicsTesting.GenerateLevel();

            //TileFloor.GenerateLevel();

            //AddMoon.GenerateLevel();

            
            /*N8Level RotateLevel1 = new N8Level(@"C:\Program Files (x86)\N8\Saves\arrow.ncd");
            N8Level RotateLevel2 = new N8Level(@"C:\Program Files (x86)\N8\Saves\arrow.ncd");
            N8Level RotateLevel3 = new N8Level(@"C:\Program Files (x86)\N8\Saves\arrow.ncd");
            N8Level OriginalLevel = new N8Level(@"C:\Program Files (x86)\N8\Saves\arrow.ncd");
            //MinorModifiers.RotateLevel(RotateLevel1, 180, new Vector3D(1, 0, 1));
            //MinorModifiers.RotateLevel(RotateLevel2, 720, new Vector3D(0, 1, 1));
            MinorModifiers.RotateLevel(RotateLevel3, -360, new Vector3D(1, 1, 0));
            MinorModifiers.TranslateLevel(OriginalLevel, new Vector3D(0, 0, -1000));
            OriginalLevel.MergeWithDestructive(RotateLevel1);
            OriginalLevel.MergeWithDestructive(RotateLevel2);
            OriginalLevel.MergeWithDestructive(RotateLevel3);
            Utilities.Save(@"C:\Program Files (x86)\N8\Saves\arrow_r.ncd", OriginalLevel);
             */

            DNA.GenerateLevel();
        }
    }
}
