using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using N8Parser.Geometry;
using System.Drawing;

namespace N8Parser
{
    public static class Utilities
    {

        public static double RadToDeg = 180 / Math.PI;
        public static double DegToRad = Math.PI / 180;
        public const int MAX_BLOCK_COUNT = 349;

        public static string ToData(this Vector3D me)
        {
            return "v" + me.X + "," + me.Z + "," + me.Y;
        }

        public static string ToData(this Quaternion me)
        {
            return "q" + me.W + "," + me.X + "," + me.Z + "," + me.Y;
        }

        public static IEnumerable<N8Block> GetNotLands(N8Level Input)
        {
            var NotLands = from N8Block b in Input.blocks.Blocks where b.type != "landmega" select b;
            return NotLands;
        }

        public static string GetDefaultSaveFolder()
        {
            return Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86) + @"\N8\Saves\";
        }

        /// <summary>
        /// Makes an even circle with a known number of blocks
        /// </summary>
        /// <param name="Count">Number of blocks</param>
        /// <param name="Radius">Radius of circle</param>
        /// <param name="Axis">Axis about which to make the circle</param>
        /// <returns>A list of cylindricals defining the circle</returns>
        public static List<Cylindrical> MakeCircle(int Count, int Radius, Vector3D Axis)
        {
            List<Cylindrical> ret = new List<Cylindrical>(Count);
            double tstep = (double)360 / (double)Count;
            for (int i = 0; i < Count; i++)
            {
                ret.Add(new Cylindrical(Radius, i * tstep * Utilities.DegToRad, 0, Axis));
            }

            return ret;
        }


        //This might have some useful stuff on tetrahedrons:
        //http://www.kjmaclean.com/Geometry/Tetrahedron.html
        //And of course the Wikipedia page:
        //http://en.wikipedia.org/wiki/Tetrahedron
        public static List<Tuple<Vector3D, Quaternion>> GenerateTetrahedron(Vector3D center, int count, double BlockLength)
        {
            List<Tuple<Vector3D, Quaternion>> temp = new List<Tuple<Vector3D, Quaternion>>();

            //First spherical is the point, second spherical is the angle from that point.
            List<Tuple<Spherical, Spherical>> points = new List<Tuple<Spherical,Spherical>>();

            //It's pretty bad, but I generated these equations basically by experiment.
            for (int i = 0; i < 3; i++)
            {
                points.Add(Tuple.Create(new Spherical(1, 3 * Math.PI / 4, (i * (2 * Math.PI) / 3) + Math.PI / 2), new Spherical(1, Math.PI / 2, (i - 1) * (2 * Math.PI) / 3)));
                points.Add(Tuple.Create(new Spherical(1, 3 * Math.PI / 4, (i * (2 * Math.PI) / 3) + Math.PI / 2), new Spherical(1, Math.PI / 5, (i - 1) * (2 * Math.PI) / 3 + Math.PI/6)));

                //points.Add(Tuple.Create(new Spherical(1, 3 * Math.PI / 4, 3 * Math.PI / 4), new Spherical(1, 0, 6 * Math.PI / 4)));
                //points.Add(Tuple.Create(new Spherical(1, 3 * Math.PI / 4, 6 * Math.PI / 4), new Spherical(1, 0, 9 * Math.PI / 4)));
            }


            //Magic! I'm sure this is entirely not right at all, but hey it works.
            //Wikipedia said Sqrt(3/8) * Count * BlockLength, but that just didn't work.
            double RealRadius = 7.0/6.0 * (count * BlockLength/2.0)/(Math.Sin(Math.PI/4.0));

            Vector3D Top = points[0].Item1.ToCartesian();
            foreach (var t in points)
            {
                t.Item1.R = RealRadius;
                Vector3D InitPoint = t.Item1.ToCartesian() + center;
                Quaternion rotation = t.Item2.GetNormalRotation();
                var LineStuff = GenerateLine(InitPoint, t.Item2, count, BlockLength);
                List<Vector3D> line = LineStuff.Item1;
                if (Top.Z < LineStuff.Item2.Z)
                {
                    Console.WriteLine("Top is currently: " + Top);
                    Top = LineStuff.Item2;
                    Console.WriteLine("Changing it to:   " + Top);
                    Console.WriteLine("First block in this segment: " + line[0]);
                }

                foreach (Vector3D point in line)
                {
                    temp.Add(Tuple.Create(point, t.Item2.GetNormalRotation()));
                }

                t.Item1.R = 0;
                center = t.Item1.ToCartesian();
                Console.WriteLine("Center is: " + center);
            }

            Console.WriteLine("Top z is: " + Top);
            Console.WriteLine("Z downward offset is: " + Math.Sqrt(3.0)/(2.0 * Math.Sqrt(2.0)) * (BlockLength * count));

            List<Tuple<Vector3D, Quaternion>> ret = new List<Tuple<Vector3D, Quaternion>>(temp.Count);
            for(int i = 0; i < temp.Count; i++)
            {
                Vector3D p = temp[i].Item1;
                Console.WriteLine("This point's Z started at: " + p.Z);
                p.Z -= Top.Z;
                Console.WriteLine("Subtracted off top z:      " + p.Z);
                p.Z += Math.Sqrt(3.0) / (2.0 * Math.Sqrt(2.0)) * (BlockLength * count);
                Console.WriteLine("Changed this point's Z to: " + p.Z);
                ret.Add(Tuple.Create(p, temp[i].Item2));
            }

            return ret;
        }

