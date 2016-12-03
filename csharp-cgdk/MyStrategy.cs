using System;
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
        public void Move(Wizard self, World world, Game game, Move move)
        {
            FillCurrentTickParamsBeforeStart(self, world, game, move);
            Strategy.Execute();
            FillCurrentTickParamsBeforeEnd();

            //DebugTrace.ExecuteVisualizer(() =>
            //{
            //    VisualClient.Instance.BeginPost();
            //    VisualClient.Instance.Circle(Tick.Self.X, Tick.Self.Y, (float)Tick.Self.CastRange, 0,0,1);
            //    VisualClient.Instance.EndPost();
            //});

        }

        private void FillCurrentTickParamsBeforeStart(Wizard self, World world, Game game, Move move)
        {
            //VisualClient.Instance.BeginPost();

            Tick.UpdateTick(self, world, game, move);

            var allyWizards = Tick.World.Wizards.Where(x => x.Faction == Tick.Self.Faction);
            var allyWizardsIds = allyWizards.Select(x => x.OwnerPlayerId).ToList();
            var projectilesCacheRange = 1000;
            var currentWorldProjectiles =
                Tick.World.Projectiles.Where(x => Tick.Self.GetDistanceTo(x) < projectilesCacheRange)
                    .Where(x => !allyWizardsIds.Contains(x.OwnerPlayerId))
                    .Where(x => x.Type == ProjectileType.MagicMissile || x.Type == ProjectileType.FrostBolt || x.Type == ProjectileType.Fireball).ToList();
            var prevTickProjectilesIds = GameState.ProjectilesInfo.Select(x => x.Key).ToList();
            var projectilesToRemove =
                prevTickProjectilesIds.Where(x => !currentWorldProjectiles.Select(y => y.Id).Contains(x)).ToList();
            var projectilesToAdd = currentWorldProjectiles.Where(x => !prevTickProjectilesIds.Contains(x.Id)).ToList();
            foreach (var projectile in projectilesToAdd)
            {
                var wizardInfo = GameState.WizardsLastPositions.ContainsKey(projectile.OwnerPlayerId)
                    ? GameState.WizardsLastPositions[projectile.OwnerPlayerId]
                    : null;

                var startPoint = wizardInfo != null
                    ? wizardInfo.Position
                    : new Point2D(projectile.X, projectile.Y);

                var castRange = wizardInfo != null
                    ? wizardInfo.CastRange
                    : 700;

                var endPoint = Point2D.GetPointAt(startPoint, projectile.Angle, castRange);

                GameState.ProjectilesInfo.Add(projectile.Id, new ProjectilesInfo()
                {
                    StartPoint = startPoint,
                    EndPoint = endPoint,
                    Speed = Math.Sqrt(Math.Pow(projectile.SpeedX,2) + Math.Pow(projectile.SpeedY, 2)),//new Vector(projectile.SpeedX, projectile.SpeedY),
                    Type = projectile.Type,
                    Radius = projectile.Radius,
                    Angle = projectile.Angle
                });
            }

            foreach (var removeItem in projectilesToRemove)
            {
                GameState.ProjectilesInfo.Remove(removeItem);
            }

            foreach (var currentWorldProjectile in currentWorldProjectiles)
            {
                GameState.ProjectilesInfo[currentWorldProjectile.Id].CurrentPoint =
                    new Point2D(currentWorldProjectile.X, currentWorldProjectile.Y);
            }
        }

        private void FillCurrentTickParamsBeforeEnd()
        {
            GameState.WizardsLastPositions = Tick.World.Wizards.ToDictionary(x => x.OwnerPlayerId,
                x => new WizardInfo()
                {
                    Position = new Point2D(x.X, x.Y),
                    CastRange = x.CastRange
                });

            //VisualClient.Instance.EndPost();
        }
    }
}