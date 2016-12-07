using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Basic;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Helpers
{
    public static class VisualClientHelper
    {
        public static void BeginPost()
        {
            if (DebugHelper.CanDebug)
            {
                VisualClient.Instance.BeginPost();
            }
        }

        public static void EndPost()
        {
            if (DebugHelper.CanDebug)
            {
                VisualClient.Instance.EndPost();
            }
        }

        public static void Line(Point2D start, Point2D end, float r, float g, float b)
        {
            if (DebugHelper.CanDebug)
            {
                VisualClient.Instance.Line(start.X, start.Y, end.X, end.Y, r, g, b);
            }
        }

        public static void Circle(CircularUnit circularUnit, VisualClientColor color)
        {
            if (DebugHelper.CanDebug)
            {
                VisualClient.Instance.Circle(circularUnit.X, circularUnit.Y, (float)circularUnit.Radius, color.r, color.g, color.b);
            }
        }

        public static void Circle(Point2D center, float radius, VisualClientColor color)
        {
            if (DebugHelper.CanDebug)
            {
                VisualClient.Instance.Circle(center.X, center.Y, radius, color.r, color.g, color.b);
            }
        }

        [Obsolete]
        public static void Circle(double x, double y, float radius, float r = 0f, float g = 0f, float b = 0f)
        {
            if (DebugHelper.CanDebug)
            {
                VisualClient.Instance.Circle(x, y, radius, r, g, b);
            }
        }

        public static void Rect(Point2D p1, Point2D p2, VisualClientColor color)
        {
            if (DebugHelper.CanDebug)
            {
                VisualClient.Instance.Rect(p1.X, p1.Y, p2.X, p2.Y, color.r, color.g, color.b);
            }
        }
    }

    public struct VisualClientColor
    {
        public float r;
        public float g;
        public float b;

        public VisualClientColor(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }
    }
}
