using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
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
        internal static Dictionary<string, int> SelectableObjects { get; set; } = new Dictionary<string, int>();

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

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            Logger.LogInfo($"krystall9.FastSwitchPlayerViewInRadar plugin has been loaded!");

        }

        public static List<string> disabledRadars()
        {
            return UnityEngine.Object.FindObjectsOfType<RadarBoosterItem>().Where(x=> x.radarEnabled == false).Select(x=> x.radarBoosterName).ToList();
        }
        public static void UpdateIndexes(List<TransformAndName> radarTargets)
        {
            int index = 0;
            foreach (var radarTarget in radarTargets)
            {
                if (Plugin.SelectableObjects.ContainsKey(radarTarget.name) && Plugin.SelectableObjects[radarTarget.name] != index)
                {
                    Plugin.SelectableObjects[radarTarget.name] = index;
                }
                index++;
            }
        }
    }
}
