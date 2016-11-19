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
        private static Random random = new Random();

        public static void MoveTo(MoveToParams moveToParams)
        {
            var stuckedLivingUnins = UnitHelper.GetForwardStuckedLivingUnits();
            if (stuckedLivingUnins.Any())
            {
                var firstUnit = stuckedLivingUnins.First();

                var randomAngle = random.NextDouble()*Math.PI/2;
                //var randomAngle = 0;
                var angleToUnit = Tick.Self.GetAngleTo(firstUnit);

                Tick.Move.Turn = angleToUnit >= 0
                    ? angleToUnit - Math.PI/2 - randomAngle
                    : angleToUnit + Math.PI/2 + randomAngle;
                Tick.Move.StrafeSpeed = angleToUnit >= 0 ? -Tick.Game.WizardStrafeSpeed : Tick.Game.WizardStrafeSpeed;
                Tick.Move.Speed = Math.Abs(angleToUnit) < Math.PI/2
                    ? Tick.Game.WizardForwardSpeed
                    : -Tick.Game.WizardBackwardSpeed;

                //Если застряли надолго пробуем пробить путь вперед
                if (GameState.StackedTickCount >= 30)
                {
                    var firstEnemyUnit =
                        stuckedLivingUnins.Where(x => x.Faction != Tick.Self.Faction)
                            .OrderBy(x => x.Life)
                            .FirstOrDefault();
                    if (firstEnemyUnit != null)
                    {
                        Strategy.AtackTarget(firstEnemyUnit);
                    }
                }

                GameState.StackedTickCount++;

                return;
            }
            GameState.StackedTickCount = 0;

            var nearestTargetPoint = GetNearestWayPointForPoint(moveToParams.TargetPoint);

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
                var angleWaypoints = AngleBetweenPoints(path[1].Position, path[0].Position,
                    new Point2D(Tick.Self.X, Tick.Self.Y));
                pathNextPoint = Math.Abs(angleWaypoints) <= Math.PI/2 ? path[1].Position : path[0].Position;
            }

            var angleToMovePoint = Tick.Self.GetAngleTo(pathNextPoint.X, pathNextPoint.Y);
            var speed = Math.Abs(angleToMovePoint) < Math.PI/2
                ? Tick.Game.WizardForwardSpeed
                : -Tick.Game.WizardBackwardSpeed;

            var strafeSpeed = angleToMovePoint > 0
                ? Tick.Game.WizardStrafeSpeed
                : -Tick.Game.WizardStrafeSpeed;

            var a = Math.Abs(Math.Sin(angleToMovePoint));
            var b = Math.Abs(Math.Cos(angleToMovePoint));
            speed = speed*b;
            strafeSpeed = strafeSpeed*a;


            var turnAngle = moveToParams.LookAtPoint != null
                ? Tick.Self.GetAngleTo(moveToParams.LookAtPoint.Value.X, moveToParams.LookAtPoint.Value.Y)
                : angleToMovePoint;


            Tick.Move.Speed = speed;
            Tick.Move.StrafeSpeed = strafeSpeed;
            Tick.Move.Turn = turnAngle;
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
            double d1 = Math.Sqrt(x1*x1 + y1*y1);
            double d2 = Math.Sqrt(x2*x2 + y2*y2);
            return Math.Acos((x1*x2 + y1*y2)/(d1*d2));
        }
    }

    public class MoveToParams
    {
        public Point2D TargetPoint { get; set; }
        public Point2D? LookAtPoint { get; set; }
    }
}
