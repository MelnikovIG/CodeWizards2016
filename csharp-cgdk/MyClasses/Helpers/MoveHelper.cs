using System;
using System.Collections.Generic;
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

            var path = WayPointsHelper.BuildWayBetweenWaypoints(getNearestMyWaypoint, nearestTargetPoint);

            //DebugTrace.ConsoleWriteLite(string.Join(" | ", path.Select(x => $"{x.Position.X} {x.Position.Y}")));

            var pathNextPoint = path[0].Position;

            if (path[0] == GameState.LastVisitedWaypoint)
            {
                pathNextPoint = path[1].Position;
            }
            else
            {
                if (path[0].Position.GetDistanceTo(myPosition) < (Tick.Self.Radius * 2) + 5)
                {
                    GameState.LastVisitedWaypoint = path[0];
                }
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

            var path = WayPointsHelper.BuildWayBetweenWaypoints(getNearestMyWaypoint, nearestTargetPoint);

            var pathNextPoint = path[0].Position;

            if (path[0] == GameState.LastVisitedWaypoint)
            {
                pathNextPoint = path[1].Position;
            }
            else
            {
                if (path[0].Position.GetDistanceTo(myPosition) < 10)
                {
                    GameState.LastVisitedWaypoint = path[0];
                }
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
    }
}
