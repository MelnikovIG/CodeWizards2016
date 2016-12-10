using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Basic;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Helpers
{
    public static class EnemyBuildingsHelper
    {
        public static List<EnemyBuildingInfo> Buildings { get; set; }

        public static void UpdateTowersInfo()
        {
            FillTowerInfo();

            var worldEnemyBuildings = Tick.World.Buildings.Where(x => x.Faction != Tick.Self.Faction).ToList();
            foreach (var enemyBuildingInfo in Buildings.Where(x => !x.IsDead))
            {
                var worldEnemyBuilding =
                    worldEnemyBuildings.FirstOrDefault(
                        x =>
                            Math.Abs(x.X - enemyBuildingInfo.Position.X) < 0.1 &&
                            Math.Abs(x.Y - enemyBuildingInfo.Position.Y) < 0.1);
                //Обновим парметры здания, если здание не найдето , то проверить не снесли ли его
                if (worldEnemyBuilding != null)
                {
                    enemyBuildingInfo.CooldownTicks = worldEnemyBuilding.CooldownTicks;
                    enemyBuildingInfo.RemainingActionCooldownTicks = worldEnemyBuilding.RemainingActionCooldownTicks;
                }
                else
                {
                    //Проверим, не убили ли башню, будем считать что убили, если ктото из союзников  должен видеть её, но не видит

                    List<LivingUnit> allyUnitsWhoShouldSeeBuilding = new List<LivingUnit>();
                    allyUnitsWhoShouldSeeBuilding.AddRange(Tick.World.Minions.Where(x => x.Faction == Tick.Self.Faction).Where(x => x.GetDistanceTo(enemyBuildingInfo.Position.X, enemyBuildingInfo.Position.Y) <= x.VisionRange));
                    allyUnitsWhoShouldSeeBuilding.AddRange(Tick.World.Wizards.Where(x => x.Faction == Tick.Self.Faction).Where(x => x.GetDistanceTo(enemyBuildingInfo.Position.X, enemyBuildingInfo.Position.Y) <= x.VisionRange));

                    if (allyUnitsWhoShouldSeeBuilding.Count > 0)
                    {
                        enemyBuildingInfo.IsDead = true;
                    }
                    else
                    {
                        if (enemyBuildingInfo.RemainingActionCooldownTicks > 0)
                        {
                            enemyBuildingInfo.RemainingActionCooldownTicks--;
                        }
                    }
                }
            }
        }

        //Заполнить изначальное значение башен , отразив параметры своих
        private static void FillTowerInfo()
        {
            if (Buildings == null)
            {
                Buildings = new List<EnemyBuildingInfo>();

                var allyBuildings =
                    Tick.World.Buildings.Where(x => x.Faction == Tick.Self.Faction)
                        .ToList();

                var i = 0;
                //Заполним вражеские тауеры, инвертируя  союзные
                foreach (var allyBuilding in allyBuildings.OrderBy(x => x.X))
                {
                    var buildingInfo = new EnemyBuildingInfo()
                    {
                        Position = new Point2D(Tick.Game.MapSize - allyBuilding.X, Tick.Game.MapSize - allyBuilding.Y),
                        AttackRange = allyBuilding.AttackRange,
                        Radius = allyBuilding.Radius,
                        CooldownTicks = allyBuilding.CooldownTicks
                    };
                    Buildings.Add(buildingInfo);

                    var towerType = EnemyBuildingType.Mid1;

                    switch (i)
                    {
                        case 0: towerType = EnemyBuildingType.Bot2;break;
                        case 1: towerType = EnemyBuildingType.Bot1;break;
                        case 2: towerType = EnemyBuildingType.FactionBase;break;
                        case 3: towerType = EnemyBuildingType.Mid2;break;
                        case 4: towerType = EnemyBuildingType.Top2;break;
                        case 5: towerType = EnemyBuildingType.Mid1;break;
                        case 6: towerType = EnemyBuildingType.Top1;break;
                    }

                    buildingInfo.EnemyBuildingType = towerType;

                    i++;
                }

               Buildings.First(x => x.EnemyBuildingType == EnemyBuildingType.Bot2).DependsOnBuilding = Buildings.First(x => x.EnemyBuildingType == EnemyBuildingType.Bot1);
               Buildings.First(x => x.EnemyBuildingType == EnemyBuildingType.Mid2).DependsOnBuilding = Buildings.First(x => x.EnemyBuildingType == EnemyBuildingType.Mid1);
               Buildings.First(x => x.EnemyBuildingType == EnemyBuildingType.Top2).DependsOnBuilding = Buildings.First(x => x.EnemyBuildingType == EnemyBuildingType.Top1);
            }

            //Buildings.Where(x => !x.IsDead).ToList().ForEach(x =>
            //{
            //    var red = (float)(x.CooldownTicks - x.RemainingActionCooldownTicks)/x.CooldownTicks;

            //    VisualClientHelper.Circle(x.Position.X,x.Position.Y, (float)x.AttackRange, red, 0, 0);
            //    //VisualClientHelper.Circle(x.Position.X,x.Position.Y, (float)x.Radius,0, 0, 1);
            //});
        }
    }

    public class EnemyBuildingInfo
    {
        public EnemyBuildingType EnemyBuildingType { get; set; }
        public bool IsDead { get; set; }
        public EnemyBuildingInfo DependsOnBuilding { get; set; }
        public Point2D Position { get; set; }
    
        public double AttackRange { get; set; }
        public double Radius { get; set; }
        public int CooldownTicks { get; set; }

        //Нельзя бить ,если предыдущий тауер не добит
        public bool CanBeDamaged => DependsOnBuilding == null || DependsOnBuilding.IsDead;

        public int RemainingActionCooldownTicks { get; set; }
    }

    public enum EnemyBuildingType
    {
        Top1,
        Top2,
        Mid1,
        Mid2,
        Bot1,
        Bot2,
        FactionBase
    }
}
