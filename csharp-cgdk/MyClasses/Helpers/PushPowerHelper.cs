using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Helpers
{
    public class PushPowerHelper
    {
        private static double wizardDamagePerTick = (double)Tick.Game.MagicMissileDirectDamage/Tick.Game.MagicMissileCooldownTicks;
        private static double guardianTowerDamagePerTick = (double) Tick.Game.GuardianTowerDamage/Tick.Game.GuardianTowerCooldownTicks;
        private static double factionBaseDamagePerTick = (double) Tick.Game.FactionBaseDamage/Tick.Game.FactionBaseCooldownTicks;
        private static double ordWoodcutterDamagePerTick = (double) Tick.Game.OrcWoodcutterDamage/Tick.Game.OrcWoodcutterActionCooldownTicks;
        private static double fetishBlowdartDamagePerTick = (double) Tick.Game.DartDirectDamage/Tick.Game.FetishBlowdartActionCooldownTicks;

        private static double baseWizardPower = 1;
        private static double baseGuardianTowerPower = baseWizardPower * guardianTowerDamagePerTick/ wizardDamagePerTick;
        private static double baseFactionBasePower = baseWizardPower * factionBaseDamagePerTick / wizardDamagePerTick;
        private static double ordWoodcutterBasePower = baseWizardPower * ordWoodcutterDamagePerTick / wizardDamagePerTick;
        private static double fetishBlowdartBasePower = baseWizardPower * fetishBlowdartDamagePerTick / wizardDamagePerTick;

        public static PushPower GetPushPower()
        {
            //Будем скинровать на большом радиусе, на всякий случай,
            //но при учете опасности все равно будем учитывть расстрояние то меня
            var enemyScanRange = Tick.Self.CastRange * 1.5;
            var friendRange = Tick.Self.CastRange;

            var enemyWizardsPower = UnitHelper.GetNearestWizards(enemyScanRange, false).Select(GetWizardPower).Sum();
            var enemyMinionsPower = UnitHelper.GetNearestMinions(enemyScanRange, false).Select(GetMinionPower).Sum();
            var enemyTowersPower = UnitHelper.GetNearestBuidigs(enemyScanRange, false).Select(GetBuildingPower).Sum();
            var enemyPower = enemyWizardsPower + enemyMinionsPower + enemyTowersPower;

            var friendlyWizardsPower = UnitHelper.GetNearestWizards(friendRange, true).Select(GetWizardPower).Sum();
            var friendlyMinionsPower = UnitHelper.GetNearestMinions(friendRange, true).Select(GetMinionPower).Sum();
            var friendlyTowersPower = UnitHelper.GetNearestBuidigs(friendRange, true).Select(GetBuildingPower).Sum();
            var friendlyPower = friendlyWizardsPower + friendlyMinionsPower + friendlyTowersPower;

            friendlyPower = friendlyPower*(Tick.Self.Life/(double) Tick.Self.MaxLife);

            var pushPower = new PushPower()
            {
                FrienlyPower = friendlyPower,
                EnemyPower = enemyPower
            };

            return pushPower;
        }

        private static double GetWizardPower(Wizard wizard)
        {
            var myCastRange = Tick.Self.CastRange;

            //От создных героев толку мало)
            if (wizard.Faction == Tick.Self.Faction && !wizard.IsMe)
            {
                if (Tick.Self.GetDistanceTo(wizard) < Tick.Self.Radius*5)
                {
                    return baseWizardPower*0.3;
                }
                return 0;
            }
            else
            {
                //Расстояние, с которого вражеский маг может нанести урон с учетом типа снаряда
                var wizardCastRange = wizard.CastRange + Tick.Self.Radius + GetAdditionalRangeFromProjectileType(wizard);
                var ignoreWizardAdditionalBuffer = 10;

                if (wizard.Faction != Tick.Self.Faction)
                {
                    if (Tick.Self.GetDistanceTo(wizard) > wizardCastRange + ignoreWizardAdditionalBuffer)
                    {
                        return 0;
                    }
                }

                var hasEmpower = wizard.Statuses.Any(x => x.Type == StatusType.Empowered);
                var hasHaste = wizard.Statuses.Any(x => x.Type == StatusType.Hastened);
                var hasShield = wizard.Statuses.Any(x => x.Type == StatusType.Shielded);
                var frozen = wizard.Statuses.Any(x => x.Type == StatusType.Frozen);

                var result = baseWizardPower
                             * (wizard.Life/(double) wizard.MaxLife)
                             *(0.75 + 0.25*GetWizardMainProjectileCooldownactor(wizard));

                //TODO: в случае раскоментирования переписать, работает неверно изза дого что каст врага больше
                //if (wizard.CastRange > myCastRange)
                //{
                //    result *= wizard.CastRange/myCastRange;
                //}

                if (hasEmpower)
                {
                    result *= 1.3;
                }

                if (hasHaste)
                {
                    result *= 1.1;
                }

                if (frozen)
                {
                    var frozenStatus =
                        wizard.Statuses.Where(x => x.Type == StatusType.Frozen)
                            .OrderByDescending(x => x.RemainingDurationTicks)
                            .First();
                    result *= 0.5 +
                              0.5*
                              ((double)(Tick.Game.FrozenDurationTicks - frozenStatus.RemainingDurationTicks)/
                               Tick.Game.FrozenDurationTicks);
                }

                return result;
            }
        }

        private static double GetMinionPower(Minion minion)
        {
            if (minion.Faction == Tick.Self.Faction)
            {
                //Пока не будем считать союзных крипов как силу
                return 0;
            }
            else if (minion.Faction == Faction.Neutral || minion.Faction == Faction.Other)
            {

                var isAgressive = UnitHelper.IsNeutralMinionAgressive(minion);

                if (isAgressive)
                {
                    if (minion.Type == MinionType.OrcWoodcutter)
                    {
                        if (Tick.Self.GetDistanceTo(minion) - Tick.Self.Radius < Tick.Game.OrcWoodcutterAttackRange)
                        {
                            return ordWoodcutterBasePower;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else if (minion.Type == MinionType.FetishBlowdart)
                    {
                        if (Tick.Self.GetDistanceTo(minion) - Tick.Self.Radius < Tick.Game.FetishBlowdartAttackRange)
                        {
                            return fetishBlowdartBasePower;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    return 0;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                if (minion.Type == MinionType.OrcWoodcutter)
                {
                    if (Tick.Self.GetDistanceTo(minion) - Tick.Self.Radius < Tick.Game.OrcWoodcutterAttackRange)
                    {
                        return ordWoodcutterBasePower;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else if (minion.Type == MinionType.FetishBlowdart)
                {
                    if (Tick.Self.GetDistanceTo(minion) - Tick.Self.Radius < Tick.Game.FetishBlowdartAttackRange)
                    {
                        return fetishBlowdartBasePower;
                    }
                    else
                    {
                        return 0;
                    }
                }

                return 0;
            }
        }

        private static double GetBuildingPower(Building building)
        {
            var basePower = building.Type == BuildingType.FactionBase ? baseFactionBasePower : baseGuardianTowerPower;
            if (building.Faction == Tick.Self.Faction)
            {
                //Пока не будем считать союзных башен как силу
                return 0;
            }
            else
            {
                //Расстояние, с которого вражесая башня может нанести урон
                var buildingAttackRange = building.AttackRange;
                var ignoreBuildingAdditionalBuffer = 10;

                if (Tick.Self.GetDistanceTo(building) > buildingAttackRange + ignoreBuildingAdditionalBuffer)
                {
                    return 0;
                }

                if (building.Type == BuildingType.FactionBase)
                {
                    return basePower*
                           ((Tick.Game.FactionBaseCooldownTicks - building.RemainingActionCooldownTicks)/
                            (double) Tick.Game.FactionBaseCooldownTicks);
                }
                else
                {
                    return basePower*
                           ((Tick.Game.GuardianTowerCooldownTicks - building.RemainingActionCooldownTicks)/
                            (double) Tick.Game.GuardianTowerCooldownTicks);
                }
            }
        }

        //Получить радиус дополнительный в зависимости от  типа снаряда
        private static double GetAdditionalRangeFromProjectileType(Wizard wizard)
        {
            var hasFireball = wizard.Skills.Any(x => x == SkillType.Fireball);
            var hasFrostBolt = wizard.Skills.Any(x => x == SkillType.FrostBolt);

            if (hasFireball)
            {
                return Tick.Game.FireballRadius + Tick.Game.FireballExplosionMaxDamageRange/2;
            }
            else if (hasFrostBolt)
            {
                return Tick.Game.FrostBoltRadius;
            }
            else return Tick.Game.MagicMissileRadius;
        }

        //Коэффициент ослабление врага при перезарядке
        private static double GetWizardMainProjectileCooldownactor(Wizard wizard)
        {
            var hasFireball = wizard.Skills.Any(x => x == SkillType.Fireball);
            var hasFrostBolt = wizard.Skills.Any(x => x == SkillType.FrostBolt);

            if (hasFireball)
            {
                return (Tick.Game.FireballCooldownTicks -
                        wizard.RemainingCooldownTicksByAction[(int) ActionType.Fireball])/
                       (double) Tick.Game.FireballCooldownTicks;
            }
            else if (hasFrostBolt)
            {
                return (Tick.Game.FrostBoltCooldownTicks -
                        wizard.RemainingCooldownTicksByAction[(int) ActionType.FrostBolt])/
                       (double) Tick.Game.FrostBoltCooldownTicks;
            }
            else
                return (Tick.Game.MagicMissileCooldownTicks -
                        wizard.RemainingCooldownTicksByAction[(int) ActionType.MagicMissile])/
                       (double) Tick.Game.MagicMissileCooldownTicks;
        }
    }

    public class PushPower
    {
        public double FrienlyPower { get; set; }
        public double EnemyPower { get; set; }
    }
}
