using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Basic;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Helpers
{
    public static class MoveHelper
    {
        public static void MoveTo(Point2D targetPoint)
        {
            var stuckedLivingUnins = UnitHelper.GetForwardStuckedLivingUnits();
            if (stuckedLivingUnins.Any())
            {
                var firstUnit = stuckedLivingUnins.First();

                var angle1 = Tick.Self.GetAngleTo(firstUnit);
                Tick.Move.Turn = angle1 <= 0 ? angle1 + Math.PI/2 : angle1 - Math.PI/2;
                Tick.Move.Speed = Tick.Game.WizardForwardSpeed;

                //Если застряли надолго пробуем пробить путь вперед
                if (GameState.StackedTickCount >= 30)
                {
                    if (firstUnit.Faction != Tick.Self.Faction)
                    {
                        Strategy.AtackTarget(firstUnit);
                    }
                }

                GameState.StackedTickCount++;

                return;
            }
            GameState.StackedTickCount = 0;

            var nearestTargetPoint = GetNearestWayPointForPoint(targetPoint);

            var myPosition = new Point2D(Tick.Self.X, Tick.Self.Y);
            var getNearestMyWaypoint = GetNearestWayPointForPoint(myPosition);

            var pathNextPoint = new Point2D();

            if (nearestTargetPoint == getNearestMyWaypoint)
            {
                pathNextPoint = nearestTargetPoint.Position;
            }
            else
            {
                var path = WayPointsHelper.BuildWayBetweenWaypoints(getNearestMyWaypoint, nearestTargetPoint);
                var angleWaypoints = AngleBetweenPoints(path[1].Position, path[0].Position, new Point2D(Tick.Self.X, Tick.Self.Y));
                pathNextPoint = Math.Abs(angleWaypoints) <= Math.PI/2 ? path[1].Position : path[0].Position;
            }

            var angle = Tick.Self.GetAngleTo(pathNextPoint.X, pathNextPoint.Y);
            var speed = Tick.Game.WizardForwardSpeed;

            Tick.Move.Speed = speed;
            Tick.Move.Turn = angle;
        }

        //Идти задним ходом к цели
        public static void MoveBackwardTo(Point2D targetPoint)
        {
            var stuckedLivingUnins = UnitHelper.GetForwardStuckedLivingUnits();
            if (stuckedLivingUnins.Any())
            {
                var firstUnit = stuckedLivingUnins.First();

                var angle1 = Tick.Self.GetAngleTo(firstUnit);
                Tick.Move.Turn = angle1 <= 0 ? angle1 + Math.PI / 2 : angle1 - Math.PI / 2;
                Tick.Move.Speed = -Tick.Game.WizardBackwardSpeed;

                GameState.StackedTickCount++;

                //Если застряли надолго пробуем пробить путь вперед
                if (GameState.StackedTickCount >= 30)
                {
                    if (firstUnit.Faction != Tick.Self.Faction)
                    {
                        Strategy.AtackTarget(firstUnit);
                    }
                }

                return;
            }
            GameState.StackedTickCount = 0;

            var nearestTargetPoint = GetNearestWayPointForPoint(targetPoint);

            var myPosition = new Point2D(Tick.Self.X, Tick.Self.Y);
            var getNearestMyWaypoint = GetNearestWayPointForPoint(myPosition);

            var pathNextPoint = new Point2D();

            if (nearestTargetPoint == getNearestMyWaypoint)
            {
                pathNextPoint = nearestTargetPoint.Position;
            }
            else
            {
                var path = WayPointsHelper.BuildWayBetweenWaypoints(getNearestMyWaypoint, nearestTargetPoint);

                var angleWaypoints = AngleBetweenPoints(path[1].Position, path[0].Position, new Point2D(Tick.Self.X, Tick.Self.Y));
                pathNextPoint = Math.Abs(angleWaypoints) <= Math.PI / 2 ? path[1].Position : path[0].Position;
            }

            var angle = Tick.Self.GetAngleTo(pathNextPoint.X, pathNextPoint.Y) + Math.PI/2;
            var speed =  Tick.Game.WizardBackwardSpeed;

            Tick.Move.Speed = -speed;
            Tick.Move.Turn = -angle;
        }

        private static WayPoint GetNearestWayPointForPoint(Point2D piont)
        {
            var nearestTargetWaypointDistance =
                WayPointsHelper.GetWayPoints().Select(x => x.Position.GetDistanceTo(piont)).Min();
            var nearestTargetWaypoint =
                WayPointsHelper.GetWayPoints()
                    .First(x => x.Position.GetDistanceTo(piont) == nearestTargetWaypointDistance);
            return nearestTargetWaypoint;
        }

        private static double AngleBetweenPoints(Point2D a, Point2D b, Point2D c)
        {
            double x1 = a.X - b.X, x2 = c.X - b.X;
            double y1 = a.Y - b.Y, y2 = c.Y - b.Y;
            double d1 = Math.Sqrt(x1 * x1 + y1 * y1);
            double d2 = Math.Sqrt(x2 * x2 + y2 * y2);
            return Math.Acos((x1 * x2 + y1 * y2) / (d1 * d2));
        }

    }
}
