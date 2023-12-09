using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LethalCompanyMonitorMod.Patch
{
    [HarmonyPatch(typeof(RadarBoosterItem))]
    public class RadarBoosterPatch
    {
        [HarmonyPatch("RemoveBoosterFromRadar")]
        [HarmonyPostfix]
        public static void RemovingUnusedRadar(ref RadarBoosterItem __instance)
        {
            Plugin.SelectableObjects.Remove(__instance.radarBoosterName);
            Plugin.Log.LogInfo($"Method - RemovingUnusedRadar | {__instance.radarBoosterName} removed");
            Plugin.CurrentlyViewingPlayer = Plugin.SelectableObjects.FirstOrDefault().Value;
            UnityEngine.Object.FindObjectOfType<StartOfRound>().mapScreen.SwitchRadarTargetAndSync(Plugin.CurrentlyViewingPlayer);
        }
    }
}
