using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N8Parser.Geometry;

namespace N8Parser.Level_Modifiers
{
    public static class DNA
    {

        public const string CYTOSINE_COLOR = "orange";
        public const string ADENINE_COLOR = "green";
        public const string THYMINE_COLOR = "purple";
        public const string GUANINE_COLOR = "blue";
        public const string ERRORA_COLOR = "black";
        public const string ERRORB_COLOR = "brown";

        public static N8Level GetLevel()
        {
            N8Level Level = new N8Level();

            Cylindrical c = new Cylindrical(500, 0, -1000);
            char[] bases = new char[] { 'a', 'c', 'g', 't' };
            Random rand = new Random();

            int max = Utilities.MAX_BLOCK_COUNT - 16;
            
            int BlocksPerRung = MakeRung(Level, c, bases[rand.Next(bases.Length)]);
            int NumRungs = max / BlocksPerRung;

            for (int i = 1; i < NumRungs; i++)
            {
                MakeRung(Level, c, bases[rand.Next(bases.Length)]);
            }

            Level = MinorModifiers.AddBorder(Level);

            return Level;
        }

        private static int MakeRung(N8Level Level, Cylindrical current, char type)
        {
            int blockcount = 0;
            int tsteps = 5;
            int ttotal = 30;
            int rsteps = 7;
            double tstep = (ttotal * Utilities.DegToRad) / tsteps;
            double hstep = 30;
            bool runged = false;
            for (int i = 0; i < tsteps; i++)
            {
                string backbonecolor = "white";
                if ((i >= tsteps / 2) && !runged)
                {
                    backbonecolor = "black";
                }

                current.Theta += tstep;
                current.H += hstep;
                N8Block Block1 = Level.blocks.GenerateBlock("megapixel" + backbonecolor, "Phosphate Deoxyribose Backbone");
                blockcount++;
                N8Block Block2 = Level.blocks.GenerateBlock("megapixel" + backbonecolor, "Phosphate Deoxyribose Backbone");
                blockcount++;

                Block1.position = current.ToCartesian();
                Block1.rotation = current.GetNormalRotation();
                current.R = -current.R;

                Block2.position = current.ToCartesian();
                Block2.rotation = current.GetNormalRotation();
                current.R = -current.R;

                //Halfway up the ladder, do the rung
                if ((i >= tsteps / 2) && !runged)
                {
                    //T and C are slightly shorter than A and G, so we need to account for that.
                    //colorA is the longer color, colorB is the shorter one; if direction is negative we flip which direction we go.
                    string colorA;
                    string colorB;
                    string nameA;
                    string nameB;
                    int direction = 1;

                    switch (type)
                    {
                        case ('a'):
                            colorA = ADENINE_COLOR;
                            colorB = THYMINE_COLOR;
                            nameA = "Adenine";
                            nameB = "Thymine";
                            break;
                            //Why can't you just let me fall through?
                        case ('t'):
                            colorA = ADENINE_COLOR;
                            colorB = THYMINE_COLOR;
                            nameA = "Adenine";
                            nameB = "Thymine";
                            direction = -1;
                            break;
                        case ('g'):
                            colorA = GUANINE_COLOR;
                            colorB = CYTOSINE_COLOR;
                            nameA = "Guanine";
                            nameB = "Cytosine";
                            break;
                        case ('c'):
                            colorA = GUANINE_COLOR;
                            colorB = CYTOSINE_COLOR;
                            nameA = "Guanine";
                            nameB = "Cytosine";
                            direction = -1;
                            break;
                        default:
                            colorA = ERRORA_COLOR;
                            colorB = ERRORB_COLOR;
                            nameA = "Errornine";
                            nameB = "Errorsine";
                            break;
                    }
                    double initR = current.R;
                    current.R *= direction;

                    double rstep = (2 * current.R) / (double)rsteps;

                    int j;

                    for (j = 0; j < (rsteps / 2) - 1; j++)
                    {
                        N8Block shortblock = Level.blocks.GenerateBlock("pixel" + colorB, nameB);
                        blockcount++;
                        current.R -= rstep;
                        shortblock.position = current.ToCartesian();
                        shortblock.rotation = current.GetNormalRotation();
                    }

                    for (; j < rsteps-1; j++)
                    {
                        N8Block longblock = Level.blocks.GenerateBlock("pixel" + colorA, nameA);
                        blockcount++;
                        current.R -= rstep;
                        longblock.position = current.ToCartesian();
                        longblock.rotation = current.GetNormalRotation();
                    }

                    current.R = initR;
                    runged = true;
                }

            }

            return blockcount;
        }

        public static void GenerateLevel()
        {
            string SavePath = @"C:\Program Files (x86)\N8\Saves\dna.ncd";
            Utilities.Save(SavePath, GetLevel());
        }
    }
}
