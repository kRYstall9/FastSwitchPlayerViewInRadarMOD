using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LethalCompanyMonitorMod.Patch
{
    [HarmonyPatch(typeof(ManualCameraRenderer))]
    public class ManualCameraRendererPatch
    {

        [HarmonyPatch("AddTransformAsTargetToRadar")]
        [HarmonyPostfix]
        static void GetSelectableObjects(ref ManualCameraRenderer __instance) 
        {
            int added = 0;
            for (int i = 0; i < __instance.radarTargets.Count; i++)
            {
                var target = __instance.radarTargets[i];

                if (target.isNonPlayer)
                {
                    Dictionary<string, int> r = new Dictionary<string, int>();
                    r.Add(target.name, i);
                    if (!Plugin.SelectableObjects.ContainsKey(target.name))
                    {
                        Plugin.SelectableObjects.Add(target.name, i);
                        added++;
                    }
                    else if(Plugin.SelectableObjects.ContainsKey(target.name))
                    {
                        Plugin.SelectableObjects[target.name] = i;
                    }
                    continue;
                }

                var targetComponent = target.transform.GetComponent<PlayerControllerB>();
                if (targetComponent.playerSteamId != 0 && !targetComponent.disconnectedMidGame && !Plugin.SelectableObjects.ContainsKey(targetComponent.playerUsername))
                {
                    Plugin.SelectableObjects.Add(targetComponent.playerUsername, i);
                    added++;
                }
                else if(
                        targetComponent.playerSteamId != 0 && !targetComponent.disconnectedMidGame && Plugin.SelectableObjects.ContainsKey(targetComponent.playerUsername))
                {
                    Plugin.SelectableObjects[targetComponent.playerUsername] = i;
                }
            }
            string text = added > 0 ? "players" : "player";
            Plugin.Log.LogInfo($"Method - GetSelectableObjects | {added} more {text} can be selected in the radar");

        }

    }
}
