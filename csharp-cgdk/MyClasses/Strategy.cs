using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Basic;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Helpers;
using EpPathFinding.cs;

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
            Tick.Move.SkillToLearn = SkillsHelper.GetSkillToLearn();

            if (Tick.Self.IsMaster)
            {
                var messages = new[]
                {
                    new Message(LaneType.Top, null, new byte[0]),
                    new Message(LaneType.Middle, null, new byte[0]),
                    new Message(LaneType.Middle, null, new byte[0]),
                    new Message(LaneType.Bottom, null, new byte[0]),
                };
                Tick.Move.Messages = messages;
                GameState.MyLaneType = LaneType.Middle;
            }
            else
            {
                //Первые 10 тиков ждем команду мастера, определяемся с лайном
                if (Tick.World.TickIndex < 10)
                {
                    if (Tick.Self.Messages != null && Tick.Self.Messages.Length > 0)
                    {
                        var message = Tick.Self.Messages.Last();
                        GameState.MyLaneType = message.Lane;
                    }
                    return;
                }

                if (GameState.MyLaneType == null)
                {
                    GameState.MyLaneType = LaneType.Middle;
                }
            }

            //DebugTrace.ExecuteVisualizer(() =>
            //{
            //    DebugTrace.ConsoleWriteLite($"XP {Tick.Self.Xp} Level {Tick.Self.Level} Skills {string.Join(", ", Tick.Self.Skills)}");
            //});

            var pushPower = PushPowerHelper.GetPushPower();

            //var arrow = pushPower.FrienlyPower >= pushPower.EnemyPower ? "-->" : "<--";
            //DebugHelper.ConsoleWriteLite($"{pushPower.FrienlyPower.ToString("N3")} / {pushPower.EnemyPower.ToString("N3")} {arrow}");

            var evideableProjectiles = ProjectilesHelper.GetEvideableProjectiles();
            if (evideableProjectiles.Any())
            {
                //TODO: evide frostbolt first?
                var firstEvideableProjectile = evideableProjectiles.First();
                var evideVector = firstEvideableProjectile.EvadeVector;
                evideVector.Normalize();
                evideVector = 5*evideVector;

                var evdePoint = new Point2D(Tick.Self.X, Tick.Self.Y) + evideVector;
                var moveToParams = new MoveToParams()
                {
                    TargetPoint = evdePoint,
                    //LookAtPoint = new Point2D(0, Tick.Game.MapSize)
                };
                MoveHelper.MoveTo(moveToParams);
                return;
            }


            var target = GetOptimalTargetToAtack(Tick.Self.CastRange);

            if (pushPower.FrienlyPower >= pushPower.EnemyPower)
            {
                if (target != null)
                {
                    AtackTarget(target);
                    if (target.Faction != Faction.Neutral && target.Faction != Faction.Other &&
                        Tick.Self.GetDistanceTo(target) > Tick.Self.CastRange*0.8)
                    {
                        MoveHelper.MoveTo(new MoveToParams()
                        {
                            TargetPoint = new Point2D(target.X, target.Y),
                            //TargetPoint = new Point2D(Tick.Game.MapSize - PathFindingHelper.gridStep, 0),
                            LookAtPoint = new Point2D(target.X, target.Y),
                        });
                    }
                }
                else
                {
                    var enemyBasePoint = new Point2D(Tick.Game.MapSize - PathFindingHelper.gridStep, 0);
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
                if (target == null)
                {
                    //Некого атаковать, но будем смотреть на ближайшего врага
                    target = GetOptimalTargetToAtack(Tick.Self.CastRange*1.5);
                }
                else
                {
                    AtackTarget(target);
                }

                var moveToParams = new MoveToParams()
                {
                    TargetPoint = new Point2D(0, Tick.Game.MapSize - PathFindingHelper.gridStep),
                    LookAtPoint = target != null ? new Point2D(target.X, target.Y) : null
                };
                MoveHelper.MoveTo(moveToParams);
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
                var nearestEnemyBuidings = UnitHelper.GetNearestBuidigs(range, false,
                    GetObjectRangeMode.CenterToTargetBorder);
                if (nearestEnemyBuidings.Count > 0)
                {
                    targetToAtack = nearestEnemyBuidings.OrderBy(x => x.Life).FirstOrDefault();
                }
                else
                {
                    var nearestEnemyMinions =
                        UnitHelper.GetNearestMinions(range, false, GetObjectRangeMode.CenterToTargetBorder)
                            .RemoveNonAgressiveNeutrals();
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
                        if (target is Wizard)
                        {
                            var canCastFrostbolt = Tick.Self.Skills.Any(x => x == SkillType.FrostBolt) &&
                                                   Tick.Self.RemainingActionCooldownTicks == 0 &&
                                                   Tick.Self.RemainingCooldownTicksByAction[(int) ActionType.FrostBolt] ==
                                                   0;
                            var canCastFireball = Tick.Self.Skills.Any(x => x == SkillType.Fireball) &&
                                                  Tick.Self.RemainingActionCooldownTicks == 0 &&
                                                  Tick.Self.RemainingCooldownTicksByAction[(int) ActionType.Fireball] ==
                                                  0;
                            if (canCastFrostbolt)
                            {
                                Tick.Move.Action = ActionType.FrostBolt;
                            }
                            else if (canCastFireball)
                            {
                                Tick.Move.Action = ActionType.Fireball;
                            }
                            else
                            {
                                Tick.Move.Action = ActionType.MagicMissile;
                            }
                        }
                        else
                        {
                            Tick.Move.Action = ActionType.MagicMissile;
                        }
                        Tick.Move.CastAngle = angle;
                        Tick.Move.MinCastDistance = distance - target.Radius + Tick.Game.MagicMissileRadius;
                    }
                }
            }
        }
    }
}
