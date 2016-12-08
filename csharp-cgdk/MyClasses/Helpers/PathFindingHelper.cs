using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Basic;
using EpPathFinding.cs;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Helpers
{
    public static class PathFindingHelper
    {
        public static int gridStep = (int)20;
        public static int gridSize = (int)Tick.Game.MapSize / gridStep;

        private static bool[][] _movableMatrix;

        public static bool[][] MovableMatrix
        {
            get
            {
                if (_movableMatrix == null)
                {
                    _movableMatrix = new bool[gridSize][];
                    for (int widthTrav = 0; widthTrav < gridSize; widthTrav++)
                    {
                        _movableMatrix[widthTrav] = new bool[gridSize];
                    }
                }
                return _movableMatrix;
            }
        }

        public static List<GridPos> GetPath(Point2D start, Point2D end)
        {
            for (int i = 0; i < MovableMatrix.Length; i++)
            {
                var row = MovableMatrix[i];
                for (int index = 0; index < row.Length; index++)
                {
                    MovableMatrix[i][index] = true;
                }
            }

            List<CircularUnit> circularUnits = new List<CircularUnit>(Tick.World.Minions);
            circularUnits.AddRange(Tick.World.Buildings);
            circularUnits.AddRange(Tick.World.Wizards.Where(x => !x.IsMe));
            circularUnits.AddRange(Tick.World.Trees);
            circularUnits = circularUnits.Where(x => Tick.Self.GetDistanceTo(x) < Tick.Self.VisionRange).ToList();
            foreach (var nearestMinion in circularUnits)
            {
                var p1 = (int)nearestMinion.X / gridStep;
                var p2 = (int)nearestMinion.Y / gridStep;
                FillMovableCircle(p1, p2, (int)(nearestMinion.Radius + Tick.Self.Radius) / gridStep, gridSize, MovableMatrix, false);
            }

            var startGridPos = GetGridPosByPoint2d(start, gridStep);
            var endGridPos = GetGridPosByPoint2d(end, gridStep);

            MovableMatrix[startGridPos.x][startGridPos.y] = true;
            FillMovableCircle(endGridPos.x, endGridPos.y, (int)(Tick.Self.Radius * 2) / gridStep, gridSize, MovableMatrix, true);

            BaseGrid searchGrid = new StaticGrid(gridSize, gridSize, MovableMatrix);

            JumpPointParam jpParam = new JumpPointParam(searchGrid, startGridPos, endGridPos);
            var result = JumpPointFinder.FindPath(jpParam);
            //result = JumpPointFinder.GetFullPath(result);

            var clmnIdx = 0;
            foreach (var row in MovableMatrix)
            {
                var rowIdx = 0;
                foreach (var b in row)
                {
                    if (!b)
                    {
                        VisualClientHelper.Rect(new Point2D(clmnIdx * gridStep, rowIdx * gridStep),
                            new Point2D((clmnIdx + 1) * gridStep, (rowIdx + 1) * gridStep), new VisualClientColor(0, 0, 1));
                    }

                    rowIdx++;
                }

                clmnIdx++;
            }

            DrawPath(result, gridStep);

            return result;
        }

        public static GridPos GetGridPosByPoint2d(Point2D point, int step)
        {
            return new GridPos((int)point.X / step, (int)point.Y / step);
        }

        public static Point2D GetCellCenterPoint(GridPos point, int step)
        {
            return new Point2D(point.x * step + step / 2, point.y * step + step / 2);
        }

        public static void DrawPath(List<GridPos> path, int step)
        {
            path.ForEach(
                x =>
                {
                    var p1 = x.x;
                    var p2 = x.y;
                    VisualClientHelper.Rect(new Point2D(p1 * step, p2 * step), new Point2D((p1 + 1) * step, (p2 + 1) * step), new VisualClientColor(0, 1, 1));
                }
                );
        }

        //public static void DrawWaypoints()
        //{
        //    WayPointsHelper.GetWayPoints().ForEach(x =>
        //    {
        //        var p1 = (int)x.Position.X / GridStep;
        //        var p2 = (int)x.Position.Y / GridStep;
        //        VisualClient.Instance.FillRect(p1 * GridStep, p2 * GridStep, (p1 + 1) * GridStep, (p2 + 1) * GridStep, 0, 1, 1);
        //    });
        //}

        //private static void DrawCircle(int x0, int y0, int radius)
        //{
        //    int x = radius;
        //    int y = 0;
        //    int err = 0;

        //    while (x >= y)
        //    {
        //        SetCell(x0 + x, y0 + y);
        //        SetCell(x0 + y, y0 + x);
        //        SetCell(x0 - y, y0 + x);
        //        SetCell(x0 - x, y0 + y);
        //        SetCell(x0 - x, y0 - y);
        //        SetCell(x0 - y, y0 - x);
        //        SetCell(x0 + y, y0 - x);
        //        SetCell(x0 + x, y0 - y);

        //        y += 1;
        //        err += 1 + 2 * y;
        //        if (2 * (err - x) + 1 > 0)
        //        {
        //            x -= 1;
        //            err += 1 - 2 * x;
        //        }
        //    }
        //}

        private static void FillMovableCircle(int x0, int y0, int radius, int size, bool[][] movableMatrix, bool moveable)
        {
            int x = radius;
            int y = 0;
            int xChange = 1 - (radius << 1);
            int yChange = 0;
            int radiusError = 0;

            while (x >= y)
            {
                for (int i = x0 - x; i <= x0 + x; i++)
                {
                    SetCell(i, y0 + y, size, movableMatrix, moveable);
                    SetCell(i, y0 - y, size, movableMatrix, moveable);
                }
                for (int i = x0 - y; i <= x0 + y; i++)
                {
                    SetCell(i, y0 + x, size, movableMatrix, moveable);
                    SetCell(i, y0 - x, size, movableMatrix, moveable);
                }

                y++;
                radiusError += yChange;
                yChange += 2;
                if (((radiusError << 1) + xChange) > 0)
                {
                    x--;
                    radiusError += xChange;
                    xChange += 2;
                }
            }
        }

        private static void SetCell(int x, int y, int size, bool[][] movableMatrix, bool moveable)
        {
            if (x < 0 || y < 0 || x > size - 1 || y > size - 1)
                return;

            movableMatrix[x][y] = moveable;
        }
    }
}
