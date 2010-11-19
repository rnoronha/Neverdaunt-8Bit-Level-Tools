using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace N8Parser.Geometry
{
    class SidewaysCylindrical
    { 
        Vector3D coordinates;

        public SidewaysCylindrical()
        {
            coordinates = new Vector3D();
        }

        /// <summary>
        /// Creates a point in a cylindrical coordinate system.
        /// </summary>
        /// <param name="R">Planar radius from 0,0,y to this point</param>
        /// <param name="Theta">Planar angle to this point, in radians</param>
        /// <param name="Y">Height of this point</param>
        public SidewaysCylindrical(double R, double Theta, double Y)
        {
            coordinates = new Vector3D(R, Theta, Y);
        }

        /// <summary>
        /// Creates a cylindrical representation of a point represented by Cartesian coordinates.
        /// </summary>
        /// <param name="Cartesian">The Cartesian coordinates to convert</param>
        public SidewaysCylindrical(Vector3D Cartesian)
        {
            double r = Math.Sqrt(Math.Pow(Cartesian.X, 2) + Math.Pow(Cartesian.Z, 2));
            double theta = Math.Atan2(Cartesian.Z, Cartesian.X);
            double y = Cartesian.Y;

            coordinates = new Vector3D(r, theta, y);
        }

        /// <summary>
        /// Retrieves a Cartesian representation of this point
        /// </summary>
        /// <returns>This point as a point in a Cartesian space</returns>
        public Vector3D ToCartesian()
        {
            Vector3D result;

            double x = R * Math.Cos(Theta);
            double z = R * Math.Sin(Theta);
            double y = Y;

            result = new Vector3D(x, y, z);

            return result;
        }

        public double R
        {
            get
            {
                return coordinates.X;
            }

            set
            {
                coordinates.X = value;
            }
        }


        public double Theta
        {
            get
            {
                return coordinates.Y;
            }
            set
            {
                coordinates.Y = value;
            }
        }

        public double Y
        {
            get
            {
                return coordinates.Z;
            }
            set
            {
                coordinates.Z = value;
            }
        }
    }
}
