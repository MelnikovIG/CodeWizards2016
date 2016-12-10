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

        public static double GetSpeedBonusFactorFromSkillsAndAuras()
        {
            var me = Tick.Self;
            var hasHaste = me.Statuses.Any(x => x.Type == StatusType.Hastened);

            var nearAllyWizardsWithSpeedAura =
                Tick.World.Wizards.Where(x => x.Faction == me.Faction)
                    .Where(x => me.GetDistanceTo(x) <= Tick.Game.AuraSkillRange)
                    .Where(
                        x =>
                            x.Skills.Contains(SkillType.MovementBonusFactorAura2) ||
                            x.Skills.Contains(SkillType.MovementBonusFactorAura1))
                    .ToList();

            var allyWizardsMaxBonusFactor = 0;
            if (nearAllyWizardsWithSpeedAura.Any())
            {
                var isSecondLevelAura =
                    nearAllyWizardsWithSpeedAura.Any(x => x.Skills.Contains(SkillType.MovementBonusFactorAura2));
                allyWizardsMaxBonusFactor = isSecondLevelAura ? 2 : 1;
            }

            var myMaxSpeedFactor = 0;
            if (me.Skills.Any(x => x == SkillType.MovementBonusFactorAura2))
            {
                myMaxSpeedFactor = 4;
            }
            else if (me.Skills.Any(x => x == SkillType.MovementBonusFactorPassive2))
            {
                myMaxSpeedFactor = 3;
            }
            if (me.Skills.Any(x => x == SkillType.MovementBonusFactorAura2))
            {
                myMaxSpeedFactor = 2;
            }
            else if (me.Skills.Any(x => x == SkillType.MovementBonusFactorPassive2))
            {
                myMaxSpeedFactor = 1;
            }

            var maxSkillBonusFactor = myMaxSpeedFactor > allyWizardsMaxBonusFactor
                ? myMaxSpeedFactor
                : allyWizardsMaxBonusFactor;

            var totalSpeedFactor = 1 + maxSkillBonusFactor*Tick.Game.MovementBonusFactorPerSkillLevel +
                                   (hasHaste ? Tick.Game.HastenedMovementBonusFactor : 0);
            return totalSpeedFactor;
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
