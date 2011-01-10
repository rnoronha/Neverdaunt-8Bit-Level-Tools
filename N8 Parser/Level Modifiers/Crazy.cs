using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace N8Parser.Level_Modifiers
{
    public class Crazy
    {
        private static string[] colors = { "blue", "green", "orange", "purple", "red", "black", "white", "yellow" };
        //string[] colors = {"black" };

        private static string[] names = { "OH MY GOD", "IT BURNS", "WHY GOD WHY", "DEAR LORD WHAT IS THIS", 
                               "DEATH WOULD BE BETTER", "WHERE IS MY EYE-SPOON", "GOOD FUCKING GRIEF", "WHHHHHYYYYYY",
                               "MOBUNGA EAT YOUR HEART OUT", "SOMEONE KILL IT QUICKLY", "OMFG SIEZURE", "lolwut",
                               "what is this I don't even"};
        public static N8Level GetBlockOctopus()
        {
            N8Level Level = new N8Level();
            Random rand = new Random();
            int NumBlocks = 349;
            Vector3D diag1 = new Vector3D(1, 1, 0);
            diag1.Normalize();
            Vector3D diag2 = new Vector3D(1, -1, 0);
            diag2.Normalize();
            Vector3D plusX = new Vector3D(1, 0, 0);
            Vector3D plusY = new Vector3D(0, 1, 0);
            //Max distance for a diagonal is Sqrt(4000^2 + 4000^2) ~= 5656, and add an extra two just to make sure it reaches the edges
            int DiagMin = -5658 / 2;
            int DiagMax = 5658 / 2;
            int StraightMin = -2000;
            int StraightMax = 2000;
            for (int i = 0; i < NumBlocks; i++)
            {
                
                string color = colors[i % colors.Length];
                N8Block CurrentBlock = Level.blocks.GenerateBlock("simple." + color + ".block", names[rand.Next(names.Length)]);

                if (rand.Next(0, 2) == 0)
                {
                    int dmag = rand.Next(DiagMin, DiagMax);
                    if (rand.Next(0, 2) == 0)
                    {

                        CurrentBlock.position = diag1 * dmag;
                    }
                    else
                    {
                        CurrentBlock.position = diag2 * dmag;
                    }
                }
                else
                {
                    int smag = rand.Next(StraightMin, StraightMax);
                    if (rand.Next(0, 2) == 0)
                    {

                        CurrentBlock.position = plusX * smag;
                    }
                    else
                    {
                        CurrentBlock.position = plusY * smag;
                    }
                }

                CurrentBlock.rotation = rand.NextQuaternion();
            }

            //And now make it come up in the center - blocks closer to 0,0,0 get more of a boost.
            int MaxHeight = 300;
            foreach (N8Block b in Level.blocks.Blocks)
            {
                b.position.Z = Math.Round(Math.Min(100 / b.position.Length * MaxHeight, 600));
            }

            MinorModifiers.OrderLoadingCylindrical(Level);

            return Level;

        }

        public static void GenerateBlockOctopus()
        {
            Utilities.Save(Utilities.GetDefaultSaveFolder() + "blocktopus.ncd", GetBlockOctopus());
        }

        public static N8Level GetBlockRoad(bool diagonal = false)
        {
            N8Level Level = new N8Level();
            Random rand = new Random();
            int NumBlocks = 349;
            if (diagonal)
            {
                Vector3D diag1 = new Vector3D(1, 1, 0);
                diag1.Normalize();
                Vector3D diag2 = new Vector3D(1, -1, 0);
                diag2.Normalize();
                //Max distance for a diagonal is Sqrt(4000^2 + 4000^2) ~= 5656, and add an extra two just to make sure it reaches the edges
                int Min = -5658 / 2;
                int Max = 5658 / 2;
                for (int i = 0; i < NumBlocks; i++)
                {
                    int mag = rand.Next(Min, Max);
                    string color = colors[i % colors.Length];
                    N8Block CurrentBlock = Level.blocks.GenerateBlock("simple." + color + ".block", names[rand.Next(names.Length)]);

                    if (rand.Next(0, 2) == 0)
                    {
                        CurrentBlock.position = diag1 * mag;
                    }
                    else
                    {
                        CurrentBlock.position = diag2 * mag;
                    }

                    CurrentBlock.rotation = rand.NextQuaternion();
                }
            }
            else
            {
                for (int i = 0; i < NumBlocks; i++)
                {
                    string color = colors[i % colors.Length];
                    N8Block CurrentBlock = Level.blocks.GenerateBlock("simple." + color + ".block", names[rand.Next(names.Length)]);
                    //Either put them in the x-coord area or the y-coord area (yes this means the center will be better covered)
                    if (rand.Next(0, 2) == 0)
                    {
                        CurrentBlock.position = rand.NextVector(new Vector3D(-100, 2000, 0), new Vector3D(100, -2000, 0));
                    }
                    else
                    {
                        CurrentBlock.position = rand.NextVector(new Vector3D(2000, -100, 0), new Vector3D(-2000, 100, 0));
                    }
                    CurrentBlock.rotation = rand.NextQuaternion();
                }
            }

            MinorModifiers.OrderLoading(Level, new Vector3D(1, 1, 0));

            return Level;
        }

        public static void GenerateBlockRoad(bool diagonal = false)
        {
            Utilities.Save(Utilities.GetDefaultSaveFolder() + "blockroad.ncd", GetBlockRoad(diagonal));
        }

        public static N8Level GetCrossroads()
        {
            N8Level Level = new N8Level(); //MaxProtectTest.GetProxyBubble();
            MinorModifiers.AddCrossroads(Level);
            Random rand = new Random();
     
            int NumBlocks = 349 - (Level.blocks.Tronics.Count + Level.blocks.Blocks.Count);
            Console.WriteLine(NumBlocks);
            Console.ReadLine();
            for (int i = 0; i < NumBlocks; i++)
            {
                string color = colors[i % colors.Length];
                //string color = "black";
                N8Block CurrentBlock = Level.blocks.GenerateBlock("simple." + color + ".block", names[rand.Next(names.Length)]);
                //Keep it out of the crossroads
                CurrentBlock.position = rand.NextVector(new Vector3D(2000, 2000, -1000), new Vector3D(300, 300, 2000));

                //And flop it around the quadrants randomly
                if (rand.Next(0, 2) == 0)
                {
                    CurrentBlock.position.X *= -1;
                }
                if (rand.Next(0, 2) == 0)
                {
                    CurrentBlock.position.Y *= -1;
                }


                CurrentBlock.rotation = rand.NextQuaternion();
            }
            MinorModifiers.OrderLoadingCylindrical(Level);
            return Level;
        }

        public static void GenerateCrossroadsLevel()
        {
            string SavePath = @"C:\Program Files (x86)\N8\Saves\crossroads.ncd";
            Utilities.Save(SavePath, GetCrossroads());
        }
    }
}
