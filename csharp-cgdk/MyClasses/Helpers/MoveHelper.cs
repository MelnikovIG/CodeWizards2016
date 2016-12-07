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
        public static void MoveTo(MoveToParams moveToParams)
        {
            var nearestTargetPoint = GetNearestWayPointForPoint(moveToParams.TargetPoint);

            var myPosition = Tick.Self.GetPositionPoint();
            var getNearestMyWaypoint = GetNearestWayPointForPoint(myPosition);

            var pathNextPoint = (Point2D) null;

            if (nearestTargetPoint == getNearestMyWaypoint)
            {
                pathNextPoint = new Point2D(moveToParams.TargetPoint.X, moveToParams.TargetPoint.Y);
            }
            else
            {
                var path = WayPointsHelper.BuildWayBetweenWaypoints(getNearestMyWaypoint, nearestTargetPoint);
                var angleWaypoints = AngleBetweenPoints(path[1].Position, path[0].Position,
                    new Point2D(Tick.Self.X, Tick.Self.Y));
                pathNextPoint = Math.Abs(angleWaypoints) <= Math.PI / 2 ? path[1].Position : path[0].Position;
            }

            var microPath = PathFindingHelper.GetPath(Tick.Self.GetPositionPoint(), pathNextPoint);
            if (microPath != null && microPath.Count > 0)
            {
                var firstMicroPathPoint = PathFindingHelper.GetCellCenterPoint(microPath[0], PathFindingHelper.gridStep);
                //Если достигли точки микропути, попробуем пойти к следующей или  если её нет пропустим её
                if (myPosition.GetDistanceTo(firstMicroPathPoint) < Tick.Self.Radius)
                {
                    if (microPath.Count > 1)
                    {
                        var secondMicroPathPoint = PathFindingHelper.GetCellCenterPoint(microPath[1], PathFindingHelper.gridStep);
                        pathNextPoint = secondMicroPathPoint;
                    }
                }
                else
                {
                    pathNextPoint = firstMicroPathPoint;
                }
            }

            var angleToTargetPoint = Tick.Self.GetAngleTo(pathNextPoint.X, pathNextPoint.Y);

            var allNearLivingUnins = UnitHelper.GetForwardStuckedLivingUnits();
            var stuckedLivingUnins = allNearLivingUnins.Where(x =>
                (Tick.Self.GetAngleTo(x) <= angleToTargetPoint + Math.PI/2) ||
                (Tick.Self.GetAngleTo(x) >= angleToTargetPoint - Math.PI/2)
                ).ToList();

            if (stuckedLivingUnins.Any())
            {
                var firstUnit = stuckedLivingUnins.First();

                var faceForward = Math.Abs(angleToTargetPoint) <= Math.PI/2;
                var angle1 = Tick.Self.GetAngleTo(firstUnit);

                var escapeSpeed = faceForward ? Tick.Game.WizardForwardSpeed: - Tick.Game.WizardBackwardSpeed; ;
                var escapeStrafe = angle1 <= 0 ? Tick.Game.WizardStrafeSpeed : -Tick.Game.WizardStrafeSpeed;

                escapeSpeed = escapeSpeed * Math.Abs(Math.Sin(angle1));
                escapeStrafe = escapeStrafe * Math.Abs(Math.Cos(angle1));

                Tick.Move.Speed = escapeSpeed;
                Tick.Move.StrafeSpeed = escapeStrafe;

                //DebugTrace.ExecuteVisualizer(() =>
                //{
                //    var endPoint = Point2D.GetPointAt(new Point2D(Tick.Self.X, Tick.Self.Y), Tick.Self.Angle + escapeAngle, 200);
                //    VisualClientHelper.BeginPost();
                //    VisualClientHelper.Line(Tick.Self.X, Tick.Self.Y, endPoint.X, endPoint.Y, 0, 1, 1);
                //    VisualClientHelper.EndPost();
                //});

                //Если застряли надолго пробуем пробить путь вперед
                if (GameState.StackedTickCount >= 30)
                {
                    var firstEnemyUnit =
                        stuckedLivingUnins.Where(x => x.Faction != Tick.Self.Faction)
                            .OrderBy(x => x.Life)
                            .FirstOrDefault();

                    if (firstEnemyUnit == null)
                    {
                        firstEnemyUnit = allNearLivingUnins.Where(x => x.Faction != Tick.Self.Faction)
                            .OrderBy(x => x.Life)
                            .FirstOrDefault();
                    }

                    if (firstEnemyUnit != null)
                    {
                        Strategy.AtackTarget(firstEnemyUnit);
                    }
                }

                GameState.StackedTickCount++;

                return;
            }
            GameState.StackedTickCount = 0;

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
                ? Tick.Self.GetAngleTo(moveToParams.LookAtPoint.X, moveToParams.LookAtPoint.Y)
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
        public Point2D LookAtPoint { get; set; }
    }
}