        /// <summary>
        /// Generates a list of points representing a line, based on an initial point and a direction from that point
        /// </summary>
        /// <param name="from">The starting point</param>
        /// <param name="direction">The direction of travel</param>
        /// <param name="count">The number of steps to take</param>
        /// <param name="BlockLength">The length of each step</param>
        /// <returns>A tuple; first item is the list of points, the second item is one extra step in the direction.</returns>
        public static Tuple<List<Vector3D>, Vector3D> GenerateLine(Vector3D from, Spherical direction, int count, double BlockLength)
        {
            List<Vector3D> points = new List<Vector3D>();
            

            for (int i = 0; i < count; i++)
            {
                direction.R = i * BlockLength;
                Vector3D temp = direction.ToCartesian();
                temp += from;
                

                points.Add(temp);
            }
            direction.R = count * BlockLength;
            return Tuple.Create(points, direction.ToCartesian() + from);
        }

        public static Vector3D RotateVector(Quaternion rotation, Vector3D vector)
        {
            
            //Formula from: http://inside.mines.edu/~gmurray/ArbitraryAxisRotation/ArbitraryAxisRotation.html
            //And you can thank MSFT for not letting me just say Quaternion * Vector3D

            Vector3D ret = new Vector3D();
            double u = rotation.Axis.X; //hur hur double u
            double v = rotation.Axis.Y;
            double w = rotation.Axis.Z;

            double x = vector.X;
            double y = vector.Y;
            double z = vector.Z;
            double theta = rotation.Angle;

            double AxisDotVector = DotProduct(rotation.Axis, vector);

            double DistSquared = rotation.Axis.LengthSquared;
            double dist = rotation.Axis.Length;

            //good fucking grief I am crazy
            ret.X =
                (u * AxisDotVector + 
                (x * (Math.Pow(v, 2) + Math.Pow(w, 2)) - u * (v * y + w * z)) * Math.Cos(theta) + 
                 dist * (-w * y + v * z) * Math.Sin(theta))
                / DistSquared;
            ret.Y =
                (v * AxisDotVector +
                (y * (Math.Pow(u, 2) + Math.Pow(w, 2)) - v * (u * x + w * z)) * Math.Cos(theta) +
                 dist * (w * x - u * z) * Math.Sin(theta))
                / DistSquared;
            ret.Z =
                (w * AxisDotVector +
                (z * (Math.Pow(u, 2) + Math.Pow(v, 2)) - w * (u * x + v * y)) * Math.Cos(theta) +
                 dist * (-v * x + u * y) * Math.Sin(theta))
                / DistSquared;

            //And then it didn't work :(

            return ret;
        }


