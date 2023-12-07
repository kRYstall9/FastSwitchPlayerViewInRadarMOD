using BepInEx.Configuration;
using LethalCompanyMonitorMod.ConfigModel;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LethalCompanyMonitorMod
{
    public class Configs
    {
        public static readonly ConfigModel<Key> PreviousPlayerCam = new ConfigModel<Key>(UnityEngine.InputSystem.Key.LeftArrow, "Get previous player's cam", "PrevPCam", "HotKeys");
        public static readonly ConfigModel<Key> NextPlayerCam = new ConfigModel<Key>(UnityEngine.InputSystem.Key.RightArrow, "Get next player's cam", "NextPCam", "HotKeys");
        public static readonly ConfigModel<Color> AfternoonClockColor = new ConfigModel<Color>(new Color(1f, 0.65f, 0f), "Changes the terminal clock color when it is afternoon. Default Color: Orange", "Afternoon", "ClockColors");
        public static readonly ConfigModel<Color> EveningClockColor = new ConfigModel<Color>(new Color(0.68f, 0f, 0f), "Changes the terminal clock color when it gets late. Default Color: Red", "Evening", "ClockColors");
        public static readonly ConfigModel<Color> MorningClockColor = new ConfigModel<Color>(new Color(0f, 1f, 0f), "Changes the terminal clock color at game start. Default Color: Green", "Morning", "ClockColors");
        public static readonly ConfigModel<Color> NightClockColor = new ConfigModel<Color>(new Color(0.68f, 0f, 0f), "Changes the terminal clock color when it gets night. Default Color: Red", "Night", "ClockColors");




        public static readonly ConfigModel<Key>[] AllBinds = {PreviousPlayerCam, NextPlayerCam};
        public static readonly ConfigModel<Color>[] TerminalClockColors = { AfternoonClockColor, EveningClockColor, MorningClockColor, NightClockColor};


    }
}
