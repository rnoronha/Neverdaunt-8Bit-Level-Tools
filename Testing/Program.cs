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
            N8Level Level = new N8Level();
            MinorModifiers.AddCrossroads(Level);
            Cylindrical X = new Cylindrical(0, 0, 0, new Vector3D(1, 0, 0));
            Cylindrical Y = new Cylindrical(0, 0, 0, new Vector3D(0, 1, 0));
            Cylindrical Z = new Cylindrical(0, 0, 0, new Vector3D(0, 0, 1));
            //Freaking MSFT - why couldn't they make this return a reference to the vector?
            Vector3D XY_axis = new Vector3D(1, 1, 0);
            XY_axis.Normalize();
            Cylindrical XY = new Cylindrical(0, 0, 0, new Vector3D(1, 1, 0));
            Vector3D XZ_axis = new Vector3D(1, 1, 0);
            XZ_axis.Normalize();
            Cylindrical XZ = new Cylindrical(0, 0, 0, new Vector3D(1, 0, 1));

            int BlockCount = 349 - (Level.blocks.Blocks.Count + Level.blocks.Tronics.Count);
            Cylindrical[] circles = { X, Y, Z, XZ, XY };
            string[] colors = {"red", "blue", "green", "white", "black"};

            int steps = circles.Length / BlockCount;

            for(int j = 0; j < circles.Length; j++)
            {
                Cylindrical c = circles[j];
                string color = colors[j];
                c.R = 300;
                c.H = 0;
                double ThetaStep = 2 * Math.PI / steps;
                for (int i = 0; i < steps; i++)
                {
                    N8Block b = Level.blocks.GenerateBlock("pixel" + color, c.Axis.ToString());
                    b.position = c.ToCartesian();
                    b.rotation = c.GetNormalRotation();
                    c.Theta += ThetaStep;
                }
            }

            Utilities.Save(Utilities.GetDefaultSaveFolder() + "circles.ncd", Level);

            //Crazy.GenerateBlockOctopus();
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
