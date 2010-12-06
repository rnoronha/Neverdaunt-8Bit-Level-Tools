using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using N8Parser.Level_Modifiers;
using System.Windows.Media.Media3D;

namespace N8Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            N8Level N8Star = new N8Level(@"C:\Program Files (x86)\N8\Saves\n8star.ncd");

            //N8Star = MinorModifiers.TranslateLevel(N8Star, new Vector3D(0, -95, 0));
            N8Block CP = N8Star.blocks.GenerateBlock("pixelgreen", "Control Point");
            CP.position = new Vector3D(0,0,-200);

            N8Star = MinorModifiers.AttachLevel(N8Star, CP);
            N8Star = MinorModifiers.OrderLoadingY(N8Star, true);

            Utilities.Save(@"C:\Program Files (x86)\N8\Saves\moved_n8star.ncd", N8Star);
            */

            EliteCellGenerator.GenerateLevel();


            //DeIsland.GenerateLevel(@"C:\Program Files (x86)\N8\Saves\mosquito.ncd");
            //GeometriesTest.GenerateLevel();
            //MaxProtectTest.GenerateProxyBubble();
            //MaxProtectTest.GenerateProxyBimesh();
            //MaxProtectTest.GenerateLevel();
            //Flies.GenerateLevel();
            //Eyestrain.GenerateLevel();
            //SCIENCE.GenerateLevel();
            //RotateSave.GenerateLevel(@"C:\Program Files (x86)\N8\Saves\arrow+y.ncd");

            //TronicsTesting.GenerateLevel();

            //TileFloor.GenerateLevel();

            //AddMoon.GenerateLevel();

        }
    }
}