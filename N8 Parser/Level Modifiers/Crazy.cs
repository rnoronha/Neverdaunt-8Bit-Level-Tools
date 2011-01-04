using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace N8Parser.Level_Modifiers
{
    public class Crazy
    {
        public static N8Level GetLevel()
        {
            N8Level Level = MaxProtectTest.GetProxyBubble();
            Random rand = new Random();
            string[] colors = { "blue", "green", "orange", "purple", "red", "black", "white"};
            //string[] colors = {"black" };

            string[] names = { "OH MY GOD", "IT BURNS", "WHY GOD WHY", "DEAR LORD WHAT IS THIS", 
                               "DEATH WOULD BE BETTER", "WHERE IS MY EYE-SPOON", "GOOD FUCKING GRIEF", "WHHHHHYYYYYY",
                               "MOBUNGA EAT YOUR HEART OUT", "SOMEONE KILL IT QUICKLY", "OMFG SIEZURE", "lolwut",
                               "what is this I don't even"};
            int NumBlocks = 349 - (Level.blocks.Tronics.Count + Level.blocks.Blocks.Count);
            Console.WriteLine(NumBlocks);
            Console.ReadLine();
            for (int i = 0; i < NumBlocks; i++)
            {
                string color = colors[i % colors.Length];
                //string color = "black";
                N8Block CurrentBlock = Level.blocks.GenerateBlock("simple." + color + ".land.mega", names[rand.Next(names.Length)]);
                CurrentBlock.position = rand.NextVector(new Vector3D(-2000, -2000, -1000), new Vector3D(2000, 2000, -230));
                CurrentBlock.rotation = rand.NextQuaternion();
            }
            MinorModifiers.OrderLoading(Level, new Vector3D(1,2,0));
            return Level;
        }

        public static void GenerateLevel()
        {
            string SavePath = @"C:\Program Files (x86)\N8\Saves\crazy.ncd";
            Utilities.Save(SavePath, GetLevel());
        }
    }
}
