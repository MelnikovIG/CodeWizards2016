using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Basic
{
    public struct Point2D
    {
        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }

        public double GetDistanceTo(Point2D point)
        {
            return Hypotenuse(X - point.X, Y - point.Y);
        }

        public double GetDistanceTo(double x, double y)
        {
            return Hypotenuse(X - x, Y - y);
        }

        public static double Hypotenuse(double a, double b)
        {
            return Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
        }
    }
}
