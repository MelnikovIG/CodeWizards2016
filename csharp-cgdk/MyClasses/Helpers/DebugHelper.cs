using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Basic;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Helpers
{
    public static class DebugHelper
    {
        private static bool DebugEnabled = false;
        //private static bool DebugEnabled = true;
        private static readonly long[] DebugWizardIds = {1};

        public static bool CanDebug = DebugEnabled && (DebugWizardIds == null || Array.IndexOf(DebugWizardIds, Tick.Self.Id) >= 0);

        public static void ConsoleWriteLite(string s)
        {
            if (CanDebug)
            {
                Console.WriteLine(s);
            }
        }
    }
}
