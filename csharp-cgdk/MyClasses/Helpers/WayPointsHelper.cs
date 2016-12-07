using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Basic;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Helpers
{
    public static class WayPointsHelper
    {
        private static bool IsWaypointsBuilt { get; set; }
        private static List<WayPoint> AllWaypoints { get; set; } = new List<WayPoint>();

        public static List<WayPoint> GetWayPoints()
        {
            if (!IsWaypointsBuilt)
            {
                var myLaneType = GameState.MyLaneType ?? GetDefaultLineType();
                BuildWaypoints(myLaneType);
            }
            return AllWaypoints;
        }

        private static LaneType GetDefaultLineType()
        {
            var lane = LaneType.Middle;

            switch ((int)Tick.Self.Id)
            {
                case 1:
                case 2:
                case 6:
                case 7:
                    lane = LaneType.Top;
                    break;
                case 3:
                case 8:
                    lane = LaneType.Middle;
                    break;
                case 4:
                case 5:
                case 9:
                case 10:
                    lane = LaneType.Bottom;
                    break;
                default:
                    break;
            }

            return lane;
        }

        public static void BuildWaypoints(LaneType laneType)
        {
            IsWaypointsBuilt = true;

            //var waypointCoordinates = TestAllBotWaypoints();
            //var waypointCoordinates = TestAllMidWaypoints();
            //var waypointCoordinates = TestAllTopWaypoints();
            //var waypointCoordinates = TestCenterTop();
            var waypointCoordinates = GetWaypoints(laneType);

            var waypoints = BuildLinkedWaypoints(waypointCoordinates);
            AllWaypoints.AddRange(waypoints);
        }

        public static List<Point2D> TestCenterTop()
        {
            var mapSize = Tick.Game.MapSize;
            return new List<Point2D>
                {
                    new Point2D(0,0),
                    new Point2D(mapSize/2.0,0),
                };
        }

        public static List<Point2D> TestAllTopWaypoints()
        {
            var mapSize = Tick.Game.MapSize;
            return new List<Point2D>
                {
                    new Point2D(100.0D, mapSize - 100.0D),
                    new Point2D(100.0D, mapSize - 400.0D),
                    new Point2D(200.0D, mapSize - 800.0D),
                    new Point2D(200.0D, mapSize*0.75D),
                    new Point2D(200.0D, mapSize*0.5D),
                    new Point2D(200.0D, mapSize*0.25D),
                    new Point2D(500.0D, 500.0D), //LEFT TOP POINT
                    new Point2D(mapSize*0.25D, 200.0D),
                    new Point2D(mapSize*0.5D, 200.0D),
                    new Point2D(mapSize*0.75D, 200.0D),
                    new Point2D(mapSize - 200.0D, 200.0D)
                };
        }

        public static List<Point2D> TestAllMidWaypoints()
        {
            var mapSize = Tick.Game.MapSize;
            return new List<Point2D>
                {
                    new Point2D(100.0D, mapSize - 100.0D),
                    /*random.nextBoolean() ? new Point2D(600.0D, mapSize - 200.0D) : */
                    new Point2D(200.0D, mapSize - 600.0D),
                    new Point2D(800.0D, mapSize - 800.0D),
                    new Point2D(mapSize - 600.0D, 600.0D)
                };
        }

        public static List<Point2D> TestAllBotWaypoints()
        {
            var mapSize = Tick.Game.MapSize;
            return new List<Point2D>
                {
                    new Point2D(100.0D, mapSize - 100.0D),
                    new Point2D(400.0D, mapSize - 100.0D),
                    new Point2D(800.0D, mapSize - 200.0D),
                    new Point2D(mapSize * 0.25D, mapSize - 200.0D),
                    new Point2D(mapSize * 0.5D, mapSize - 200.0D),
                    new Point2D(mapSize * 0.75D, mapSize - 200.0D),
                    new Point2D(mapSize - 500.0D, mapSize - 500.0D), //RIGHT BOT POINT
                    new Point2D(mapSize - 200.0D, mapSize * 0.75D),
                    new Point2D(mapSize - 200.0D, mapSize * 0.5D),
                    new Point2D(mapSize - 200.0D, mapSize * 0.25D),
                    new Point2D(mapSize - 200.0D, 200.0D)
                };
        }

        private static List<Point2D> GetWaypoints(LaneType laneType)
        {
            var mapSize = Tick.Game.MapSize;

            var waypointCoordinates = new List<Point2D>();

            if (laneType == LaneType.Top)
            {
                waypointCoordinates.AddRange(new[]
                {
                    new Point2D(100.0D, mapSize - 100.0D),
                    new Point2D(100.0D, mapSize - 400.0D),
                    new Point2D(200.0D, mapSize - 800.0D),
                    new Point2D(200.0D, mapSize*0.75D),
                    new Point2D(200.0D, mapSize*0.5D),
                    new Point2D(200.0D, mapSize*0.25D),
                    new Point2D(500.0D, 500.0D), //LEFT TOP POINT
                    new Point2D(mapSize*0.25D, 200.0D),
                    new Point2D(mapSize*0.5D, 200.0D),
                    new Point2D(mapSize*0.75D, 200.0D),
                    new Point2D(mapSize - 200.0D, 200.0D)
                });
            }
            else if (laneType == LaneType.Middle)
            {
                waypointCoordinates.AddRange(new[]
                {
                    new Point2D(100.0D, mapSize - 100.0D),
                    /*random.nextBoolean() ? new Point2D(600.0D, mapSize - 200.0D) : */
                    new Point2D(200.0D, mapSize - 600.0D),
                    new Point2D(800.0D, mapSize - 800.0D),
                    new Point2D(1400.0D, mapSize - 1400.0D),
                    new Point2D(2000.0D, mapSize - 2000.0D),
                    new Point2D(2600.0D, mapSize - 2600.0D),
                    new Point2D(3200.0D, mapSize - 3200.0D),
                    new Point2D(mapSize - 600.0D, 600.0D)
                });
            }
            else if (laneType == LaneType.Bottom)
            {
                waypointCoordinates.AddRange(new[]
                {
                    new Point2D(100.0D, mapSize - 100.0D),
                    new Point2D(400.0D, mapSize - 100.0D),
                    new Point2D(800.0D, mapSize - 200.0D),
                    new Point2D(mapSize * 0.25D, mapSize - 200.0D),
                    new Point2D(mapSize * 0.5D, mapSize - 200.0D),
                    new Point2D(mapSize * 0.75D, mapSize - 200.0D),
                    new Point2D(mapSize - 500.0D, mapSize - 500.0D), //RIGHT BOT POINT
                    new Point2D(mapSize - 200.0D, mapSize * 0.75D),
                    new Point2D(mapSize - 200.0D, mapSize * 0.5D),
                    new Point2D(mapSize - 200.0D, mapSize * 0.25D),
                    new Point2D(mapSize - 200.0D, 200.0D)
                });
            }
            return waypointCoordinates;
        } 

        public static List<WayPoint> BuildLinkedWaypoints(List<Point2D> wayPointCoordinates)
        {
            var wayPoints = wayPointCoordinates.Select(x => new WayPoint() {Position = x}).ToList();
            var result = new List<WayPoint>(wayPointCoordinates.Count);
            result.AddRange(wayPoints);

            for (int i = 0; i < wayPoints.Count - 1; i++)
            {
                LinkWayPoints(wayPoints[i], wayPoints[i+1]);
            }

            return result;
        }

        private static void LinkWayPoints(WayPoint a, WayPoint b)
        {
            a.LinkedWayPoints.Add(b);
            b.LinkedWayPoints.Add(a);
        }

        public static List<WayPoint> BuildWayBetweenWaypoints(WayPoint from, WayPoint to)
        {
            AllWaypoints.ForEach(x =>
            {
                x.LeftScanned = false;
                x.RightScanned = false;
            });

            List<List<WayPoint>> leftPathes = new List<List<WayPoint>>() {new List<WayPoint>() {from}};
            List<List<WayPoint>> rightPathes = new List<List<WayPoint>>() {new List<WayPoint>() {to}};

            while (true)
            {
                var newLeftPathes = new List<List<WayPoint>>();

                foreach (var leftPath in leftPathes)
                {
                    var lastLeftPathWaypoint = leftPath.Last();

                    if (lastLeftPathWaypoint.RightScanned)
                    {
                        var rightPath = rightPathes.First(x => x.Last() == lastLeftPathWaypoint);
                        rightPath.Reverse();
                        leftPath.RemoveAt(leftPath.Count - 1);
                        leftPath.AddRange(rightPath);
                        return leftPath;
                    }

                    var pathConnectWaypoints = lastLeftPathWaypoint.LinkedWayPoints.Where(x => x.RightScanned).ToList();
                    if (pathConnectWaypoints.Any())
                    {
                        var pathConnectWaypoint = pathConnectWaypoints.FirstOrDefault();
                        var rightPath = rightPathes.First(x => x.Last() == pathConnectWaypoint);
                        rightPath.Reverse();
                        leftPath.AddRange(rightPath);
                        return leftPath;
                    }

                    foreach (var linked in lastLeftPathWaypoint.LinkedWayPoints.Where(x => !x.LeftScanned))
                    {
                        var newLeftPath = new List<WayPoint>(leftPath) {linked};
                        linked.LeftScanned = true;
                        newLeftPathes.Add(newLeftPath);
                    }
                }

                leftPathes = newLeftPathes;

                var newRightPathes = new List<List<WayPoint>>();

                foreach (var rightPath in rightPathes)
                {
                    var lastRightPathWaypoint = rightPath.Last();
                    if (lastRightPathWaypoint.LeftScanned)
                    {
                        var leftPath = leftPathes.First(x => x.Last() == lastRightPathWaypoint);
                        rightPath.Reverse();
                        rightPath.RemoveAt(0);
                        leftPath.AddRange(rightPath);
                        return leftPath;
                    }

                    var pathConnectWaypoints = lastRightPathWaypoint.LinkedWayPoints.Where(x => x.LeftScanned).ToList();
                    if (pathConnectWaypoints.Any())
                    {
                        var pathConnectWaypoint = pathConnectWaypoints.FirstOrDefault();
                        var leftPath = leftPathes.First(x => x.Last() == pathConnectWaypoint);
                        rightPath.Reverse();
                        leftPath.AddRange(rightPath);
                        return leftPath;
                    }

                    foreach (var linked in lastRightPathWaypoint.LinkedWayPoints.Where(x => !x.RightScanned))
                    {
                        var newRightPath = new List<WayPoint>(rightPath) {linked};
                        linked.RightScanned = true;
                        newRightPathes.Add(newRightPath);
                    }
                }

                rightPathes = newRightPathes;
            }
        }
    }

    public class WayPoint
    {
        public List<WayPoint> LinkedWayPoints { get; set; } = new List<WayPoint>();
        public Point2D Position { get; set; }

        public bool LeftScanned { get; set; }
        public bool RightScanned { get; set; }
    }
}
