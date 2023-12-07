using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
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
        internal static TextMeshProUGUI TerminalClockText { get; set; }
        internal static GameObject ClockObject { get; set; }
        internal static List<int> SelectableObjects { get; set; } = new List<int>();
        internal static List<ConfigEntry<Color>> ClockColors { get; set; } = new List<ConfigEntry<Color>> { };

        private void Awake()
        {
            Instance = this;
            Log = base.Logger;


            foreach(var bind in Configs.AllBinds)
            {
                var hotkey = Config.Bind(
                        bind.Section,
                        bind.Key,
                        bind.DefaultValue,
                        bind.Description
                    );
                bind.ConfigEntry = hotkey;
            }
            
            foreach(var clockColor in Configs.TerminalClockColors) 
            {
                var color = Config.Bind(
                        clockColor.Section,
                        clockColor.Key,
                        clockColor.DefaultValue,
                        clockColor.Description
                    );
                clockColor.ConfigEntry = color;
            }

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            Logger.LogInfo($"krystall9.FastSwitchPlayerViewInRadar plugin has been loaded!");

        }
    }
}
