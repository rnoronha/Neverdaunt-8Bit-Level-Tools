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
        private static Vector3D DefaultAxis = new Vector3D(0,0,1);
        Vector3D coordinates;
        Vector3D axis;

        public Cylindrical()
        {
            coordinates = new Vector3D();
            axis = new Vector3D(0, 0, 1);
        }

        /// <summary>
        /// Creates a point in a cylindrical coordinate system with the default axis of 0,0,1
        /// </summary>
        /// <param name="R">Planar radius from 0,0,z to this point</param>
        /// <param name="Theta">Planar angle to this point, in radians</param>
        /// <param name="Z">Height of this point</param>
        public Cylindrical(double R, double Theta, double Z)
        {
            axis = new Vector3D(0, 0, 1);
            coordinates = new Vector3D(R, Theta, Z);
        }

        /// <summary>
        /// Creates a cylindrical representation of a point represented by Cartesian coordinates with the default axis of 0,0,1.
        /// </summary>
        /// <param name="Cartesian">The Cartesian coordinates to convert</param>
        public Cylindrical(Vector3D Cartesian)
        {
            /*
            double r = Math.Sqrt(Math.Pow(Cartesian.X, 2) + Math.Pow(Cartesian.Y, 2));
            double theta = Math.Atan2(Cartesian.Y, Cartesian.X);
            double z = Cartesian.Z;
            */
            axis = new Vector3D(0, 0, 1);

            coordinates = FromCartesian(Cartesian);
        }

        /// <summary>
        /// Creates a point in a cylindrical coordinate system.
        /// </summary>
        /// <param name="R">Planar radius from h * Axis to this point</param>
        /// <param name="Theta">Planar angle to this point, in radians</param>
        /// <param name="Z">Height of this point</param>
        /// <param name="Axis">The axis along which this cylinder runs</param>
        public Cylindrical(double R, double Theta, double Z, Vector3D Axis)
        {
            axis = Axis;
            coordinates = new Vector3D(R, Theta, Z);
        }

        /// <summary>
        /// Creates a cylindrical representation of a point represented by Cartesian coordinates.
        /// </summary>
        /// <param name="Cartesian">The Cartesian coordinates to convert</param>
        /// <param name="Axis">The axis along which this cylinder will run</param>
        public Cylindrical(Vector3D Cartesian, Vector3D Axis)
        {
            double r = Sqrt(Sq(Cartesian.X) + Sq(Cartesian.Y));
            double theta = Math.Atan2(Cartesian.Y, Cartesian.X);
            double z = Cartesian.Z;

            axis = Axis;
            coordinates = FromCartesian(Cartesian);
        }

        //From http://www.euclideanspace.com/maths/geometry/space/coordinates/polar/cylindrical/index.htm
        private Vector3D FromCartesian(Vector3D Cartesian)
        {
            double r;
            double theta;
            double h;
            double ax_z_sq = axis.Z * axis.Z;
            double ax_x_sq = axis.X * axis.X;
            double ax_y_sq = axis.Y * axis.Y;
            double ax_y_z_part = (Cartesian.X * Sqrt(Sq(axis.Z) + Sq(axis.Y)) - axis.X * Cartesian.Z);
            double ax_x_z_part = (Cartesian.Y * Sqrt(Sq(axis.Z) + Sq(axis.X)) - axis.Y * Cartesian.Z);

            r = Sqrt(Sq(ax_x_z_part) + Sq(ax_y_z_part));
            theta = Math.Atan2(ax_y_z_part, ax_x_z_part);
            h = Utilities.DotProduct(axis, Cartesian);

            return new Vector3D(r, theta, h);
        }

        /// <summary>
        /// Retrieves a Cartesian representation of this point
        /// </summary>
        /// <returns>This point as a point in a Cartesian space</returns>
        public Vector3D ToCartesian()
        {
            Vector3D result;

            double x = Math.Sqrt(axis.Z * axis.Z + axis.Y * axis.Y) * R * Math.Sin(Theta) + H * axis.X;
            double y = Math.Sqrt(axis.Z * axis.Z + axis.X * axis.X) * R * Math.Cos(Theta) + H * axis.Y;
            double z = -axis.X * R * Math.Sin(Theta) - axis.Y * R * Math.Cos(Theta) + H * axis.Z;


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

        public double H
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

        public Vector3D Axis
        {
            get
            {
                return axis;
            }
        }



        public Quaternion GetNormalRotation()
        {
            return new Quaternion(Axis, Theta * Utilities.RadToDeg);
        }

        //Some functions to make the math look nicer
        private double Sq(double x)
        {
            return x * x;
        }

        private double Sqrt(double x)
        {
            return Math.Sqrt(x);
        }

    }
}

