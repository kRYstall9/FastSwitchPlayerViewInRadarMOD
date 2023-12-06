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
            int index = 0;
            for(int i=0; i< __instance.radarTargets.Count; i++)
            {
                var target = __instance.radarTargets[i];
                if(target.name.StartsWith("Player", StringComparison.OrdinalIgnoreCase))
                {
                    index++;
                    continue;
                }
                if(!Plugin.SelectableObjects.Contains(index))
                    Plugin.SelectableObjects.Add(index);
                index++;
            }
        }

    }
}
