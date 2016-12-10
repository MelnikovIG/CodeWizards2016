using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Helpers
{
    public static class UnitHelper
    {
        public static List<LivingUnit> GetForwardStuckedLivingUnits()
        {
            var allLivingUnits = new List<LivingUnit>();
            allLivingUnits.AddRange(Tick.World.Wizards.Where(x => !x.IsMe));
            allLivingUnits.AddRange(Tick.World.Minions);
            allLivingUnits.AddRange(Tick.World.Buildings);
            allLivingUnits.AddRange(Tick.World.Trees);

            var nearUnits =
                allLivingUnits
                    .Where(x => Tick.Self.GetDistanceTo(x) - x.Radius - Tick.Self.Radius <= 10).ToList();

            return nearUnits;
        }

        private static bool GetDistance<TCircularUnit>(TCircularUnit unit, double range, GetObjectRangeMode rangeMode)
            where TCircularUnit : CircularUnit
        {
            if (rangeMode == GetObjectRangeMode.BorderToBorder)
            {
                return Tick.Self.GetDistanceTo(unit) - unit.Radius - Tick.Self.Radius <= range;
            }
            else if (rangeMode == GetObjectRangeMode.CenterToTargetBorder)
            {
                return Tick.Self.GetDistanceTo(unit) - unit.Radius <= range;
            }

            return Tick.Self.GetDistanceTo(unit) <= range;
        }

        public static List<Building> GetNearestBuidigs(double range, bool friendly,
            GetObjectRangeMode rangeMode = GetObjectRangeMode.CenterToCenter, bool? canAttack = null)
        {
            if (friendly)
            {
                return
                    Tick.World.Buildings.Where(
                        x => x.Faction == Tick.Self.Faction)
                        .Where(x => GetDistance(x, range, rangeMode))
                        .ToList();
            }
            else
            {
                var nonDeadTowers = EnemyBuildingsHelper.Buildings.Where(x => !x.IsDead);
                if (canAttack.HasValue)
                {
                    nonDeadTowers = nonDeadTowers.Where(x => x.CanBeDamaged == canAttack.Value);
                }

                return
                    nonDeadTowers.Select(
                        x =>
                        {
                            long id = -1;
                            var _x = x.Position.X;
                            var _y = x.Position.Y;
                            var speedX = 0;
                            var speedY = 0;
                            var angle = 0;
                            var faction = Tick.Self.Faction == Faction.Academy ? Faction.Renegades : Faction.Academy;
                            var remainingActionCooldownTicks = x.RemainingActionCooldownTicks;

                            return new Building(id, _x, _y, speedX, speedY, angle, faction, x.Radius, 0,
                                0, new Status[0], BuildingType.GuardianTower, x.AttackRange, x.AttackRange, 0, 0,
                                remainingActionCooldownTicks);
                        })
                        .Where(x => GetDistance(x, range, rangeMode))
                        .ToList();
            }
        }

        public static List<Wizard> GetNearestWizards(double range, bool friendly,
            GetObjectRangeMode rangeMode = GetObjectRangeMode.CenterToCenter)
        {
            return
                Tick.World.Wizards.Where(x => friendly ? x.Faction == Tick.Self.Faction : x.Faction != Tick.Self.Faction)
                    .Where(x => GetDistance(x, range, rangeMode))
                    .ToList();
        }

        public static List<Minion> GetNearestMinions(double range, bool friendly,
            GetObjectRangeMode rangeMode = GetObjectRangeMode.CenterToCenter)
        {
            return
                Tick.World.Minions.Where(x => friendly ? x.Faction == Tick.Self.Faction : x.Faction != Tick.Self.Faction)
                    .Where(x => GetDistance(x, range, rangeMode))
                    .ToList();
        }

        public static bool IsNeutralMinionAgressive(Minion minion)
        {
            var isAgressive = minion.SpeedX > 0 || minion.SpeedY > 0 || minion.Life < minion.MaxLife ||
                  minion.RemainingActionCooldownTicks > 0;
            return isAgressive;
        }

        public static List<Minion> RemoveNonAgressiveNeutrals(this List<Minion> minions)
        {
            return minions.Where(x => x.Faction != Faction.Neutral || IsNeutralMinionAgressive(x)).ToList();
        } 
    }

    public enum GetObjectRangeMode
    {
        //Расстояние от цента до центра
        CenterToCenter,
        //Растрояние от края до края
        BorderToBorder,
        //От центра до края цели
        CenterToTargetBorder
    }
}
