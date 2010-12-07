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
            N8Level Planetarium = new N8Level(@"C:\Program Files (x86)\N8\Saves\stars_mega.ncd");
            MinorModifiers.TranslateLevel(Planetarium, new Vector3D(-200, 0, 0));
            MinorModifiers.OrderLoadingZ(Planetarium, false);
            Utilities.Save(@"C:\Program Files (x86)\N8\Saves\moved_stars_mega.ncd", Planetarium);
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