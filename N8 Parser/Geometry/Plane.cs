using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace N8Parser
{
    class Plane
    {
        Vector3D parts;
        double d;

        public Vector3D VectorPart
        {
            get
            {
                return parts;
            }
            set
            {
                parts = value;
            }
        }

        public double A
        {
            get
            {
                return parts.X;
            }
            set
            {
                parts.X = value;
            }
        }

        public double B
        {
            get
            {
                return parts.Y;
            }
            set
            {
                parts.Y = value;
            }
        }

        public double C
        {
            get
            {
                return parts.Z;
            }
            set
            {
                parts.Z = value;
            }
        }

        public double D
        {
            get
            {
                return d;
            }
            set
            {
                d = value;
            }
        }

        public Plane(Vector3D vector, double d)
        {
            parts = vector;
            this.d = d;
        }

        public Plane(double a, double b, double c, double d)
        {
            parts = new Vector3D(a, b, c);
            this.d = d;
        }

        public Plane()
        {
            parts = new Vector3D();
            this.d = 0;
        }

        public double DistToPoint(Vector3D Point)
        {
            double ret;

            ret = Utilities.DotProduct(parts, Point) + d;
            ret = ret / parts.LengthSquared;

            return ret;
        }
    }
}
