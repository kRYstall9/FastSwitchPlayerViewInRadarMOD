using HarmonyLib;

namespace LethalCompanyMonitorMod.Patch
{
    [HarmonyPatch(typeof(RadarBoosterItem))]
    public class RadarBoosterPatch
    {
        [HarmonyPatch("RemoveBoosterFromRadar")]
        [HarmonyPostfix]
        public static void RemovingUnusedRadar(ref RadarBoosterItem __instance)
        {
            var __camInstance = UnityEngine.Object.FindObjectOfType<StartOfRound>().mapScreen;
            Plugin.Log.LogInfo("Method - RemovingUnusedRadar | Current cam target name: " + __instance.radarBoosterName);
            Plugin.Log.LogInfo($"Method - RemovingUnusedRadar | {__instance.radarBoosterName} removed");
            Plugin.Log.LogInfo("Method - RemovingUnusedRadar | Updating camera target");
            
            for (int i = 0; i < __camInstance.radarTargets.Count; i++) 
            {
                if (__camInstance.radarTargets.Count <= Plugin.CurrentlyViewingPlayer || __camInstance.radarTargets[Plugin.CurrentlyViewingPlayer] == null)
                {
                    Plugin.CurrentlyViewingPlayer = i;
                    continue;
                }

                __camInstance.SwitchRadarTargetAndSync(Plugin.CurrentlyViewingPlayer);
                Plugin.Log.LogInfo($"Method - RemovingUnusedRadar | Currently targeting {__camInstance.radarTargets[Plugin.CurrentlyViewingPlayer].name}");
                return;

            }
            
        }
    }
}
