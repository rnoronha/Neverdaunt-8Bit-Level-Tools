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
        /// Convert a quaternion rotation to a point in a spherical coordinate system.
        /// 
        /// Who knows how the fuck it works?
        /// 
        /// Copied from here: http://web.archive.org/web/20041029003853/http:/www.j3d.org/matrix_faq/matrfaq_latest.html#Q59
        /// </summary>
        /// <param name="q"></param>
        public Spherical(Quaternion q)
        {
            coordinates = new Vector3D();
            this.R = 1;

            double cos_a = q.W;
            double sin_a = Math.Sqrt(1.0 - cos_a * cos_a);

            double angle = Math.Acos(cos_a) * 2;
            if (Math.Abs(sin_a) < 0.0000000000005) sin_a = 1;

            double tX = q.X / sin_a;
            double tY = q.Y / sin_a;
            double tZ = q.Z / sin_a;

            this.Phi = -Math.Asin(tY);
            this.Theta = 0;

            if ((tX * tX + tZ * tZ) < 0.0000000000005)
            {
                this.Theta = 0;
            }
            else
            {
                this.Theta = Math.Atan2(tX, tZ);
            }

            while (this.Theta < 0)
            {
                this.Theta += 2 * Math.PI;
            }

        }

        /// <summary>
        /// Creates a point in a spherical coordinate system.
        /// </summary>
        /// <param name="R">Distance from 0,0,0 to this point</param>
        /// <param name="Theta">Elevation of this point, in degrees</param>
        /// <param name="Phi">Distance around the sphere, in degrees</param>
        public Spherical(double R, int Theta, int Phi)
        {
            coordinates = new Vector3D(R, Theta * Utilities.DegToRad, Phi * Utilities.DegToRad);
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

        public Quaternion GetNormalRotation()
        {
            Quaternion PhiPart = new Quaternion(new Vector3D(0, 0, 1), -(this.Phi * Utilities.RadToDeg));
            Quaternion ThetaPart = new Quaternion(new Vector3D(0, 1, 0), -this.Theta * Utilities.RadToDeg);

            Quaternion rotation = ThetaPart * PhiPart;

            return rotation;
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
