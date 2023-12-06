using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.InputSystem;

namespace LethalCompanyMonitorMod
{
    [BepInPlugin("krystall9.FastSwitchPlayerViewInRadar", "FastSwitchPlayerViewInRadar", PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Lethal Company.exe")]
    public class Plugin : BaseUnityPlugin
    {
        internal static Plugin Instance { get; private set; }
        internal static ManualLogSource Log { get; private set; }
        internal static bool ViewMonitorSubmitted { get;  set; } = false;
        internal static int CurrentlyViewingPlayer { get; set; } = 0;
        internal static List<int> SelectableObjects { get; set; } = new List<int>();

        private void Awake()
        {
            Instance = this;
            Log = base.Logger;


            foreach(var bind in CustomActions.AllBinds)
            {
                var hotkey = Config.Bind(
                        bind.Section,
                        bind.KeyName,
                        bind.Hotkey,
                        bind.Description
                    );
                bind.ConfigEntry = hotkey;
            }

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            Logger.LogInfo($"krystall9.FastSwitchPlayerViewInRadar plugin has been loaded!");

        }
    }
}
