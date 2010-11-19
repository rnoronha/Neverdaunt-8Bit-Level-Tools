using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media.Media3D;
using System.Text.RegularExpressions;

namespace N8Parser
{
    class Planetarium
    {
        public static void GenerateLevel(string StarsPath)
        {
            N8Level level = new N8Level();
            N8BlockFactory LevelBlocks = level.blocks;
            List<Vector3D> points = new List<Vector3D>();

            using (StreamReader sr = new StreamReader(File.OpenRead(StarsPath)))
            {
                string input;

                Regex SplitWhitespace = new Regex(@"\s+");
                while ((input = sr.ReadLine()) != null)
                {
                    string[] parts = SplitWhitespace.Split(input);
                    points.Add(new Vector3D(double.Parse(parts[3]), double.Parse(parts[4]), double.Parse(parts[5])));
                }
            }

            //Take only the first 300 stars
            points = points.Take(300).ToList();

            List<N8Block> stars;

            //Useful options:
            //snowmantop
            //letter.period
            //letter.period.big
            stars = Utilities.PlotPointsSphere(points, new Quaternion(0, 0, 0, 1), 1000, "snowmantop", "Star", level);

            N8Block ControlPoint = LevelBlocks.GenerateBlock("minipixelblack", "Control Point");

            foreach (N8Block star in stars)
            {
                ControlPoint.AttachToMe(star);
            }

            //Make a roof
            for (int i = -800; i <= 800; i += 400)
            {
                for (int j = -800; j <= 800; j += 400)
                {
                    N8Block roof = LevelBlocks.GenerateBlock("simplelandblack", "Vault of the Heavens");
                    roof.position = new Vector3D(i, j, 1000);
                }
            }

            string save = level.GenerateSaveFile();
            using(StreamWriter sw = new StreamWriter(File.Open(@"C:\Program Files (x86)\N8\Saves\planetarium.ncd", FileMode.Truncate)))
            {
                sw.WriteLine(save);
            }

            Console.WriteLine(save);
            Console.Read();
        }
    }
}
