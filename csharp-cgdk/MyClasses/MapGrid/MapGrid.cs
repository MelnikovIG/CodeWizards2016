//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Security.Cryptography;
//using System.Text;
//using System.Threading.Tasks;
//using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Basic;
//using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Helpers;

//namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses
//{
//    //Класс для сетки карты
//    public class MapGrid
//    {
//        //Кол-во ячеек в сетке
//        public static int GridStep = 30;
//        private static int GridSize = (int) Tick.Game.MapSize/GridStep;
//        public static int[,] Field = new int[GridSize, GridSize];

//        public static List<PointGrid> FindPath(int[,] field, PointGrid start, PointGrid goal)
//        {
//            // Шаг 1.
//            var closedSet = new Collection<PathNode>();
//            var openSet = new Collection<PathNode>();
//            // Шаг 2.
//            PathNode startNode = new PathNode()
//            {
//                Position = start,
//                CameFrom = null,
//                PathLengthFromStart = 0,
//                HeuristicEstimatePathLength = GetHeuristicPathLength(start, goal)
//            };
//            openSet.Add(startNode);
//            while (openSet.Count > 0)
//            {
//                // Шаг 3.
//                var currentNode = openSet.OrderBy(node =>
//                  node.EstimateFullPathLength).First();
//                // Шаг 4.
//                if (currentNode.Position == goal)
//                    return GetPathForNode(currentNode);
//                // Шаг 5.
//                openSet.Remove(currentNode);
//                closedSet.Add(currentNode);
//                // Шаг 6.
//                foreach (var neighbourNode in GetNeighbours(currentNode, goal, field))
//                {
//                    // Шаг 7.
//                    if (closedSet.Count(node => node.Position == neighbourNode.Position) > 0)
//                        continue;
//                    var openNode = openSet.FirstOrDefault(node =>
//                      node.Position == neighbourNode.Position);
//                    // Шаг 8.
//                    if (openNode == null)
//                        openSet.Add(neighbourNode);
//                    else
//                      if (openNode.PathLengthFromStart > neighbourNode.PathLengthFromStart)
//                    {
//                        // Шаг 9.
//                        openNode.CameFrom = currentNode;
//                        openNode.PathLengthFromStart = neighbourNode.PathLengthFromStart;
//                    }
//                }
//            }
//            // Шаг 10.
//            return null;
//        }

//        private static double GetHeuristicPathLength(PointGrid from, PointGrid to)
//        {
//            var DeltaX = Math.Abs(from.X - to.X);
//            var DeltaY = Math.Abs(from.Y - to.Y);
//            var Dist = Math.Sqrt(DeltaX * DeltaX + DeltaY * DeltaY);

//            return Dist;

//            //return Math.Sqrt(Math.Pow(from.X - to.X, 2) + Math.Pow(from.Y - to.Y,2));
//            ////return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
//        }

//        /// <summary>
//        /// 0 - можно ходить, 1 нельзя
//        /// </summary>
//        /// <param name="pathNode"></param>
//        /// <param name="goal"></param>
//        /// <param name="field"></param>
//        /// <returns></returns>
//        private static Collection<PathNode> GetNeighbours(PathNode pathNode, PointGrid goal, int[,] field)
//        {
//            var result = new Collection<PathNode>();

//            // Соседними точками являются соседние по стороне клетки.
//            var neighbourPoints = new PointGrid[]
//            {
//                new PointGrid(pathNode.Position.X + 1, pathNode.Position.Y),
//                new PointGrid(pathNode.Position.X - 1, pathNode.Position.Y),
//                new PointGrid(pathNode.Position.X, pathNode.Position.Y + 1),
//                new PointGrid(pathNode.Position.X, pathNode.Position.Y - 1),

//                new PointGrid(pathNode.Position.X + 1, pathNode.Position.Y + 1),
//                new PointGrid(pathNode.Position.X - 1, pathNode.Position.Y + 1),
//                new PointGrid(pathNode.Position.X - 1, pathNode.Position.Y + 1),
//                new PointGrid(pathNode.Position.X - 1, pathNode.Position.Y - 1),
//            };

