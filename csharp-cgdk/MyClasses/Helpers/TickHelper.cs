using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Helpers
{
    public static class Tick
    {
        public static Wizard Self { get; set; }
        public static World World { get; set; }
        public static Game Game { get; set; }
        public static Move Move { get; set; }

        public static void UpdateTick(Wizard self, World world, Game game, Move move)
        {
            Self = self;
            World = world;
            Game = game;
            Move = move;
        }
    }
}