        /// <summary>
        /// Calculates the dot product of two vectors (i.e, sum of Ai * Bi)
        /// </summary>
        /// <param name="A">One vector</param>
        /// <param name="B">Another vector</param>
        /// <returns>The dot product of A and B</returns>
        public static double DotProduct(Vector3D A, Vector3D B)
        {
            double ret = A.X * B.X + A.Y * B.Y + A.Z * B.Z;
            return ret;
        }

        /// <summary>
        /// Rotates a block around the unit Z vector at the origin.
        /// </summary>
        /// <param name="block">The block to rotate</param>
        /// <param name="radians">Rotation to perform, in radians</param>
        /// <returns></returns>
        public static N8Block Rotate(N8Block block, double radians)
        {
            Cylindrical c = new Cylindrical(block.position);
            c.Theta += radians;
            Quaternion rot = new Quaternion(new Vector3D(0, 0, 1), radians * RadToDeg);

            block.rotation = rot * block.rotation;
            block.position = c.ToCartesian();

            return block;
        }

        /// <summary>
        /// Rotates a block around the origin
        /// </summary>
        /// <param name="block">The block to rotate</param>
        /// <param name="degrees">The rotation to perform, in degrees</param>
        /// <returns></returns>
        public static N8Block Rotate(N8Block block, int degrees)
        {
            //We end up converting twice, but I don't want to implement this twice.
            Console.WriteLine("Rotating block " + block + " by " + degrees + " degrees");
            return Rotate(block, degrees * DegToRad);
        }

        /// <summary>
        /// Projects a 3D vector onto another vector.
        /// </summary>
        /// <param name="Onto">The vector being projected on to</param>
        /// <param name="From">The vector that is being projected</param>
        /// <returns>The projection of From onto Onto</returns>
        public static Vector3D Project(Vector3D Onto, Vector3D From)
        {
            double FromDotOnto = DotProduct(From, Onto);
            double OntoDotOnto = DotProduct(Onto, Onto);
            double ratio = FromDotOnto/OntoDotOnto;

            Vector3D ret = new Vector3D(Onto.X, Onto.Y, Onto.Z);

            ret = ret * ratio;

            return ret;
        }

        /// <summary>
        /// Plots a set of blocks represented by points on the surface of a sphere, rotating them as needed
        /// </summary>
        /// <param name="points">The points to plot, on the surface of a unit sphere</param>
        /// <param name="InitialRotation">The initial rotation to apply to the blocks</param>
        /// <param name="Radius">The points radius from the real sphere</param>
        /// <param name="BlockType">The type of block to plot with (necessary)</param>
        /// <param name="BlockName">The name of the blocks (cosmetic)</param>
        /// <param name="level">The level to which these blocks should be added</param>
        /// <returns></returns>
        public static List<N8Block> PlotPointsSphere(List<Vector3D> points, Quaternion InitialRotation, double Radius, string BlockType, string BlockName, N8Level level)
        {
            List<N8Block> blocks = new List<N8Block>(points.Count);

            //In theory we could just take a point, multiply all of its vector components by the radius and plot it
            //unfortunately, this wouldn't give us the point's inclination or angle, which we need to rotate the block
            //it represents (otherwise you'd have things facing you at oblique angles)
            //Thus we convert it such that it's represented in spherical cooridnates.
            foreach (Vector3D point in points)
            {
                Spherical sphere = new Spherical(point);
                sphere.R = Radius;

                Quaternion PhiPart = new Quaternion(new Vector3D(0, 0, 1), -(sphere.Phi * RadToDeg));
                Quaternion ThetaPart = new Quaternion(new Vector3D(0, 1, 0), -sphere.Theta * RadToDeg);

                //Apparently quaternion rotations from an absolute frame of reference (e.g, the level) go in reverse order.
                //Keep in mind that quaternion multiplication does not commute - 
                Quaternion result = InitialRotation * ThetaPart * PhiPart;

                blocks.Add(PlotPoint(BlockType, BlockName, level, Tuple.Create(sphere.ToCartesian(), result)));

            }

            return blocks;

        }