//            foreach (var point in neighbourPoints)
//            {
//                // Проверяем, что не вышли за границы карты.
//                if (point.X < 0 || point.X >= field.GetLength(0))
//                    continue;
//                if (point.Y < 0 || point.Y >= field.GetLength(1))
//                    continue;
//                // Проверяем, что по клетке можно ходить.
//                if ((field[point.X, point.Y] != 0) /*&& (field[point.X, point.Y] != 1)*/)
//                    continue;
//                // Заполняем данные для точки маршрута.
//                var neighbourNode = new PathNode()
//                {
//                    Position = point,
//                    CameFrom = pathNode,
//                    PathLengthFromStart = pathNode.PathLengthFromStart +
//                                          GetDistanceBetweenNeighbours(),
//                    HeuristicEstimatePathLength = GetHeuristicPathLength(point, goal)
//                };
//                result.Add(neighbourNode);
//            }
//            return result;
//        }

//        private static int GetDistanceBetweenNeighbours()
//        {
//            return 1;
//        }

//        private static List<PointGrid> GetPathForNode(PathNode pathNode)
//        {
//            var result = new List<PointGrid>();
//            var currentNode = pathNode;
//            while (currentNode != null)
//            {
//                result.Add(currentNode.Position);
//                currentNode = currentNode.CameFrom;
//            }
//            result.Reverse();
//            return result;
//        }
//    }

//    public struct PointGrid
//    {

//        /// <include file='doc\Point.uex' path='docs/doc[@for="Point.Empty"]/*' />
//        /// <devdoc>
//        ///    Creates a new instance of the <see cref='System.Drawing.Point'/> class
//        ///    with member data left uninitialized.
//        /// </devdoc>
//        public static readonly PointGrid Empty = new PointGrid();

//        private int x;
//        private int y;

//        /// <include file='doc\Point.uex' path='docs/doc[@for="Point.Point"]/*' />
//        /// <devdoc>
//        ///    Initializes a new instance of the <see cref='System.Drawing.Point'/> class
//        ///    with the specified coordinates.
//        /// </devdoc>
//        public PointGrid(int x, int y)
//        {
//            this.x = x;
//            this.y = y;
//        }

//        ///// <include file='doc\Point.uex' path='docs/doc[@for="Point.Point1"]/*' />
//        ///// <devdoc>
//        /////    <para>
//        /////       Initializes a new instance of the <see cref='System.Drawing.Point'/> class
//        /////       from a <see cref='System.Drawing.Size'/> .
//        /////    </para>
//        ///// </devdoc>
//        //public Point(Size sz)
//        //{
//        //    this.x = sz.Width;
//        //    this.y = sz.Height;
//        //}

//        ///// <include file='doc\Point.uex' path='docs/doc[@for="Point.Point2"]/*' />
//        ///// <devdoc>
//        /////    Initializes a new instance of the Point class using
//        /////    coordinates specified by an integer value.
//        ///// </devdoc>
//        //public Point(int dw)
//        //{
//        //    unchecked
//        //    {
//        //        this.x = (short)LOWORD(dw);
//        //        this.y = (short)HIWORD(dw);
//        //    }
//        //}

//        ///// <include file='doc\Point.uex' path='docs/doc[@for="Point.IsEmpty"]/*' />
//        ///// <devdoc>
//        /////    <para>
//        /////       Gets a value indicating whether this <see cref='System.Drawing.Point'/> is empty.
//        /////    </para>
//        ///// </devdoc>
//        //[Browsable(false)]
//        //public bool IsEmpty
//        //{
//        //    get
//        //    {
//        //        return x == 0 && y == 0;
//        //    }
//        //}

//        /// <include file='doc\Point.uex' path='docs/doc[@for="Point.X"]/*' />
//        /// <devdoc>
//        ///    Gets the x-coordinate of this <see cref='System.Drawing.Point'/>.
//        /// </devdoc>
//        public int X
//        {
//            get
//            {
//                return x;
//            }
//            set
//            {
//                x = value;
//            }
//        }

//        /// <include file='doc\Point.uex' path='docs/doc[@for="Point.Y"]/*' />
//        /// <devdoc>
//        ///    <para>
//        ///       Gets the y-coordinate of this <see cref='System.Drawing.Point'/>.
//        ///    </para>
//        /// </devdoc>
//        public int Y
//        {
//            get
//            {
//                return y;
//            }
//            set
//            {
//                y = value;
//            }
//        }

//        ///// <include file='doc\Point.uex' path='docs/doc[@for="Point.operatorPointF"]/*' />
//        ///// <devdoc>
//        /////    <para>
//        /////       Creates a <see cref='System.Drawing.PointF'/> with the coordinates of the specified
//        /////    <see cref='System.Drawing.Point'/> 
//        /////    .
//        ///// </para>
//        ///// </devdoc>
//        //public static implicit operator PointF(Point p)
//        //{
//        //    return new PointF(p.X, p.Y);
//        //}

