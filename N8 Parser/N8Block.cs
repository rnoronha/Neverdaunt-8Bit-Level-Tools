using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace N8Parser
{
    [Serializable]
    public class N8Block
    {
        private int _id;

        public int ID
        {
            get
            {
                return _id;
            }
            set
            {
                ChangeID(value);
            }
        }

        

        public string type;
        public string name;
        public Vector3D position;
        public Quaternion rotation;
        public N8Block AttachedTo;

        public List<N8Block> Attachees;

        public N8Block(string[] pieces)
        {
            AttachedTo = null;
 
            Attachees = new List<N8Block>();

            _id = int.Parse(pieces[0]);
            type = pieces[1];
            name = pieces[2];

            double[] vector_parts = (from s in pieces[3].Split(',') select double.Parse(s)).ToArray();
            position = new Vector3D(vector_parts[0], vector_parts[2], vector_parts[1]);

            double[] quat_parts = (from s in pieces[4].Split(',') select double.Parse(s)).ToArray();
            rotation = new Quaternion(quat_parts[1], quat_parts[3], quat_parts[2], quat_parts[0]);
        }

        public N8Block(int id, string type, string name)
        {
            AttachedTo = null;
            Attachees = new List<N8Block>();

            _id = id;
            this.type = type;
            this.name = name;
        }

        //Copy constructor. Does not copy attachments or ID.
        public N8Block(N8Block source, int ID)
        {
            AttachedTo = null;
            Attachees = new List<N8Block>();

            this._id = ID;

            type = source.type;
            name = source.name;

            position = source.position;
            rotation = source.rotation;
        }

        //Should only be used to generate the shrine
        public N8Block(int id)
        {
            AttachedTo = null;
            Attachees = new List<N8Block>();
             
            _id = id;
            this.type = "";
            this.name = "";
        }


        public virtual void ChangeID(int newID)
        {
            _id = newID;         
        }

        public override string ToString()
        {
            //Apparently the N8 server dislikes doubles, so cast all these suckers to floats before outputting
            string ret = _id + ":" + type + ":" + name + ":" + (float)position.X + "," + (float)position.Z + "," + (float)position.Y
                            + ":" + (float)rotation.W + "," + (float)rotation.X + "," + (float)rotation.Z + "," + (float)rotation.Y;
            return ret;
        }

        /// <summary>
        /// Attaches other to this block, making its position become relative to this block.
        /// </summary>
        /// <param name="other"></param>
        public void AttachToMe(N8Block other)
        {
            this.Attachees.Add(other);
            other.AttachedTo = this;
            //Coordinate systems get translated too - now other's coords are relative to this, instead of 0,0,0
        }

        /// <summary>
        /// Attaches other to this block, and keeps other in the same location
        /// </summary>
        /// <param name="?"></param>
        public void AttachToMeAbsolute(N8Block other)
        {
            other.position = other.position - this.position;
            AttachToMe(other);
        }

        public void Detach()
        {
            if (this.AttachedTo != null)
            {
                N8Block Attacher = this.AttachedTo;
                this.AttachedTo = null;

                Attacher.Attachees.Remove(this);
            }
        }

        public bool Equals(N8Block other)
        {
            //Just checking ID, should maybe check other stuff potentially.
            if (this._id == other._id)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

    }
}
