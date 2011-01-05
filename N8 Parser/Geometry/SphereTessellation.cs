using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N8Parser.Geometry
{
    public class SphereTessellation
    {
        /// <summary>
        /// Attempts to tessellate a sphere using a point repulsion method inspired by this:
        /// http://www.math.niu.edu/~rusin/known-math/96/repulsion
        /// </summary>
        public static List<Spherical> TessellateSphere(int NumPoints, int MaxSteps)
        {
            List<Spherical> ret = new List<Spherical>(NumPoints);
            Random rand = new Random();
            return ret;
        }
    }
}
