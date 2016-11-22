using System.Globalization;
using System.Threading;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Helpers;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk
{
    public sealed class MyStrategy : IStrategy
    {
        public MyStrategy()
        {
            DebugTrace.ExecuteVisualizer(() =>
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                Debug.connect("127.0.0.1", 13579);
            });
        }

        public void Move(Wizard self, World world, Game game, Move move)
        {
            Tick.UpdateTick(self, world, game, move);
            Strategy.Execute();

            DebugTrace.ExecuteVisualizer(() =>
            {
                Debug.beginPost();
                Debug.circle(Tick.Self.X, Tick.Self.Y, Tick.Self.CastRange, 50);
                Debug.endPost();
            });
        }
    }
}