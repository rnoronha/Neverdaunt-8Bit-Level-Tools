using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace N8Parser.Animation
{
    public class AnimationFrame
    {
        private SortedList<int, Tuple<Vector3D, Quaternion>> _blocks;
        public SortedList<int, Tuple<Vector3D, Quaternion>> Blocks
        {
            get
            {
                return _blocks;
            }
        }


        public AnimationFrame(string data)
        {
            _blocks = ParseData(data);
        }

        public AnimationFrame(SortedList<int, Tuple<Vector3D, Quaternion>> blocks)
        {
            _blocks = blocks;
        }

        public static SortedList<int, Tuple<Vector3D, Quaternion>> ParseData(string data)
        {
            SortedList<int, Tuple<Vector3D, Quaternion>> blocks = new SortedList<int, Tuple<Vector3D, Quaternion>>();

            if (data.Length == 0)
            {
                //This is empty, so just leave it empty
                return blocks;
            }

            //Aion's using a shitload of different separators :(
            //Individual block position entries are separated by backslashes
            string[] entries = data.Split('\\');

            //And then inside those entries, data is separated by slashes
            foreach(string entry in entries)
            {
                string[] pieces = entry.Split('/');
                if (pieces.Length != 3)
                {
                    //Invalid thingy, what do?
                    throw new FormatException("Improper format for animation data, expected 3 elements found " + pieces.Length + ". Make sure there's at most three backslashes (\\) in it!");
                }
                //And then it's pretty simple: <ID>, position, rotation
                int id = int.Parse(pieces[0].Trim('<', '>'));
                Vector3D position = Utilities.VectorFromN8String(pieces[1]);
                Quaternion rotation = Utilities.RotationFromN8String(pieces[2]);

                blocks.Add(id, Tuple.Create(position, rotation));
            }

            //And keep it ordered by block ID for later

            return blocks;
        }

        /// <summary>
        /// Figures out what the differences between this animation frame and the given one are,
        /// and returns those differences.
        /// To visualize it, "this" frame is now, "other" frame is the next time step. 
        /// Thus, any elements in "this" that's not in "other" can be ignored, elements in "other" not in "this" are required,
        /// and elements in both are required if they change from "this" to "other".
        /// </summary>
        /// <param name="other">The other animation frame to look at</param>
        /// <returns>The differences between this and other</returns>
        public AnimationFrame delta(AnimationFrame other, double RotationEpsilon = 0.001, double PositionEpsilon = 0.5)
        {
            SortedList<int, Tuple<Vector3D, Quaternion>> ret = new SortedList<int, Tuple<Vector3D, Quaternion>>();
            foreach (var kv in other.Blocks)
            {
                if (this.Blocks.ContainsKey(kv.Key))
                {
                    Vector3D posdiff = (this.Blocks[kv.Key].Item1 - kv.Value.Item1).Abs();
                    //I'm not sure if this will work - I don't know if they implement special
                    //quaternion-specific subtraction routines.
                    Quaternion rotdiff = (this.Blocks[kv.Key].Item2 - kv.Value.Item2).Abs();

                    if (posdiff.X > PositionEpsilon || posdiff.Y > PositionEpsilon || posdiff.Z > PositionEpsilon)
                    {
                        ret.Add(kv.Key, kv.Value);
                    }
                    else if (rotdiff.W > RotationEpsilon || rotdiff.X > RotationEpsilon || rotdiff.Y > RotationEpsilon || rotdiff.Z > RotationEpsilon)
                    {
                        ret.Add(kv.Key, kv.Value);
                    }

                }
                else
                {
                    ret.Add(kv.Key, kv.Value);
                }
            }

            return new AnimationFrame(ret);
        }

        public string ToData()
        {

            StringBuilder sb = new StringBuilder();
            foreach(var kv in Blocks)
            {
                //Be sure to trim the Q and V off of the beginning, because we want them in the weird N8 order format but not with their data types :(
                sb.Append("<" + kv.Key + ">/" + kv.Value.Item1.ToData().TrimStart('v') + "/" + kv.Value.Item2.ToData().TrimStart('q') + "\\");
            }

            return sb.ToString();
        }
    }
}
