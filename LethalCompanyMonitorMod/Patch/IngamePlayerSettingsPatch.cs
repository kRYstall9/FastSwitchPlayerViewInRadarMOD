using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LethalCompanyMonitorMod.Patch
{
    [HarmonyPatch(typeof(IngamePlayerSettings))]
    public class IngamePlayerSettingsPatch
    {
        [HarmonyPatch("CompleteRebind")]
        [HarmonyPrefix]
        public static void Rebind(IngamePlayerSettings __instance)
        {
            string actionName = __instance.rebindingOperation.action.name;

            Plugin.Log.LogInfo("Method - Rebind | Rebinding " + actionName);

            string newBind;
            string bindsFromFile;

            //Previous Cam bind is always the first string in the file
            switch (actionName)
            {
                case "Previous Cam":
                    bindsFromFile = File.ReadAllText(Plugin.keyMappingPath);
                    newBind = __instance.rebindingOperation.action.controls[0].path + "\n" + bindsFromFile.Trim().Split('\n')[1];
                    File.WriteAllText(Plugin.keyMappingPath, newBind);
                    break;

                case "Next Cam":
                    bindsFromFile = File.ReadAllText(Plugin.keyMappingPath);
                    newBind = bindsFromFile.Trim().Split('\n')[0] + "\n" +  __instance.rebindingOperation.action.controls[0].path;
                    File.WriteAllText(Plugin.keyMappingPath, newBind);
                    break;

                default:
                    break;
            }
            if(File.Exists(Plugin.keyMappingPath))
            {
                bindsFromFile = File.ReadAllText(Plugin.keyMappingPath);
                string prevCamBind = bindsFromFile.Trim().Split('\n')[0];
                string nextCamBind = bindsFromFile.Trim().Split('\n')[1];
                Plugin.SetAsset(prevCamBind, nextCamBind);
                return;
            }
                Plugin.SetAsset(Plugin.defaultKeys["Previous Cam"], Plugin.defaultKeys["Next Cam"]);
            }
        }
    }
}
