using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Basic
{
    [DebuggerDisplay("x = {X} y = {Y}")]
    public class Point2D
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

        public static Point2D GetPointAt(Point2D startPoint, double angle, double range)
        {
            return new Point2D(
                startPoint.X - range * Math.Cos(angle + Math.PI /*/ 2*/),
                startPoint.Y - range * Math.Sin(angle + Math.PI /*/ 2*/)
                );
        }
    }
}
