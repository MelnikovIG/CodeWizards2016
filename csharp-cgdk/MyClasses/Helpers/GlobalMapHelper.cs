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
    public static class GlobalMapHelper
    {
        public static int gridStep = (int)400;
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

                    FillMatrix();
                }
                return _movableMatrix;
            }
        }

        public static void FillMatrix()
        {
            var laneType = GameState.MyLaneType ?? LaneType.Middle;

            if (laneType == LaneType.Middle)
            {
                for (int i = 0; i < gridSize; i++)
                {
                    MovableMatrix[i][gridSize - i - 1] = true;
                }
            }
            else if (laneType == LaneType.Top)
            {
                for (int i = 0; i < gridSize; i++)
                {
                    for (int j = 0; j < gridSize; j++)
                    {
                        if (i == 0 || j == 0)
                        {
                            MovableMatrix[i][j] = true;
                        }
                    }
                }
            }
            else if (laneType == LaneType.Bottom)
            {
                for (int i = 0; i < gridSize; i++)
                {
                    for (int j = 0; j < gridSize; j++)
                    {
                        if (i == gridSize-1 || j == gridSize - 1)
                        {
                            MovableMatrix[i][j] = true;
                        }
                    }
                }
            }
        }

        public static List<GridPos> GetPath(Point2D start, Point2D end)
        {
            BaseGrid searchGrid = new StaticGrid(gridSize, gridSize, MovableMatrix);

            var startGridPos = GetGridPosByPoint2d(start, gridStep);
            var endGridPos = GetGridPosByPoint2d(end, gridStep);


            JumpPointParam jpParam = new JumpPointParam(searchGrid, startGridPos, endGridPos);
            var result = JumpPointFinder.FindPath(jpParam);
            result = JumpPointFinder.GetFullPath(result);

            DrawPath(result, gridStep);

            return result;
        }


        public static void DrawPath(List<GridPos> path, int step)
        {
            //path.ForEach(
            //    x =>
            //    {
            //        var p1 = x.x;
            //        var p2 = x.y;
            //        VisualClientHelper.Rect(new Point2D(p1*step, p2*step), new Point2D((p1 + 1)*step, (p2 + 1)*step),
            //            new VisualClientColor(0, 1, 1));
            //    }
            //    );
        }

        public static GridPos GetGridPosByPoint2d(Point2D point, int step)
        {
            return new GridPos((int)point.X / step, (int)point.Y / step);
        }

        public static Point2D GetCellCenterPoint(GridPos point, int step)
        {
            return new Point2D(point.x * step + step / 2, point.y * step + step / 2);
        }
    }
}
