using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.IO;

namespace N8Parser
{
    class RedMirror
    {
        public static void Mirror()
        {
            string filename = @"C:\Program Files (x86)\N8\Saves\ctf_blu_wallsonly_wip.ncd";

            N8Level level = new N8Level(filename);

            List<N8Block> blue_blocks = (from b in level.blocks.Blocks where b.type.Contains("blue") select b).ToList<N8Block>();

            foreach (N8Block b in blue_blocks)
            {
                Console.WriteLine("Looking at " + b);
                N8Block red_clone = level.blocks.CloneBlock(b);
                Console.WriteLine("Clone is: " + red_clone);

                //necessary to change the type
                red_clone.type = red_clone.type.Replace("blue", "red");

                //Not necessary, cosmetic only.
                red_clone.name = red_clone.name.Replace("Blue", "Red");

                Console.WriteLine("Changed name, now it's: " + red_clone);

                red_clone.position = Utilities.ReflectPlane(red_clone.position, new Vector3D(1, 0, 0));

                Console.WriteLine("Reflected it, now it's: " + red_clone);
            }

            string result = level.GenerateSaveFile();
            Console.WriteLine(result);
            Console.Read();

            string outFilename = @"C:\Program Files (x86)\N8\Saves\out-test.ncd";

            using (StreamWriter sw = new StreamWriter(File.OpenWrite(outFilename)))
            {
                sw.Write(result);
            }
        }
    }
}
