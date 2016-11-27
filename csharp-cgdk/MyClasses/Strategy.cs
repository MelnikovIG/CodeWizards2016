using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Basic;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Helpers;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses
{
    /*
    Порядок обработки алгоритма
    1) move.skillToLearn
    2) move.action
    3) move.speed и move.strafeSpeed
    4) move.turn
    */

    public static class Strategy
    {
        public static void Execute()
        {
            var pushPower = GetPushPower();

            DebugTrace.ExecuteVisualizer(() =>
            {
                var arrow = pushPower.FrienlyPower >= pushPower.EnemyPower ? "-->" : "<--";
                DebugTrace.ConsoleWriteLite(
                    $"{pushPower.FrienlyPower.ToString("N3")} / {pushPower.EnemyPower.ToString("N3")} {arrow}");
            });

            if (pushPower.FrienlyPower >= pushPower.EnemyPower)
            {
                var target = GetOptimalTargetToAtack(Tick.Self.CastRange);
                if (target != null)
                {
                    AtackTarget(target);
                    if (target.Faction != Faction.Neutral && target.Faction != Faction.Other)
                    {
                        MoveHelper.MoveTo(new MoveToParams()
                        {
                            TargetPoint = new Point2D(target.X, target.Y),
                            LookAtPoint = new Point2D(target.X, target.Y),
                        });
                    }
                }
                else
                {
                    var enemyBasePoint = new Point2D(Tick.Game.MapSize, 0);
                    var moveToParams = new MoveToParams()
                    {
                        TargetPoint = enemyBasePoint,
                        //LookAtPoint = new Point2D(0, Tick.Game.MapSize)
                    };
                    MoveHelper.MoveTo(moveToParams);
                }
            }
            else
            {
                AttackAnyTargetOnBackward();

                var viewTarget = GetOptimalTargetToAtack(Tick.Self.CastRange * 1.5);

                var moveToParams = new MoveToParams()
                {
                    TargetPoint = new Point2D(0, Tick.Game.MapSize),
                    LookAtPoint = viewTarget != null ? new Point2D(viewTarget.X, viewTarget.Y) : null
                };
                MoveHelper.MoveTo(moveToParams);
            }
        }

        //Атаковать любую цель при отступлении, которая попадает в рендж
        private static void AttackAnyTargetOnBackward()
        {
            var castRange = Tick.Self.CastRange;

            LivingUnit targetToAtack = null;

            //При отступлении не повораиваемся ради атаки, а ищем цели которые подходят по текущий угол
            Func<LivingUnit, bool> isAngleAllowedToAttack = unit =>
            {
                var angle = Tick.Self.GetAngleTo(unit);
                return Math.Abs(angle) < Tick.Game.StaffSector/2.0D;
            };

            var nearestEnemyWizards =
                UnitHelper.GetNearestWizards(castRange, false, GetObjectRangeMode.CenterToTargetBorder).Where(isAngleAllowedToAttack).ToList();
            if (nearestEnemyWizards.Count > 0)
            {
                targetToAtack = nearestEnemyWizards.OrderBy(x => x.Life).FirstOrDefault();
            }
            else
            {
                var nearestEnemyBuidings =
                    UnitHelper.GetNearestBuidigs(castRange, false).Where(isAngleAllowedToAttack).ToList();
                
                if (nearestEnemyBuidings.Count > 0)
                {
                    targetToAtack = nearestEnemyBuidings.OrderBy(x => x.Life).FirstOrDefault();
                }
                else
                {
                    var nearestEnemyMinions =
                        UnitHelper.GetNearestMinions(castRange, false).Where(isAngleAllowedToAttack).ToList();
                    ;
                    if (nearestEnemyMinions.Count > 0)
                    {
                        targetToAtack = nearestEnemyMinions.OrderBy(x => x.Life).FirstOrDefault();
                    }
                }
            }

            if (targetToAtack == null)
            {
                return;
            }
            else
            {
                double distance = Tick.Self.GetDistanceTo(targetToAtack);
                double angle = Tick.Self.GetAngleTo(targetToAtack);
                if (distance - targetToAtack.Radius <= 70)
                {
                    // ... то атакуем с руки.
                    Tick.Move.Action = ActionType.MagicMissile;
                    Tick.Move.CastAngle = angle;
                    Tick.Move.MinCastDistance = distance - targetToAtack.Radius + Tick.Game.MagicMissileRadius;
                }
                else
                {
                    // ... то атакуем палкой.
                    Tick.Move.Action = ActionType.MagicMissile;
                    Tick.Move.CastAngle = angle;
                    Tick.Move.MinCastDistance = distance - targetToAtack.Radius + Tick.Game.MagicMissileRadius;
                }
            }
        }

        /// <summary>
        /// Получить цель для атаки
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        private static LivingUnit GetOptimalTargetToAtack(double range)
        {
            LivingUnit targetToAtack = null;

            var nearestEnemyWizards = UnitHelper.GetNearestWizards(range, false, GetObjectRangeMode.CenterToTargetBorder);
            if (nearestEnemyWizards.Count > 0)
            {
                targetToAtack = nearestEnemyWizards.OrderBy(x => x.Life).FirstOrDefault();
            }
            else
            {
                var nearestEnemyBuidings = UnitHelper.GetNearestBuidigs(range, false, GetObjectRangeMode.CenterToTargetBorder);
                if (nearestEnemyBuidings.Count > 0)
                {
                    targetToAtack = nearestEnemyBuidings.OrderBy(x => x.Life).FirstOrDefault();
                }
                else
                {
                    var nearestEnemyMinions = UnitHelper.GetNearestMinions(range, false, GetObjectRangeMode.CenterToTargetBorder);
                    if (nearestEnemyMinions.Count > 0)
                    {
                        targetToAtack = nearestEnemyMinions.OrderBy(x => x.Life).FirstOrDefault();
                    }
                }
            }

            return targetToAtack;
        }

        public static void AtackTarget(LivingUnit target)
        {
            double distance = Tick.Self.GetDistanceTo(target);

            // ... и он в пределах досягаемости наших заклинаний, ...
            if (distance <= Tick.Self.CastRange + target.Radius)
            {
                double angle = Tick.Self.GetAngleTo(target);

                // ... то поворачиваемся к цели.
                Tick.Move.Turn = angle;

                // Если цель перед нами, ...
                if (Math.Abs(angle) < Tick.Game.StaffSector/2.0D)
                {
                    if (distance - target.Radius <= Tick.Game.StaffRange)
                    {
                        // ... то атакуем с руки.
                        Tick.Move.Action = ActionType.Staff;
                        Tick.Move.CastAngle = angle;
                        Tick.Move.MinCastDistance = distance - target.Radius + Tick.Game.MagicMissileRadius;
                    }
                    else
                    {
                        // ... то атакуем палкой.
                        Tick.Move.Action = ActionType.MagicMissile;
                        Tick.Move.CastAngle = angle;
                        Tick.Move.MinCastDistance = distance - target.Radius + Tick.Game.MagicMissileRadius;
                    }
                }
            }
        }

        private static PushPower GetPushPower()
        {
            var wizardBasePower = 1;
            var minionBasePower = 0.3;
            var towerBasePower = 0.4;
            var baseBasePower = 0.7;
            var enemyRange = Tick.Self.CastRange + Tick.Game.WizardRadius * 2;
            var friendRange = Tick.Self.CastRange;

            Func<Wizard, double> getWizardPower =
                (wizard) =>
                {
                    //От создных героев олку мало)
                    if (wizard.Faction == Tick.Self.Faction && !wizard.IsMe)
                    {
                        return wizardBasePower*0.2;
                    }
                    else
                    {
                        var hasEmpower = wizard.Statuses.Any(x => x.Type == StatusType.Empowered);
                        var hasHaste = wizard.Statuses.Any(x => x.Type == StatusType.Hastened);
                        var hasShield = wizard.Statuses.Any(x => x.Type == StatusType.Shielded);

                        var result = wizardBasePower*(wizard.Life/(double) wizard.MaxLife)*
                               ((Tick.Game.MagicMissileCooldownTicks - wizard.RemainingCooldownTicksByAction[2])/
                                (double) Tick.Game.MagicMissileCooldownTicks);

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
                };

            Func<Minion, double> getMinionPower =
                (minion) =>
                {
                    if (minion.Faction == Tick.Self.Faction)
                    {
                        //Пока не будем считать союзных крипов как силу
                        return 0;
                    }
                    else if (minion.Faction == Faction.Neutral || minion.Faction == Faction.Other)
                    {

                        var isAgressive = minion.SpeedX > 0 || minion.SpeedY > 0 || minion.Life < minion.MaxLife ||
                                          minion.RemainingActionCooldownTicks > 0;

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
                    //return minionBasePower*(minion.Life/(double) minion.MaxLife);
                };

            Func<Building, double> getBuidingPower =
                (building) =>
                {
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
                    //return power*(building.Life/(double) building.MaxLife);
                };

            var enemyWizardsPower = UnitHelper.GetNearestWizards(enemyRange, false).Select(x => getWizardPower(x)).Sum();
            var enemyMinionsPower = UnitHelper.GetNearestMinions(enemyRange, false).Select(x => getMinionPower(x)).Sum();
            var enemyTowersPower = UnitHelper.GetNearestBuidigs(enemyRange, false).Select(x => getBuidingPower(x)).Sum();
            var enemyPower = enemyWizardsPower + enemyMinionsPower + enemyTowersPower;

            var friendlyWizardsPower = UnitHelper.GetNearestWizards(friendRange, true).Select(x => getWizardPower(x)).Sum();
            var friendlyMinionsPower = UnitHelper.GetNearestMinions(friendRange, true).Select(x => getMinionPower(x)).Sum();
            var friendlyTowersPower = UnitHelper.GetNearestBuidigs(friendRange, true).Select(x => getBuidingPower(x)).Sum();
            var friendlyPower = friendlyWizardsPower + friendlyMinionsPower + friendlyTowersPower;

            friendlyPower = friendlyPower*(Tick.Self.Life/(double) Tick.Self.MaxLife);

            var pushPower = new PushPower()
            {
                FrienlyPower = friendlyPower,
                EnemyPower = enemyPower
            };

            return pushPower;
        }
    }
}
