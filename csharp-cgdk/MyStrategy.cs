using System.Globalization;
using System.Linq;
using System.Threading;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Basic;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Helpers;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk
{
    public sealed class MyStrategy : IStrategy
    {
        public MyStrategy()
        {
            DebugTrace.ExecuteVisualizer(() =>
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                Debug.connect("127.0.0.1", 13579);
            });
        }

        public void Move(Wizard self, World world, Game game, Move move)
        {
            FillCurrentTickParamsBeforeStart(self, world, game, move);
            Strategy.Execute();
            FillCurrentTickParamsBeforeEnd();

            DebugTrace.ExecuteVisualizer(() =>
            {
                Debug.beginPost();
                Debug.circle(Tick.Self.X, Tick.Self.Y, Tick.Self.CastRange, 50);
                Debug.endPost();
            });

        }

        private void FillCurrentTickParamsBeforeStart(Wizard self, World world, Game game, Move move)
        {
            Tick.UpdateTick(self, world, game, move);

            var allyWizards = Tick.World.Wizards.Where(x => x.Faction == Tick.Self.Faction);
            var allyWizardsIds = allyWizards.Select(x => x.OwnerPlayerId).ToList();
            var projectilesCacheRange = 1000;
            var currentWorldProjectiles =
                Tick.World.Projectiles.Where(x => Tick.Self.GetDistanceTo(x) < projectilesCacheRange)
                    .Where(x => !allyWizardsIds.Contains(x.OwnerPlayerId))
                    .Where(x => x.Type == ProjectileType.MagicMissile).ToList();
            var prevTickProjectilesIds = GameState.ProjectilesInfo.Select(x => x.Key).ToList();
            var projectilesToRemove =
                prevTickProjectilesIds.Where(x => !currentWorldProjectiles.Select(y => y.Id).Contains(x)).ToList();
            var projectilesToAdd = currentWorldProjectiles.Where(x => !prevTickProjectilesIds.Contains(x.Id)).ToList();
            foreach (var projectile in projectilesToAdd)
            {
                var startPoint = GameState.WizardsLastPositions.ContainsKey(projectile.OwnerPlayerId)
                    ? GameState.WizardsLastPositions[projectile.OwnerPlayerId].Position
                    : new Point2D(projectile.X, projectile.Y);

                var endPoint = Point2D.GetPointAt(startPoint, projectile.Angle, Tick.Self.CastRange);

                GameState.ProjectilesInfo.Add(projectile.Id, new ProjectilesInfo()
                {
                    StartPoint = startPoint,
                    EndPoint = endPoint
                });
            }

            foreach (var removeItem in projectilesToRemove)
            {
                GameState.ProjectilesInfo.Remove(removeItem);
            }

            DebugTrace.ExecuteVisualizer(() =>
            {
                foreach (var projectilesInfo in GameState.ProjectilesInfo)
                {
                    Debug.beginPost();
                    Debug.line(projectilesInfo.Value.StartPoint.X, projectilesInfo.Value.StartPoint.Y,
                        projectilesInfo.Value.EndPoint.X, projectilesInfo.Value.EndPoint.Y, 0xFF0000);
                    Debug.endPost();
                }
            });
        }

        private void FillCurrentTickParamsBeforeEnd()
        {
            GameState.WizardsLastPositions = Tick.World.Wizards.ToDictionary(x => x.OwnerPlayerId,
                x => new WizardInfo() {Position = new Point2D(x.X, x.Y)});
        }
    }
}