//        ///// <include file='doc\Point.uex' path='docs/doc[@for="Point.operatorSize"]/*' />
//        ///// <devdoc>
//        /////    <para>
//        /////       Creates a <see cref='System.Drawing.Size'/> with the coordinates of the specified <see cref='System.Drawing.Point'/> .
//        /////    </para>
//        ///// </devdoc>
//        //public static explicit operator Size(Point p)
//        //{
//        //    return new Size(p.X, p.Y);
//        //}

//        ///// <include file='doc\Point.uex' path='docs/doc[@for="Point.operator+"]/*' />
//        ///// <devdoc>
//        /////    <para>
//        /////       Translates a <see cref='System.Drawing.Point'/> by a given <see cref='System.Drawing.Size'/> .
//        /////    </para>
//        ///// </devdoc>        
//        //public static Point operator +(Point pt, Size sz)
//        //{
//        //    return Add(pt, sz);
//        //}

//        ///// <include file='doc\Point.uex' path='docs/doc[@for="Point.operator-"]/*' />
//        ///// <devdoc>
//        /////    <para>
//        /////       Translates a <see cref='System.Drawing.Point'/> by the negative of a given <see cref='System.Drawing.Size'/> .
//        /////    </para>
//        ///// </devdoc>        
//        //public static Point operator -(Point pt, Size sz)
//        //{
//        //    return Subtract(pt, sz);
//        //}

//        /// <include file='doc\Point.uex' path='docs/doc[@for="Point.operator=="]/*' />
//        /// <devdoc>
//        ///    <para>
//        ///       Compares two <see cref='System.Drawing.Point'/> objects. The result specifies
//        ///       whether the values of the <see cref='System.Drawing.Point.X'/> and <see cref='System.Drawing.Point.Y'/> properties of the two <see cref='System.Drawing.Point'/>
//        ///       objects are equal.
//        ///    </para>
//        /// </devdoc>
//        public static bool operator ==(PointGrid left, PointGrid right)
//        {
//            return left.X == right.X && left.Y == right.Y;
//        }

//        /// <include file='doc\Point.uex' path='docs/doc[@for="Point.operator!="]/*' />
//        /// <devdoc>
//        ///    <para>
//        ///       Compares two <see cref='System.Drawing.Point'/> objects. The result specifies whether the values
//        ///       of the <see cref='System.Drawing.Point.X'/> or <see cref='System.Drawing.Point.Y'/> properties of the two
//        ///    <see cref='System.Drawing.Point'/> 
//        ///    objects are unequal.
//        /// </para>
//        /// </devdoc>
//        public static bool operator !=(PointGrid left, PointGrid right)
//        {
//            return !(left == right);
//        }

//        ///// <devdoc>
//        /////    <para>
//        /////       Translates a <see cref='System.Drawing.Point'/> by a given <see cref='System.Drawing.Size'/> .
//        /////    </para>
//        ///// </devdoc>        
//        //public static Point Add(Point pt, Size sz)
//        //{
//        //    return new Point(pt.X + sz.Width, pt.Y + sz.Height);
//        //}

//        ///// <devdoc>
//        /////    <para>
//        /////       Translates a <see cref='System.Drawing.Point'/> by the negative of a given <see cref='System.Drawing.Size'/> .
//        /////    </para>
//        ///// </devdoc>        
//        //public static Point Subtract(Point pt, Size sz)
//        //{
//        //    return new Point(pt.X - sz.Width, pt.Y - sz.Height);
//        //}

//        ///// <include file='doc\Point.uex' path='docs/doc[@for="Point.Ceiling"]/*' />
//        ///// <devdoc>
//        /////   Converts a PointF to a Point by performing a ceiling operation on
//        /////   all the coordinates.
//        ///// </devdoc>
//        //public static Point Ceiling(PointF value)
//        //{
//        //    return new Point((int)Math.Ceiling(value.X), (int)Math.Ceiling(value.Y));
//        //}

//        ///// <include file='doc\Point.uex' path='docs/doc[@for="Point.Truncate"]/*' />
//        ///// <devdoc>
//        /////   Converts a PointF to a Point by performing a truncate operation on
//        /////   all the coordinates.
//        ///// </devdoc>
//        //public static Point Truncate(PointF value)
//        //{
//        //    return new Point((int)value.X, (int)value.Y);
//        //}

//        ///// <include file='doc\Point.uex' path='docs/doc[@for="Point.Round"]/*' />
//        ///// <devdoc>
//        /////   Converts a PointF to a Point by performing a round operation on
//        /////   all the coordinates.
//        ///// </devdoc>
//        //public static Point Round(PointF value)
//        //{
//        //    return new Point((int)Math.Round(value.X), (int)Math.Round(value.Y));
//        //}