        /// <summary>
        /// Randomly generates a sphere around the point labeled "around"
        /// </summary>
        /// <param name="Around">Which point to generate the sphere around</param>
        /// <param name="BlockSize">The block's size (if it were placed at the origin)</param>
        /// <param name="initialRotation">Initial rotation (should make the block stand up straight at x = +1000 y=0)</param>
        /// <param name="Radius">The radius of the sphere</param>
        /// <param name="BlockType">The block type to use</param>
        /// <param name="BlockName">The name of the block to use</param>
        /// <param name="level">The level to add the sphere to</param>
        /// <returns></returns>
        public static List<Tuple<Vector3D, Quaternion>> GenerateSphere(Vector3D Around, double Radius, int quantity=324, Random rand = null)
        {
            List<Tuple<Vector3D, Quaternion>> ret = new List<Tuple<Vector3D, Quaternion>>(quantity);
            double RadToDeg = 180 / Math.PI;
            if (rand == null)
            {
                rand = new Random();
            }
            for (int i = 0; i < quantity; i++)
            {
                Spherical position = rand.NextSpherical();
                position.R = Radius;

                Quaternion PhiPart = new Quaternion(new Vector3D(0, 0, 1), -(position.Phi * RadToDeg));
                Quaternion ThetaPart = new Quaternion(new Vector3D(0, 1, 0), -position.Theta * RadToDeg);

                Quaternion rotation = ThetaPart * PhiPart;
                Vector3D pos = position.ToCartesian() + Around;

                ret.Add(Tuple.Create(pos, rotation));
            }

            return ret;
        }

        public static List<Tuple<Vector3D, Quaternion>> EvenCircle(Vector3D Around, int BlockWidth, double Radius, double InitTheta=0)
        {
            List<Tuple<Vector3D, Quaternion>> ret = new List<Tuple<Vector3D, Quaternion>>();
            Cylindrical coords = new Cylindrical(Radius, 0, 0);
            Vector3D axis = new Vector3D(0, 0, 1);

            //Actually fuck all that shit, we can just calculate the circumference of the circle, 
            //divide by the block width, and then get theta from that!
            
            // Base case: radius = 0, we just return one element
            if (Radius == 0)
            {
                ret.Add(Tuple.Create(coords.ToCartesian() + Around, coords.GetNormalRotation()));
            }
            else
            {
                double circumference = 2 * Math.PI * Radius;
                double dTheta = (double)BlockWidth / circumference * 2 * Math.PI;
                for (double i = InitTheta; i < 2 * Math.PI + InitTheta; i += dTheta)
                {
                    coords.Theta = i;
                    if ((2 * Math.PI + InitTheta) - i < ((double)1 / 2 * dTheta))
                    {
                        //If the next one we place is gonna overlap a lot with the first one, end early
                        break;
                    }
                    ret.Add(Tuple.Create(coords.ToCartesian() + Around, coords.GetNormalRotation()));
                }
            }
            return ret;
        }

        
        public static List<Tuple<Vector3D, Quaternion>> EvenSphere(Vector3D Around, int BlockWidth, int Radius, double MaxPhi=Math.PI)
        {
            //For this, we make even circles in two dimensions basically. Who knows how well it'll work?
            List<Tuple<Vector3D, Quaternion>> ret = new List<Tuple<Vector3D, Quaternion>>();
            Spherical coords = new Spherical(Radius, 0, 0);

            double circumference = 2 * Math.PI * Radius;
            double dPhi = BlockWidth / circumference * 2 * Math.PI;
            double NextOffset = 0;

            //Phi only traverses half the sphere
            for (double i = 0; i < MaxPhi; i += dPhi)
            {
                coords.Phi = i;
                double z = coords.R * Math.Cos(coords.Phi);
                
                //Simple trig here, see e.g, http://en.allexperts.com/q/Advanced-Math-1363/Slice-sphere.htm
                double circle_radius = Math.Sqrt(coords.R * coords.R - z * z);

                List<Tuple<Vector3D, Quaternion>> ThisSlice =
                    (from Tuple<Vector3D, Quaternion> t in EvenCircle(new Vector3D(0, 0, z), BlockWidth, circle_radius, NextOffset)
                    select Tuple.Create(t.Item1 + Around, new Spherical(t.Item1).GetNormalRotation())).ToList();


                if (circle_radius != 0)
                {
                    //Rotate it around a bit to make a slightly better spread I hope.
                    double circle_circumference = 2 * Math.PI * circle_radius;
                    NextOffset = -BlockWidth / circumference * 2 * Math.PI;
                }

                ret.AddRange(ThisSlice);
            }

            return ret;

        }


