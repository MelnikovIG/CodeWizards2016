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
                    .Where(x => Tick.Self.GetDistanceTo(x) - x.Radius - Tick.Self.Radius < 5).ToList();

            return nearUnits;
        }

        public static List<Building> GetNearestEnemyBuidigs(double range)
        {
            List<Building> targets = new List<Building>();
            targets.AddRange(Tick.World.Buildings.Where(x => x.Faction != Tick.Self.Faction));
            return targets.Where(x => Tick.Self.GetDistanceTo(x) <= range).ToList();
        }

        public static List<Building> GetNearestFriendBuidigs(double range)
        {
            List<Building> targets = new List<Building>();
            targets.AddRange(Tick.World.Buildings.Where(x => x.Faction == Tick.Self.Faction));
            return targets.Where(x => Tick.Self.GetDistanceTo(x) <= range).ToList();
        }

        public static List<Wizard> GetNearestEnemyWizards(double range)
        {
            List<Wizard> targets = new List<Wizard>();
            targets.AddRange(Tick.World.Wizards.Where(x => x.Faction != Tick.Self.Faction));
            return targets.Where(x => Tick.Self.GetDistanceTo(x) <= range).ToList();
        }

        public static List<Wizard> GetNearestFriendWizards(double range)
        {
            List<Wizard> targets = new List<Wizard>();
            targets.AddRange(Tick.World.Wizards.Where(x => x.Faction == Tick.Self.Faction));
            return targets.Where(x => Tick.Self.GetDistanceTo(x) <= range).ToList();
        }

        public static List<Minion> GetNearestEnemyMinions(double range)
        {
            List<Minion> targets = new List<Minion>();
            targets.AddRange(Tick.World.Minions.Where(x => x.Faction != Tick.Self.Faction));
            return targets.Where(x => Tick.Self.GetDistanceTo(x) <= range).ToList();
        }

        public static List<Minion> GetNearestFriendMinions(double range)
        {
            List<Minion> targets = new List<Minion>();
            targets.AddRange(Tick.World.Minions.Where(x => x.Faction == Tick.Self.Faction));
            return targets.Where(x => Tick.Self.GetDistanceTo(x) <= range).ToList();
        }
    }
}
