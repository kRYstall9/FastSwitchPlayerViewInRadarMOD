using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

namespace LethalCompanyMonitorMod
{
    [BepInPlugin("krystall9.FastSwitchPlayerViewInRadar", "FastSwitchPlayerViewInRadar", "1.0.0")]
    [BepInProcess("Lethal Company.exe")]
    public class Plugin : BaseUnityPlugin
    {
        internal static Plugin Instance { get; private set; }
        internal static ManualLogSource Log { get; private set; }
        internal static int amountOfPlayers { get; set; } = 0;
        internal static bool viewMonitorSubmitted { get;  set; } = false;

        private void Awake()
        { 
            Instance = this;
            Log = base.Logger;

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            Logger.LogInfo($"krystall9.FastSwitchPlayerViewInRadar plugin has been loaded!");

        }
    }
}
