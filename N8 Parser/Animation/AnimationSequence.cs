using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N8Parser.Animation
{
    public class AnimationSequence
    {
        private List<AnimationFrame> _anim;
        public List<AnimationFrame> Animation
        {
            get
            {
                return _anim;
            }
        }

        public int Length
        {
            get
            {
                return Animation.Count;
            }
        }

        public AnimationSequence()
        {
            _anim = new List<AnimationFrame>();
        }

        public void AddFrame(AnimationFrame a)
        {
            _anim.Add(a);
        }

        public AnimationSequence DeltaCompress()
        {
            if (Animation.Count <= 1)
            {
                return this;
            }
            AnimationSequence ret = new AnimationSequence();
            ret.AddFrame(Animation[0]);

            for (int i = 1; i < Animation.Count; i++)
            {
                ret.AddFrame(Animation[i - 1].delta(Animation[i]));
            }

            return ret;
        }

        public string ToData()
        {
            StringBuilder ret = new StringBuilder();
            foreach (AnimationFrame a in Animation)
            {
                ret.Append(a.ToData() + "|");
            }

            return ret.ToString();
        }
    }
}
