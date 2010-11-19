using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace N8Parser.Geometry
{
    /// <summary>
    /// Represents a point in a cylindrical coordinate system. Uses radians, of course.
    /// </summary>
    public class Cylindrical
    {
        Vector3D coordinates;

        public Cylindrical()
        {
            coordinates = new Vector3D();
        }

        /// <summary>
        /// Creates a point in a cylindrical coordinate system.
        /// </summary>
        /// <param name="R">Planar radius from 0,0,z to this point</param>
        /// <param name="Theta">Planar angle to this point, in radians</param>
        /// <param name="Z">Height of this point</param>
        public Cylindrical(double R, double Theta, double Z)
        {
            coordinates = new Vector3D(R, Theta, Z);
        }

        /// <summary>
        /// Creates a cylindrical representation of a point represented by Cartesian coordinates.
        /// </summary>
        /// <param name="Cartesian">The Cartesian coordinates to convert</param>
        public Cylindrical(Vector3D Cartesian)
        {
            double r = Math.Sqrt(Math.Pow(Cartesian.X, 2) + Math.Pow(Cartesian.Y, 2));
            double theta = Math.Atan2(Cartesian.Y, Cartesian.X);
            double z = Cartesian.Z;

            coordinates = new Vector3D(r, theta, z);
        }

        /// <summary>
        /// Retrieves a Cartesian representation of this point
        /// </summary>
        /// <returns>This point as a point in a Cartesian space</returns>
        public Vector3D ToCartesian()
        {
            Vector3D result;

            double x = R * Math.Cos(Theta);
            double y = R * Math.Sin(Theta);
            double z = Z;

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

        public double Z
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

