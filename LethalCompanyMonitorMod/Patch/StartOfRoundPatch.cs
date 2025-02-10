using BepInEx.Bootstrap;
using HarmonyLib;
using System;
using System.Linq;

namespace LethalCompanyMonitorMod.Patch
{
    [HarmonyPatch(typeof(StartOfRound))]
    public class StartOfRoundPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void HandleStartOfRound()
        {
            bool twoRadarMapsFound = (Chainloader.PluginInfos.Where(pluginInfo => pluginInfo.Value.Metadata.Name.Contains("tworadarmap", StringComparison.InvariantCultureIgnoreCase)).Any());
            Plugin.TwoRadarMapsFound = twoRadarMapsFound;
        }
    }
}
