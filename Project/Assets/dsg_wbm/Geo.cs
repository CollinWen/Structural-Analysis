using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dsg_wbm
{
    public class Vec3d
    {
        public double X { get; set; } // x coordinate
        public double Y { get; set; } // y coordinate
        public double Z { get; set; } // z coordinate
        
        public double Length
        {
            get
            {
                return GetMag();
            }
        }

        // Constructors

        /// <summary>
        /// Base initializer
        /// </summary>
        public Vec3d()
        {
            X = 0.0;
            Y = 0.0;
            Z = 0.0;
        }

        /// <summary>
        /// Array of points
        /// </summary>
        /// <param name="positions"></param>
        public Vec3d(double[] positions)
        {
            X = positions[0];
            Y = positions[1];
            Z = positions[2];
        }

        /// <summary>
        /// Individual points
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vec3d(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// Reference point + move vector
        /// </summary>
        /// <param name="start"></param>
        /// <param name="move"></param>
        public Vec3d(Point3d start, Vec3d move)
        {
            this.X = start.X + move.X;
            this.Y = start.Y + move.Y;
            this.Z = start.Z + move.Z;
        }

        // Private Methods
        private double GetMag()
        {
            double sq1 = this.X * this.X + this.Y * this.Y + this.Z * this.Z;
            double len = Math.Sqrt(sq1);
            return len;
        }

        // Public methods
        public void Normalize()
        {
            this.X /= this.Length;
            this.Y /= this.Length;
            this.Z /= this.Length;
        }
        /// <summary>
        /// Returns a normalized vector for new construction
        /// </summary>
        /// <returns></returns>
        public Vec3d GetNormalizedVector()
        {
            double newX = this.X / this.Length;
            double newY = this.Y / this.Length;
            double newZ = this.Z / this.Length;

            return new Vec3d(newX, newY, newZ);
        }
        /// <summary>
        /// Convert to Point3d
        /// </summary>
        /// <returns></returns>
        public Point3d ToPoint3d()
        {
            return new Point3d(this.X, this.Y, this.Z);
        }

        /// <summary>
        /// Convert to Array of double[]
        /// </summary>
        /// <returns></returns>
        public double[] ToArray()
        {
            return new double[3] { this.X, this.Y, this.Z };
        }
    }

    public class Point3d
    {
        // Fields
        public double X { get; set; } // x coordinate
        public double Y { get; set; } // y coordinate
        public double Z { get; set; } // z coordinate
        public double[] Position {
            get
            {
                return new[] { X, Y, Z };
            }
        }
        public double Mag
        {
            get
            {
                return GetMag();
            }
        }

        // Constructors

        /// <summary>
        /// Base initializer
        /// </summary>
        public Point3d()
        {
            X = 0.0;
            Y = 0.0;
            Z = 0.0;
        }

        /// <summary>
        /// Array of points
        /// </summary>
        /// <param name="positions"></param>
        public Point3d(double[] positions)
        {
            X = positions[0];
            Y = positions[1];
            Z = positions[2];
        }

        /// <summary>
        /// Individual points
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Point3d(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// Reference point + move vector
        /// </summary>
        /// <param name="start"></param>
        /// <param name="move"></param>
        public Point3d(Point3d start, Vec3d move)
        {
            this.X = start.X + move.X;
            this.Y = start.Y + move.Y;
            this.Z = start.Z + move.Z;
        }

        // Methods
        // Magnitude of vector
        private double GetMag()
        {
            double sq1 = this.X * this.X + this.Y * this.Y + this.Z * this.Z;
            double len = Math.Sqrt(sq1);
            return len;
        }
        public double DistanceTo(Point3d other)
        {
            double xdist = this.X - other.X;
            double ydist = this.Y - other.Y;
            double zdist = this.Z - other.Z;

            return Math.Sqrt(xdist * xdist + ydist * ydist + zdist * zdist);
        }

        /// <summary>
        /// Moves a point
        /// </summary>
        /// <param name="direction"></param>
        public void Move(Vec3d direction)
        {
            this.X += direction.X;
            this.Y += direction.Y;
            this.Z += direction.Z;
        }

        /// <summary>
        /// Convert to Array of double[]
        /// </summary>
        /// <returns></returns>
        public double[] ToArray()
        {
            return new double[3] { this.X, this.Y, this.Z };
        }
    }
}