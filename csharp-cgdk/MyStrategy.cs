using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Helpers;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk
{
    public sealed class MyStrategy : IStrategy
    {
        public void Move(Wizard self, World world, Game game, Move move)
        {
            Tick.UpdateTick(self, world, game, move);
            Strategy.Execute();
        }
    }
}