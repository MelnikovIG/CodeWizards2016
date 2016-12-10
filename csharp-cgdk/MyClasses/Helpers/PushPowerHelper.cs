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
        public static PushPower GetPushPower()
        {
            var enemyScanRange = Tick.Game.WizardCastRange + 300;
            var friendRange = Tick.Self.CastRange;

            var enemyWizardsPower = UnitHelper.GetNearestWizards(enemyScanRange, false).Select(GetWizardPower).Sum();
            var enemyMinionsPower = UnitHelper.GetNearestMinions(enemyScanRange, false).Select(GetMinionPower).Sum();
            var enemyTowersPower = UnitHelper.GetNearestBuidigs(enemyScanRange, false).Select(GetBuildingPower).Sum();
            var enemyPower = enemyWizardsPower + enemyMinionsPower + enemyTowersPower;

            var friendlyWizardsPower = UnitHelper.GetNearestWizards(friendRange, true).Select(GetWizardPower).Sum();
            var friendlyMinionsPower = UnitHelper.GetNearestMinions(friendRange, true).Select(GetMinionPower).Sum();
            var friendlyTowersPower = UnitHelper.GetNearestBuidigs(friendRange, true).Select(GetBuildingPower).Sum();
            var friendlyPower = friendlyWizardsPower + friendlyMinionsPower + friendlyTowersPower;

            friendlyPower = friendlyPower * (Tick.Self.Life / (double)Tick.Self.MaxLife);

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
            var wizardBasePower = 1d;

            //От создных героев толку мало)
            if (wizard.Faction == Tick.Self.Faction && !wizard.IsMe)
            {
                if (Tick.Self.GetDistanceTo(wizard) < Tick.Self.Radius * 4)
                {
                    return wizardBasePower * 0.3;
                }
                return 0;
            }
            else
            {
                if (wizard.Faction != Tick.Self.Faction)
                {
                    if (Tick.Self.GetDistanceTo(wizard) > wizard.CastRange + Tick.Self.Radius * 2)
                    {
                        return 0;
                    }
                }

                var hasEmpower = wizard.Statuses.Any(x => x.Type == StatusType.Empowered);
                var hasHaste = wizard.Statuses.Any(x => x.Type == StatusType.Hastened);
                var hasShield = wizard.Statuses.Any(x => x.Type == StatusType.Shielded);

                var result = wizardBasePower
                             * (wizard.Life / (double)wizard.MaxLife)
                             *
                             (0.75 + 0.25 * (
                                 ((Tick.Game.MagicMissileCooldownTicks -
                                   wizard.RemainingCooldownTicksByAction[2]) /
                                  (double)Tick.Game.MagicMissileCooldownTicks)));

                if (wizard.CastRange > myCastRange)
                {
                    result *= wizard.CastRange / myCastRange;
                }

                if (hasEmpower)
                {
                    result *= 1.5;
                }

                if (hasHaste)
                {
                    result *= 1.3;
                }

                return result;
            }
        }

        private static double GetMinionPower(Minion minion)
        {
            var minionBasePower = 0.3;
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
                            return minionBasePower;
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
                            return minionBasePower;
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
                        return minionBasePower;
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
                        return minionBasePower;
                    }
                    else
                    {
                        return 0;
                    }
                }

                return minionBasePower;
            }
        }

        private static double GetBuildingPower(Building building)
        {
            var towerBasePower = 0.4;
            var baseBasePower = 0.7;

            var basePower = building.Type == BuildingType.FactionBase ? baseBasePower : towerBasePower;
            if (building.Faction == Tick.Self.Faction)
            {
                //Пока не будем считать союзных башен как силу
                return 0;
            }
            else
            {
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
    }

    public class PushPower
    {
        public double FrienlyPower { get; set; }
        public double EnemyPower { get; set; }
    }
}
