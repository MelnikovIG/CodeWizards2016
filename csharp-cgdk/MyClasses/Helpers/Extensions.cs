using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Basic;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Helpers
{
    public static class Extensions
    {
        public static Point2D GetPositionPoint(this Wizard wizard)
        {
            return new Point2D(wizard.X, wizard.Y);
        }

        public static void Line(this VisualClient visualClient, Point2D start, Point2D end, float r, float g, float b)
        {
            VisualClient.Instance.Line(start.X, start.Y, end.X, end.Y, r, g, b);
        }
    }
}
