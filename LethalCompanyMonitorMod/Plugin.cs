using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace LethalCompanyMonitorMod
{
    [BepInPlugin(PluginStaticInfo.PLUGIN_GUID, PluginStaticInfo.PLUGIN_NAME, PluginStaticInfo.PLUGIN_VERSION)]
    [BepInDependency("com.rune580.LethalCompanyInputUtils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInProcess("Lethal Company.exe")]
    public class Plugin : BaseUnityPlugin
    {
        internal static Plugin Instance { get; private set; }
        internal static ManualLogSource Log { get; private set; }
        internal static bool ViewMonitorSubmitted { get;  set; } = false;
        internal static int CurrentlyViewingPlayer { get; set; } = 0;
        internal static ManualCameraRenderer CameraRendererInstance { get; set; } = null;
        internal static KeyBindings KeyBindingsInstance;
        internal static bool TwoRadarMapsFound { get; set; } = false;

        private void Awake()
        {
            Instance = this;
            Log = base.Logger;
            KeyBindingsInstance = new();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            Logger.LogInfo($"{PluginStaticInfo.PLUGIN_NAME} plugin is loaded!");

        }
    }
}


