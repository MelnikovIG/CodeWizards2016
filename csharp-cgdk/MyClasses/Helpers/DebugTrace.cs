using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Basic;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Helpers
{
    public static class DebugTrace
    {
        public static void ConsoleWriteLite(string s)
        {
            Console.WriteLine(s);
        }

        public static void ExecuteVisualizer(Action action)
        {
#if VISUALIZER
            action?.Invoke();
#endif
        }
    }
}