        public static Tuple<Vector3D, Quaternion> PlaneToPositionAndRotation(Plane P, Quaternion InitialRotation)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Reflects a point across a plane through the origin defined by MirrorLine.
        /// </summary>
        /// <param name="SourcePoint">The point to reflect</param>
        /// <param name="MirrorLine">The line from the origin that defines the plane</param>
        /// <returns>The reflected point point</returns>
        public static Vector3D ReflectPlane(Vector3D SourcePoint, Vector3D MirrorLine)
        {
            Vector3D ret;

            ret = SourcePoint - 2 * (new Plane(MirrorLine, 0)).DistToPoint(SourcePoint) * MirrorLine;

            return ret;
        }

        /// <summary>
        /// Plots a list of points as BlockType.
        /// </summary>
        /// <param name="PointsAndRotations">The points at which these blocks should go, tupled with their rotations</param>
        /// <param name="BlockType">The type of block to add; this will also be the block's name</param>
        /// <param name="level">The level to which the new blocks should be added</param>
        /// <returns>List of blocks just added</returns>
        public static List<N8Block> PlotPoints(List<Tuple<Vector3D, Quaternion>> PointsAndRotations, string BlockType, N8Level level)
        {
            return PlotPoints(PointsAndRotations, BlockType, BlockType, level);
        }

        /// <summary>
        /// Plots a list of points as BlockType.
        /// </summary>
        /// <param name="points">The points at which these blocks should go</param>
        /// <param name="BlockType">The type of block to add; this will also be the block's name</param>
        /// <param name="level">The level to which the new blocks should be added</param>
        /// <returns>List of blocks just added</returns>
        public static List<N8Block> PlotPoints(List<Vector3D> points, string BlockType, N8Level level)
        {
            List<Tuple<Vector3D, Quaternion>> pAndr = (from Vector3D p in points select Tuple.Create(p, new Quaternion(0, 0, 0, 1))).ToList();

            return PlotPoints(pAndr, BlockType, BlockType, level);
        }

        /// <summary>
        /// Plots a list of points as BlockType and with Blockname.
        /// </summary>
        /// <param name="points">The points at which these blocks should go</param>
        /// <param name="BlockType">The type of block to add</param>
        /// <param name="BlockName">The name of the blocks to add</param>
        /// <param name="level">The level to which the new blocks should be added</param>
        /// <returns>List of blocks just added</returns>
        public static List<N8Block> PlotPoints(List<Vector3D> points, string BlockType, string BlockName, N8Level level)
        {
            List<Tuple<Vector3D, Quaternion>> pAndr = (from Vector3D p in points select Tuple.Create(p, new Quaternion(0, 0, 0, 1))).ToList();

            return PlotPoints(pAndr, BlockType, BlockName, level);
        }

        /// <summary>
        /// Plots a list of points as BlockType and with Blockname.
        /// </summary>
        /// <param name="PointsAndRotations">The points at which these blocks should go, tupled with their rotations</param>
        /// <param name="BlockType">The type of block to add</param>
        /// <param name="BlockName">The name of the blocks to add</param>
        /// <param name="level">The level to which the new blocks should be added</param>
        /// <returns>List of blocks just added</returns>
        public static List<N8Block> PlotPoints(List<Tuple<Vector3D, Quaternion>> PointsAndRotations, string BlockType, string BlockName, N8Level level)
        {
            List<N8Block> ret = new List<N8Block>(PointsAndRotations.Count);
            
            foreach (Tuple<Vector3D, Quaternion> PointAndRotation in PointsAndRotations)
            {
                ret.Add(PlotPoint(BlockType, BlockName, level, PointAndRotation));
            }

            return ret;

        }

