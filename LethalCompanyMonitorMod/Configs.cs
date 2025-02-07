using LethalCompanyMonitorMod.ConfigModel;
using UnityEngine.InputSystem;

namespace LethalCompanyMonitorMod
{
    public class Configs
    {
        public static readonly ConfigModel<Key> PreviousPlayerCam = new ConfigModel<Key>(UnityEngine.InputSystem.Key.LeftArrow, "Get previous player's cam", "PrevPCam", "HotKeys");
        public static readonly ConfigModel<Key> NextPlayerCam = new ConfigModel<Key>(UnityEngine.InputSystem.Key.RightArrow, "Get next player's cam", "NextPCam", "HotKeys");

        public static readonly ConfigModel<Key>[] AllBinds = {PreviousPlayerCam, NextPlayerCam};



    }
}
