using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Basic;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Helpers
{
    public static class ProjectilesHelper
    {
        ////Получить вражеские снаряды, пересекающие героя
        //public static List<ProjectilesInfo> GetInterselecitionsProjectiles()
        //{
        //    FillNearProjectilesInfo(2000);

        //    var interselectedProjectiles = GameState.ProjectilesInfo.Where(x => x.Value.IsInterselect).ToList();
        //    return interselectedProjectiles.Select(x => x.Value).ToList();
        //}

        /// <summary>
        /// Получить снаряды от которых можно увернуться :)
        /// </summary>
        /// <returns></returns>
        public static List<ProjectilesInfo> GetEvideableProjectiles()
        {
            FillNearProjectilesInfo(2000);
            var evadeableProjectiles =
                GameState.ProjectilesInfo.Where(x => x.Value.IsInDangerZone && x.Value.CanEvade).ToList();
            return evadeableProjectiles.Select(x => x.Value).ToList();
        } 

        private static void FillNearProjectilesInfo(double distance)
        {
            var selfPoint = new Point2D(Tick.Self.X, Tick.Self.Y);

            //Будем анализировать снаряды, леящие рядом
            var nearProjectiles =
                GameState.ProjectilesInfo.Where(
                    x => Tick.Self.GetDistanceTo(x.Value.CurrentPoint.X, x.Value.CurrentPoint.Y) < distance).ToList();

            //Доп расстояние до опасной зоны, при которой нужно уворавчивтаься на всякй случай
            var additionalDangerZone = 5;

            foreach (var projectileInfoPair in nearProjectiles)
            {
                bool isInDangerZone = false;
                bool canEvade = true;
                Vector evadeVector = new Vector();

                var projectileInfo = projectileInfoPair.Value;

                var v1 = new Vector(Tick.Self.GetPositionPoint()) - new Vector(projectileInfo.EndPoint);
                var v2 = new Vector(projectileInfo.StartPoint) - new Vector(projectileInfo.EndPoint);
                var angleBetweenVectors = Math.Abs(Vector.AngleBetweenInRadians(v1, v2));

                //DebugTrace.ConsoleWriteLite($"angleBetweenVectors {angleBetweenVectors.ToString("N3")}");
                //VisualClientHelper.Line(projectileInfo.StartPoint, projectileInfo.EndPoint, 0, 0, 1);

                if (angleBetweenVectors < Math.PI/2)
                {
                    Point2D normalInterselectionPoint;
                    var isInterselectByNormal = IsInterselectByNormal(projectileInfo, out normalInterselectionPoint);
                    if (isInterselectByNormal)
                    {
                        isInDangerZone = selfPoint.GetDistanceTo(normalInterselectionPoint) - Tick.Self.Radius -
                                             projectileInfo.Radius - additionalDangerZone <= 0;
                        canEvade = CanEvadeFromInterselectPoint(projectileInfo, normalInterselectionPoint);
                        evadeVector = GetEvadeVector(normalInterselectionPoint);
                    }
                }
                else
                {
                    if (IsInterselectByHead(projectileInfo, additionalDangerZone))
                    {
                        isInDangerZone = true;
                        canEvade = CanEvadeFromInterselectPointHead(projectileInfo);
                        evadeVector = GetEvadeVector(projectileInfo.EndPoint);
                        //VisualClientHelper.Line(Tick.Self.GetPositionPoint(), Tick.Self.GetPositionPoint()+ evadeVector, 0,1,0);
                    }
                }

                projectileInfo.IsInDangerZone = isInDangerZone;
                projectileInfo.CanEvade = canEvade;
                projectileInfo.EvadeVector = evadeVector;
            }
        }

        //Получить вектор ухода от снаряда
        private static Vector GetEvadeVector(Point2D interselectPoint)
        {
            var evadeVector =
                new Vector(interselectPoint.X, interselectPoint.Y) -
                new Vector(Tick.Self.X, Tick.Self.Y);
            evadeVector.Negate();
            evadeVector = 1000 * evadeVector;
            return evadeVector;
        }

        //Есть ли пересечение по радиусу конца выстрела?
        //Считаем что есть если мы в зоне поражения радиуса конца выстрела  + буфер
        private static bool IsInterselectByHead(ProjectilesInfo projectileInfo, double additionalDangerZone)
        {
            return Tick.Self.GetDistanceTo(projectileInfo.EndPoint.X, projectileInfo.EndPoint.Y) - Tick.Self.Radius -
                   projectileInfo.Radius - additionalDangerZone <= 0;
        }

        //Есть ли нормаль к вектору
        private static bool IsInterselectByNormal(ProjectilesInfo projectileInfo, out Point2D normalInterselectionPoint)
        {
            var p1 = projectileInfo.StartPoint;
            var p2 = projectileInfo.EndPoint;

            var me = new Vector(Tick.Self.X, Tick.Self.Y);
            var xy1 = new Vector(p1.X, p1.Y);
            var xy2 = new Vector(p2.X, p2.Y);

            var v0 = me - xy1;
            var v2 = xy2 - xy1;

            var scalar = (v0 * v2) / (v2 * v2);

            normalInterselectionPoint = p1 + scalar * v2;

            var isInterselect = scalar > 0 && scalar < 1;
            return isInterselect;
        }

        private static bool CanEvadeFromInterselectPoint(ProjectilesInfo projectile, Point2D interselectPoint)
        {
            var interselectPointRange = projectile.CurrentPoint.GetDistanceTo(interselectPoint);
            var interselectMyRange = Tick.Self.GetDistanceTo(interselectPoint.X, interselectPoint.Y);

            var distanceToEscape = Tick.Self.Radius + projectile.Radius - interselectMyRange;
            if (distanceToEscape < 0)
            {
                return true;
            }

            var wizardSpeed = 3 * UnitHelper.GetSpeedBonusFactorFromSkillsAndAuras();

            var projectilesToInterselectTicks = Math.Abs(/*(int)*/ (interselectPointRange / projectile.Speed));
            var escapeTicks = Math.Abs(/*(int)*/ (distanceToEscape / wizardSpeed));
            //DebugTrace.ConsoleWriteLite($"{projectilesToInterselectTicks.ToString("N3")} / {escapeTicks.ToString("N3")}");

            return escapeTicks <= projectilesToInterselectTicks;
        }

        private static bool CanEvadeFromInterselectPointHead(ProjectilesInfo projectile)
        {
            var interselectPointRange = projectile.CurrentPoint.GetDistanceTo(projectile.EndPoint);

            var safeDistance = Tick.Self.Radius + projectile.Radius;
            var evadeVector =
                new Vector(projectile.EndPoint) -
                new Vector(Tick.Self.GetPositionPoint());
            evadeVector.Negate();
            evadeVector.Normalize();
            evadeVector = safeDistance*evadeVector;
            var safePoint = projectile.EndPoint + evadeVector;

            //VisualClientHelper.Circle(safePoint.X,safePoint.Y, 5, 1,0,0);

            var distanceToEscape = Tick.Self.GetDistanceTo(safePoint.X, safePoint.Y);

            var wizardSpeed = 3 * UnitHelper.GetSpeedBonusFactorFromSkillsAndAuras();

            var projectilesToInterselectTicks = Math.Abs(interselectPointRange/projectile.Speed);
            var escapeTicks = Math.Abs(distanceToEscape/wizardSpeed);
            //DebugTrace.ConsoleWriteLite($"{projectilesToInterselectTicks.ToString("N3")} / {escapeTicks.ToString("N3")}");

            return escapeTicks <= projectilesToInterselectTicks;
        }
    }
}
