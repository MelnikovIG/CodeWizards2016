//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Model;
//using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Basic;

//namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Helpers
//{
//    public static class ProjectilesHelper
//    {
//        //Найти снаряды летящие в меня
//        public static void DetectToMeProjectiles()
//        {
//            //var interselectedProjectiles = GameState.ProjectilesInfo.Where(x => IsInterselect(x.Value.StartPoint, x.Value.EndPoint, new Point2D(Tick.Self.X, Tick.Self.Y), Tick.Self.Radius)).ToList();
//            //if (interselectedProjectiles.Any())
//            //{
//            //    var first = interselectedProjectiles.FirstOrDefault();
//            //    //Tick.Move.Turn = Math.PI/2;
//            //    Tick.Move.Speed = -Tick.Game.WizardBackwardSpeed;
//            //    //Tick.Move.StrafeSpeed = -Tick.Game.WizardStrafeSpeed;
//            //}
//            //else
//            //{
//            //    var a = 0;
//            //}
//        }

//        //public static List<Projectile> GetNearProjectiles(double range)
//        //{
//        //    var allyWizards = Tick.World.Wizards.Where(x => x.Faction == Tick.Self.Faction);
//        //    var allyWizardsIds = allyWizards.Select(x => x.OwnerPlayerId).ToList();

//        //    var nearProjectiles = Tick.World.Projectiles
//        //        .Where(x => Tick.Self.GetDistanceTo(x) < range)
//        //        .Where(x => !allyWizardsIds.Contains(x.OwnerPlayerId))
//        //        .Where(x => x.Type == ProjectileType.MagicMissile)
//        //        .Where(x => IsInterselect(x, Tick.Self.CastRange, Tick.Self))
//        //        .ToList();

//        //    if (nearProjectiles.Any())
//        //    {
//        //        var firsProjectile = nearProjectiles.First();

//        //        var startX = Tick.Self.X;
//        //        var startY = Tick.Self.Y;
//        //        var endX = startX + 10 * Math.Cos(firsProjectile.Angle + Math.PI/2);
//        //        var endY = startY + 10 * Math.Sin(firsProjectile.Angle + Math.PI / 2);

//        //        //Tick.Move.StrafeSpeed = 30;
//        //        ////Tick.Move.Speed = -30;
//        //        ////MoveHelper.MoveTo(new MoveToParams()
//        //        ////{
//        //        ////    TargetPoint = new Point2D(endX, endY)
//        //        ////});
//        //    }

//        //    return nearProjectiles;
//        //}

//        public static bool IsInterselect(Point2D p1, Point2D p2, Point2D pCircle, double radius)
//        {
//            Point2D intersection1;
//            Point2D intersection2;
//            int intersections = FindLineCircleIntersections(pCircle.X, pCircle.Y, radius, p1, p2, out intersection1, out intersection2);

//            if (intersections == 1)
//                //return intersection1;
//                return true;

//            if (intersections == 2)
//            {
//                return true;

//                //double dist1 = Distance(intersection1, p1);
//                //double dist2 = Distance(intersection2, p1);

//                //if (dist1 < dist2)
//                //    return intersection1;
//                //else
//                //    return intersection2;
//            }

//            return false;
//        }

 
//        private static double Distance(Point2D p1, Point2D p2)
//        {
//            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
//        }

//        // Find the points of intersection.
//        private static int FindLineCircleIntersections(double cx, double cy, double radius,
//            Point2D point1, Point2D point2, out Point2D intersection1, out Point2D intersection2)
//        {
//            double dx, dy, A, B, C, det, t;

//            dx = point2.X - point1.X;
//            dy = point2.Y - point1.Y;

//            A = dx * dx + dy * dy;
//            B = 2 * (dx * (point1.X - cx) + dy * (point1.Y - cy));
//            C = (point1.X - cx) * (point1.X - cx) + (point1.Y - cy) * (point1.Y - cy) - radius * radius;

//            det = B * B - 4 * A * C;
//            if ((A <= 0.0000001) || (det < 0))
//            {
//                // No real solutions.
//                intersection1 = new Point2D(float.NaN, float.NaN);
//                intersection2 = new Point2D(float.NaN, float.NaN);
//                return 0;
//            }
//            else if (det == 0)
//            {
//                // One solution.
//                t = -B / (2 * A);
//                intersection1 = new Point2D(point1.X + t * dx, point1.Y + t * dy);
//                intersection2 = new Point2D(float.NaN, float.NaN);
//                return 1;
//            }
//            else
//            {
//                // Two solutions.
//                t = (float)((-B + Math.Sqrt(det)) / (2 * A));
//                intersection1 = new Point2D(point1.X + t * dx, point1.Y + t * dy);
//                t = (float)((-B - Math.Sqrt(det)) / (2 * A));
//                intersection2 = new Point2D(point1.X + t * dx, point1.Y + t * dy);
//                return 2;
//            }
//        }
//    }
//}
