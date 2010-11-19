using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace N8Parser
{
    /// <summary>
    /// Represents a point in a spherical coordinate system. Uses radians, of course.
    /// </summary>
    public class Spherical
    {
        Vector3D coordinates;

        public Spherical()
        {
            coordinates = new Vector3D();
        }

        /// <summary>
        /// Creates a point in a spherical coordinate system.
        /// </summary>
        /// <param name="R">Distance from 0,0,0 to this point</param>
        /// <param name="Theta">Elevation of this point, in radians</param>
        /// <param name="Phi">Distance around the sphere, in radians</param>
        public Spherical(double R, double Theta, double Phi)
        {
            coordinates = new Vector3D(R, Theta, Phi);
        }

        /// <summary>
        /// Creates a spherical representation of a point represented by Cartesian coordinates.
        /// </summary>
        /// <param name="Cartesian">The Cartesian coordinates to convert</param>
        public Spherical(Vector3D Cartesian)
        {
            double r = Cartesian.Length;
            double phi = Math.Atan2(Cartesian.Y, Cartesian.X);
            double theta = Math.Acos(Cartesian.Z / r);

            coordinates = new Vector3D(r, theta, phi);
        }

        /// <summary>
        /// Retrieves a Cartesian representation of this point
        /// </summary>
        /// <returns>This point as a point in a Cartesian space</returns>
        public Vector3D ToCartesian()
        {
            Vector3D result;

            double x = R * Math.Sin(Theta) * Math.Cos(Phi);
            double y = R * Math.Sin(Theta) * Math.Sin(Phi);
            double z = R * Math.Cos(Theta);

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

        public double Phi
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
