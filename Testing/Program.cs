using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N8Parser.Level_Modifiers;
using N8Parser;
using System.Windows.Media.Media3D;
using N8Parser.Geometry;
using N8Parser.Tronics;
using System.Drawing;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            for (int i = 0; i < 4; i++)
            {
                N8Level stack = MaxProtectTest.GetProxyBubble();

                for (int k = -1000; k < 2000; k += 60)
                {
                    N8Block b = stack.blocks.GenerateBlock("megafloor", "stack");
                    b.position = new Vector3D(2000, 2000, k);
                }

                N8Block corner1 = stack.blocks.GenerateBlock("landmega", "Ring");
                corner1.position = new Vector3D(-1600, -1600, 0);
                corner1.rotation = new Quaternion(new Vector3D(0,0,1), 90);
                N8Block corner2 = stack.blocks.GenerateBlock("landmega", "Ring");
                corner2.position = new Vector3D(-800, -800, 0);
                corner2.rotation = new Quaternion(new Vector3D(0,0,1), 90);

                //Inner ring
                for (int l = 0; l <= 1600; l+= 800)
                {
                    N8Block ringX = stack.blocks.GenerateBlock("landmega", "Ring");
                    ringX.position = new Vector3D(l, -800, 0);
                    ringX.rotation = new Quaternion(new Vector3D(0, 0, 1), 90);

                    N8Block ringY = stack.blocks.GenerateBlock("landmega", "Ring");
                    ringY.position = new Vector3D(-800, l, 0);
                    ringY.rotation = new Quaternion(new Vector3D(0, 0, 1), 90);
                }

                //outer ring
                for (int l = -800; l <= 1600; l += 800)
                {
                    N8Block ringX = stack.blocks.GenerateBlock("landmega", "Ring");
                    ringX.position = new Vector3D(l, -1600, 0);
                    ringX.rotation = new Quaternion(new Vector3D(0, 0, 1), 90);

                    N8Block ringY = stack.blocks.GenerateBlock("landmega", "Ring");
                    ringY.position = new Vector3D(-1600, l, 0);
                    ringY.rotation = new Quaternion(new Vector3D(0, 0, 1), 90);
                }

                stack = MinorModifiers.RotateLevel(stack, i * 90, new Vector3D(0, 0, 1));


                Utilities.Save(Utilities.GetDefaultSaveFolder() + "machostack" + i + ".ncd", stack);
            }
            */
            


            Crazy.GenerateBlockOctopus();
            /*
            N8Level Level = new N8Level();
            MinorModifiers.AddCrossroads(Level);
            Cylindrical X = new Cylindrical(0, 0, 0, new Vectsor3D(1, 0, 0));
            Cylindrical Y = new Cylindrical(0, 0, 0, new Vector3D(0, 1, 0));
            Cylindrical Z = new Cylindrical(0, 0, 0, new Vector3D(0, 0, 1));
            //Freaking MSFT - why couldn't they make this return a reference to the vector?
            Vector3D XY_axis = new Vector3D(1, 1, 0);
            XY_axis.Normalize();
            Cylindrical XY = new Cylindrical(0, 0, 0, XY_axis);

            Vector3D YZ_axis = new Vector3D(0, 1, 1);
            YZ_axis.Normalize();
            Cylindrical YZ = new Cylindrical(0, 0, 0, YZ_axis);

            Vector3D XZ_axis = new Vector3D(1, 0, 1);
            XZ_axis.Normalize();
            Cylindrical XZ = new Cylindrical(0, 0, 0, XZ_axis);

            int BlockCount = 300 - (Level.blocks.Blocks.Count + Level.blocks.Tronics.Count);
            Cylindrical[] circles = { X, Y, Z, XZ, YZ, XY };
            string[] colors = {"red", "blue", "green", "white", "black", "brown"};

            int steps = BlockCount / circles.Length;

            for(int j = 0; j < circles.Length; j++)
            {
                Cylindrical c = circles[j];
                string color = colors[j];
                c.R = 500;
                c.H = 0;
                double ThetaStep = 2 * Math.PI / steps;
                for (int i = 0; i < steps; i++)
                {
                    N8Block b = Level.blocks.GenerateBlock("letter.period.big", i + "");//"pixel" + color, i + "");
                    b.position = c.ToCartesian();
                    b.rotation = c.GetNormalRotation();
                    c.Theta += ThetaStep;
                }
            }
            //MinorModifiers.OrderLoadingCylindrical(Level);
            Utilities.Save(Utilities.GetDefaultSaveFolder() + "circles.ncd", Level);
            //*/
            
            /*
            Heightmap seed = new Heightmap(32);
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    seed[i, j] = (30 - (Math.Abs(i-15) + Math.Abs(j-15))) * 10;
                }
            }

            Heightmap res = Terrain.GetHeightmap(5, 400, 0.2, seed);

            List<List<Heightmap>> maps = Terrain.ChopHeightmap(10, res);

            for (int k = 0; k < 3; k++)
            {
                for (int l = 0; l < 3; l++)
                {
                    N8Level Level = new N8Level();
                    for (int i = 1; i <= 10; i++)
                    {
                        for (int j = 1; j <= 10; j++)
                        {
                            N8Block land = Level.blocks.GenerateBlock("land", "Terrain");
                            land.position.X = (i - 5) * 400 + 200;
                            land.position.Y = (j - 5) * 400 + 200;
                            land.position.Z = maps[k][l][i,j];
                        }
                    }

                    Utilities.Save(Utilities.GetDefaultSaveFolder() + "terrain" + (k-1) + "," + (l-1) + ".ncd", Level);
                }
            }
             */
            /*
            Heightmap seed = new Heightmap(17);
            seed[5, 5] = 300;
            for(int i = -1; i <= 1; i += 2)
                for (int j = -1; j <= 1; j += 2)
                {
                    seed[5 + i, 5 + j] = 200;
                }

            Heightmap res = Terrain.GetHeightmap(4, 100, 0.5, seed);

            N8Level Level = new N8Level();
            for (int i = 1; i <= 10; i++)
            {
                for (int j = 1; j <= 10; j++)
                {
                    N8Block land = Level.blocks.GenerateBlock("land", "Terrain");
                    land.position.X = (i - 5) * 400 - 200;
                    land.position.Y = (j - 5) * 400 - 200;
                    land.position.Z = res[i, j];
                }
            }

            int min = (int)Level.blocks.Blocks.Min((b) => b.position.Z);
            MinorModifiers.TranslateLevel(Level, new Vector3D(0, 0, -min));

            Utilities.Save(Utilities.GetDefaultSaveFolder() + "terrain.ncd", Level);
             */
        }
        
    }
}