//        /// <include file='doc\Point.uex' path='docs/doc[@for="Point.Equals"]/*' />
//        /// <devdoc>
//        ///    <para>
//        ///       Specifies whether this <see cref='System.Drawing.Point'/> contains
//        ///       the same coordinates as the specified <see cref='System.Object'/>.
//        ///    </para>
//        /// </devdoc>
//        public override bool Equals(object obj)
//        {
//            if (!(obj is PointGrid)) return false;
//            PointGrid comp = (PointGrid)obj;
//            // Note value types can't have derived classes, so we don't need 
//            // to check the types of the objects here.  -- Microsoft, 2/21/2001
//            return comp.X == this.X && comp.Y == this.Y;
//        }

//        /// <include file='doc\Point.uex' path='docs/doc[@for="Point.GetHashCode"]/*' />
//        /// <devdoc>
//        ///    <para>
//        ///       Returns a hash code.
//        ///    </para>
//        /// </devdoc>
//        public override int GetHashCode()
//        {
//            return unchecked(x ^ y);
//        }

//        ///**
//        // * Offset the current Point object by the given amount
//        // */
//        ///// <include file='doc\Point.uex' path='docs/doc[@for="Point.Offset"]/*' />
//        ///// <devdoc>
//        /////    Translates this <see cref='System.Drawing.Point'/> by the specified amount.
//        ///// </devdoc>
//        //public void Offset(int dx, int dy)
//        //{
//        //    X += dx;
//        //    Y += dy;
//        //}

//        ///// <include file='doc\Point.uex' path='docs/doc[@for="Point.Offset2"]/*' />
//        ///// <devdoc>
//        /////    Translates this <see cref='System.Drawing.Point'/> by the specified amount.
//        ///// </devdoc>
//        //public void Offset(Point p)
//        //{
//        //    Offset(p.X, p.Y);
//        //}

//        ///// <include file='doc\Point.uex' path='docs/doc[@for="Point.ToString"]/*' />
//        ///// <devdoc>
//        /////    <para>
//        /////       Converts this <see cref='System.Drawing.Point'/>
//        /////       to a human readable
//        /////       string.
//        /////    </para>
//        ///// </devdoc>
//        //public override string ToString()
//        //{
//        //    return "{X=" + X.ToString(CultureInfo.CurrentCulture) + ",Y=" + Y.ToString(CultureInfo.CurrentCulture) + "}";
//        //}

//        //private static int HIWORD(int n)
//        //{
//        //    return (n >> 16) & 0xffff;
//        //}

//        //private static int LOWORD(int n)
//        //{
//        //    return n & 0xffff;
//        //}
//    }

//    //public class PointGrid
//    //{
//    //    public PointGrid(int x, int y)
//    //    {
//    //        X = x;
//    //        Y = y;
//    //    }

//    //    public static bool operator ==(PointGrid a, PointGrid b)
//    //    {
//    //        // If both are null, or both are same instance, return true.
//    //        if (System.Object.ReferenceEquals(a, b))
//    //        {
//    //            return true;
//    //        }

//    //        // If one is null, but not both, return false.
//    //        if (((object)a == null) || ((object)b == null))
//    //        {
//    //            return false;
//    //        }

//    //        // Return true if the fields match:
//    //        return a.X == b.X && a.Y == b.Y;
//    //    }

//    //    public static bool operator !=(PointGrid a, PointGrid b)
//    //    {
//    //        return !(a == b);
//    //    }

//    //    public override bool Equals(object obj)
//    //    {
//    //        return obj is PointGrid && ((PointGrid) obj).X == X && ((PointGrid) obj).Y == Y;
//    //    }

//    //    public override int GetHashCode()
//    //    {
//    //        return X.GetHashCode() ^ Y.GetHashCode();
//    //    }

//    //    public int X { get; set; }
//    //    public int Y { get; set; }
//    //}

//    public class PathNode
//    {
//        // Координаты точки на карте.
//        public PointGrid Position { get; set; }
//        // Длина пути от старта (G).
//        public int PathLengthFromStart { get; set; }
//        // Точка, из которой пришли в эту точку.
//        public PathNode CameFrom { get; set; }
//        // Примерное расстояние до цели (H).
//        public double HeuristicEstimatePathLength { get; set; }
//        // Ожидаемое полное расстояние до цели (F).
//        public double EstimateFullPathLength => this.PathLengthFromStart + this.HeuristicEstimatePathLength;
//    }
//}
