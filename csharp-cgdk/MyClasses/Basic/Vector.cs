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

        public Vector(Point2D point)
        {
            X = point.X;
            Y = point.Y;
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

        public double Length => Math.Sqrt(X * X + Y * Y);

        public static double AngleBetweenInRadians(Vector vector1, Vector vector2)
        {
            double sin = vector1.X * vector2.Y - vector2.X * vector1.Y;
            double cos = vector1.X * vector2.X + vector1.Y * vector2.Y;

            return Math.Atan2(sin, cos);
        }

        public static double AngleBetweenInDegrees(Vector vector1, Vector vector2)
        {
            return AngleBetweenInRadians(vector1, vector2)*(180/Math.PI);
        }

        public void Normalize()
        {
            // Avoid overflow
            this /= Math.Max(Math.Abs(X), Math.Abs(Y));
            this /= Length;
        }
    }
}
