﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Basic;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Helpers
{
    public static class GameState
    {
        //Скольк тиков находимся в застрявшем состоянии
        public static long StackedTickCount { get; set; }

        //Последние координаты волщебников
        public static Dictionary<long, WizardInfo> WizardsLastPositions { get; set; } = new Dictionary<long, WizardInfo>();

        //Информация о начале и конце выстрела
        public static Dictionary<long, ProjectilesInfo> ProjectilesInfo { get; set; } = new Dictionary<long, ProjectilesInfo>();
    }

    public class ProjectilesInfo
    {
        public Point2D StartPoint { get; set; }
        public Point2D EndPoint { get; set; }
        public bool IsInterselect { get; set; }
        public Point2D NormalInterselectionPoint { get; set; }
    }

    public class WizardInfo
    {
        public Point2D Position { get; set; }
    }
}
