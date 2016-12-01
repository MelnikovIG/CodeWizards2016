using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Basic
{
    public struct Vector
    {
        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }

        public static Vector operator +(Vector vector1, Vector vector2)
        {
            return new Vector(vector1.X + vector2.X,
                              vector1.Y + vector2.Y);
        }

        public static Point2D operator +(Point2D point, Vector vector)
        {
            return new Point2D(point.X + vector.X, point.Y + vector.Y);
        }

        public static Vector operator -(Vector vector1, Vector vector2)
        {
            return new Vector(vector1.X - vector2.X,
                              vector1.Y - vector2.Y);
        }

        public static double operator *(Vector vector1, Vector vector2)
        {
            return vector1.X * vector2.X + vector1.Y * vector2.Y;
        }

        public static Vector operator *(double scalar, Vector vector)
        {
            return new Vector(vector.X * scalar,
                              vector.Y * scalar);
        }

        public static Vector operator /(Vector vector, double scalar)
        {
            return (1.0 / scalar) * vector;
        }

        public void Negate()
        {
            X = -X;
            Y = -Y;
        }
    }
}