        private static N8Block PlotPoint(string BlockType, string BlockName, N8Level level, Tuple<Vector3D, Quaternion> PointAndRotation)
        {
            N8Block temp = level.blocks.GenerateBlock(BlockType, BlockName);
            temp.position = PointAndRotation.Item1;
            temp.rotation = PointAndRotation.Item2;
            return temp;
        }

        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }



        /// <summary>
        /// Generates a random vector between a and b. Components are always integers.
        /// </summary>
        /// <param name="rand">The random number generator</param>
        /// <param name="a">One point between which to generate a random point</param>
        /// <param name="b">The second point</param>
        /// <returns>A random point between a and b</returns>
        public static Vector3D NextVector(this Random rand, Vector3D a, Vector3D b)
        {
            Vector3D mins = new Vector3D(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
            Vector3D maxs = new Vector3D(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));

            Vector3D ret = new Vector3D(rand.Next((int)mins.X, (int)maxs.X), rand.Next((int)mins.Y, (int)maxs.Y), rand.Next((int)mins.Z, (int)maxs.Z));

            return ret;
        }

        /// <summary>
        /// Generates a random quaternion. Equation stolen from here: 
        /// http://planning.cs.uiuc.edu/node198.html
        /// </summary>
        /// <param name="rand"></param>
        /// <returns>A random quaternion. I hope. I wish people would all use the same goddamn notation.</returns>
        public static Quaternion NextQuaternion(this Random rand)
        {
            double u_1 = rand.NextDouble();
            double u_2 = rand.NextDouble();
            double u_3 = rand.NextDouble();

            double Sqrt1MinU_1 = Math.Sqrt(1 - u_1);
            double SqrtU_1 = Math.Sqrt(u_1);

            double TwoPiU_2 = 2 * Math.PI * u_2;
            double TwoPiU_3 = 2 * Math.PI * u_3; 

            double w = Sqrt1MinU_1 * Math.Sin(TwoPiU_2);
            double i = Sqrt1MinU_1 * Math.Cos(TwoPiU_2);
            double j = SqrtU_1     * Math.Sin(TwoPiU_3);
            double k = SqrtU_1     * Math.Cos(TwoPiU_3);

            Quaternion ret = new Quaternion(i, j, k, w);
            return ret;
        }

        public static Vector3D round(this Vector3D input)
        {
            return new Vector3D(Math.Round(input.X), Math.Round(input.Y), Math.Round(input.Z));
        }
    

        /// <summary>
        /// Returns a random point on the surface of a unit sphere. Formula from here: http://mathproofs.blogspot.com/2005/04/uniform-random-distribution-on-sphere.html
        /// </summary>
        /// <param name="rand">The random thingy</param>
        /// <returns>A random point on the surface of a sphere</returns>
        public static Spherical NextSpherical(this Random rand)
        {
            double phi = 2 * Math.PI * rand.NextDouble();
            double theta = Math.Acos(1 - 2 * rand.NextDouble());

            Spherical ret = new Spherical(1, theta, phi);
            return ret;
        }

        public static void Save(string SavePath, N8Level Level)
        {

            if (!SavePath.EndsWith(".ncd"))
            {
                SavePath += ".ncd";
            }

            if (!File.Exists(SavePath))
            {
                using (File.Create(SavePath)) { }
            }

            using (StreamWriter sw = new StreamWriter(File.Open(SavePath, FileMode.Truncate, FileAccess.Write, FileShare.None)))
            {
                sw.WriteLine(Level.GenerateSaveFile());
            }
        }

        public static N8Level GetDefault()
        {
            string DefaultPath = @"C:\Program Files (x86)\N8\Saves\default.ncd";
            N8Level DefaultGround = new N8Level(DefaultPath);
            return DefaultGround;
        }

        public static void MergeWithDefault(N8Level toMerge)
        {
            string DefaultPath = @"C:\Program Files (x86)\N8\Saves\default.ncd";
            N8Level DefaultGround = new N8Level(DefaultPath);
            toMerge.MergeWithDestructive(DefaultGround);
        }

        public static string Sanitize(string data)
        {
            string SanitizedData = data;
            SanitizedData = SanitizedData.Replace(":", "\b");
            SanitizedData = SanitizedData.Replace("\n", "/n ");
            return SanitizedData;
        }

        
    }
}
