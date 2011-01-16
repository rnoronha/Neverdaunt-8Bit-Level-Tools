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
            Axis = new Vector3D(0, 0, 1);
        }

        /// <summary>
        /// Creates a point in a cylindrical coordinate system with the default axis of 0,0,1
        /// </summary>
        /// <param name="R">Planar radius from 0,0,z to this point</param>
        /// <param name="Theta">Planar angle to this point, in radians</param>
        /// <param name="Z">Height of this point</param>
        public Cylindrical(double R, double Theta, double Z)
        {
            Axis = new Vector3D(0, 0, 1);
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
            Axis = new Vector3D(0, 0, 1);

            coordinates = FromCartesian(ToRightHand(Cartesian));
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
            this.Axis = Axis;
            coordinates = new Vector3D(R, Theta, Z);
        }

        /// <summary>
        /// Creates a cylindrical representation of a point represented by Cartesian coordinates.
        /// </summary>
        /// <param name="Cartesian">The Cartesian coordinates to convert</param>
        /// <param name="Axis">The axis along which this cylinder will run</param>
        public Cylindrical(Vector3D Cartesian, Vector3D Axis)
        {
            this.Axis = Axis;
            coordinates = FromCartesian(ToRightHand(Cartesian));
        }

        //From http://www.euclideanspace.com/maths/geometry/space/coordinates/polar/cylindrical/index.htm
        private Vector3D FromCartesian(Vector3D Cartesian)
        {
            double r;
            double theta;
            double h;
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

            double x = Sqrt(Sq(axis.Z) + Sq(axis.Y)) * R * Math.Sin(Theta) + H * axis.X;
            double y = Sqrt(Sq(axis.Z) + Sq(axis.X)) * R * Math.Cos(Theta) + H * axis.Y;
            double z = -axis.X * R * Math.Sin(Theta) - axis.Y * R * Math.Cos(Theta) + H * axis.Z;


            result = ToN8(new Vector3D(x, y, z));
            return result;
        }

        //Converts a vector like I normally use (an N8 vector I guess)
        //To a vector in the right-hand coordinate system the cylinder page uses
        private Vector3D ToRightHand(Vector3D N8)
        {
            return new Vector3D(N8.Y, N8.Z, N8.X);
        }

        //Does the reverse of the previous
        private Vector3D ToN8(Vector3D RightHand)
        {
            return new Vector3D(RightHand.Z, RightHand.X, RightHand.Y);
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

        /// <summary>
        /// Distance around the circle in radians
        /// </summary>
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

        /// <summary>
        /// Distance around the circle in degrees
        /// </summary>
        public double Theta_Degrees
        {
            get
            {
                return coordinates.Y * Utilities.RadToDeg;
            }
            set
            {
                coordinates.Y = value * Utilities.DegToRad;
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
                return ToN8(axis);
            }

            private set
            {
                this.axis = ToRightHand(value);
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

