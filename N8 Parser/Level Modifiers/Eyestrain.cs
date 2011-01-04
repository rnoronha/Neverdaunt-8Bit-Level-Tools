using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media.Media3D;

namespace N8Parser.Level_Modifiers
{
    public class Eyestrain
    {
        public static void GenerateLevel()
        {
            string SavePath = @"C:\Program Files (x86)\N8\Saves\eyestrain.ncd";
            
            string[] colors = { "blue", "green", "orange", "purple", "red", "black"};
            //string[] colors = {"black" };

            string[] names = { "OH MY GOD", "IT BURNS", "WHY GOD WHY", "DEAR LORD WHAT IS THIS", 
                               "DEATH WOULD BE BETTER", "WHERE IS MY EYE-SPOON", "GOOD FUCKING GRIEF", "WHHHHHYYYYYY",
                               "MOBUNGA EAT YOUR HEART OUT", "SOMEONE KILL IT QUICKLY", "OMFG SIEZURE" };
            N8Level Level = new N8Level();


            N8BlockFactory LevelBlocks = Level.blocks;

            Random rand = new Random();

            /*
            bool done = false;
            int blocksize = 100;
            int count = 0;
            while(!done)
            {
                var test = Utilities.EvenSphere(new Vector3D(0, 500, 1000), blocksize, 500, (double)3 * Math.PI/4);
                if (test.Count < 324)
                {
                    blocksize--;
                }
                if (test.Count > 324)
                {
                    count = test.Count;
                    blocksize++;
                    done = true;
                }
            }
            Console.WriteLine("Blocksize is: " + blocksize + ", count is: " + count);
            */

            for(int i = 0; i < 300; i++)
            {
                string color = colors[i%colors.Length];
                //string color = "black";
                N8Block CurrentBlock = LevelBlocks.GenerateBlock("simple." + color + ".land.mega", names[rand.Next(names.Length)]);
                CurrentBlock.position = rand.NextVector(new Vector3D(-2000, -2000, -1000), new Vector3D(2000, 2000, 0));
                CurrentBlock.rotation = rand.NextQuaternion();
            }


            Utilities.Save(SavePath, Level);
            Console.Read();

            
        }
    }
}
