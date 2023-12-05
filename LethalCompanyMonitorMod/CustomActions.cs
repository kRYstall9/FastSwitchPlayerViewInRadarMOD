using System;
using System.Collections.Generic;
using System.Text;

namespace LethalCompanyMonitorMod
{
    public class CustomActions
    {
        public static readonly Keybinding PreviousPlayerCam = new Keybinding(UnityEngine.InputSystem.Key.LeftArrow, "Get previous player's cam", "PrevPCam", "HotKeys");
        public static readonly Keybinding NextPlayerCam = new Keybinding(UnityEngine.InputSystem.Key.RightArrow, "Get next player's cam", "NextPCam", "HotKeys");

        public static readonly Keybinding[] AllBinds = {PreviousPlayerCam, NextPlayerCam};

    }
}
