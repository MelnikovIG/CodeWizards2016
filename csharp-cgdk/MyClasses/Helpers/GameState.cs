using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Helpers
{
    public static class GameState
    {
        public static WayPoint LastVisitedWaypoint;

        //Скольк тиков находимся в застрявшем состоянии
        public static long StackedTickCount { get; set; }
    }
}
