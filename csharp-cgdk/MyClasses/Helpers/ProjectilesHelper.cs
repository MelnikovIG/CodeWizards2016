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
        //Получить вражеские снаряды, пересекающие героя
        public static List<ProjectilesInfo> GetInterselecitionsProjectiles()
        {
            FillProjectilesInfo();

            var interselectedProjectiles =GameState.ProjectilesInfo.Where(x =>x.Value.IsInterselect).ToList();
            return interselectedProjectiles.Select(x => x.Value).ToList();
        }
    
        private static void FillProjectilesInfo()
        {
            var pCircle = new Point2D(Tick.Self.X, Tick.Self.Y);
            var radius = Tick.Self.Radius;

            foreach (var projectileInfo in GameState.ProjectilesInfo)
            {
                var p1 = projectileInfo.Value.StartPoint;
                var p2 = projectileInfo.Value.EndPoint;

                var me = new Vector(Tick.Self.X, Tick.Self.Y);
                var xy1 = new Vector(p1.X, p1.Y);
                var xy2 = new Vector(p2.X, p2.Y);

                var v0 = me - xy1;
                var v2 = xy2 - xy1;

                var scalar = (v0 * v2) / (v2 * v2);

                var normalInterselectionPoint = p1 + scalar * v2;

                var isInterselect = scalar > 0 && scalar < 1;
                var isInRadius = pCircle.GetDistanceTo(normalInterselectionPoint) <= radius;

                projectileInfo.Value.IsInterselect = isInterselect && isInRadius;
                projectileInfo.Value.NormalInterselectionPoint = normalInterselectionPoint;
            }
        }
    }
}
