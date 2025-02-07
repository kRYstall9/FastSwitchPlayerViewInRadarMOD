using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.InputSystem;

namespace LethalCompanyMonitorMod.Patch
{
    [HarmonyPatch(typeof(KepRemapPanel))]
    public class KeyRemapPanelPatch
    {

        [HarmonyPatch("LoadKeybindsUI")]
        [HarmonyPrefix]
        public static void LoadBindings(ref KepRemapPanel __instance)
        {
            string binds;
            List<string> bindings = new List<string>();
            if (!File.Exists(Plugin.keyMappingPath))
            {
                binds = Plugin.defaultKeys["Previous Cam"] + "\n" + Plugin.defaultKeys["Next Cam"];
                File.WriteAllText(Plugin.keyMappingPath, binds);
            }
            else
            {
                binds = File.ReadAllText(Plugin.keyMappingPath);
                bindings = binds.Trim().Split('\n').ToList();
            }

            foreach(var key in __instance.remappableKeys)
            {
                if(key.ControlName == "Radar Previous Cam" || key.ControlName == "Radar Next Cam")
                {
                    return;
                }
            }
            if(bindings.Count > 0)
            {
                Plugin.SetAsset(bindings[0], bindings[1]);
            }
            else
            {
                Plugin.SetAsset(Plugin.defaultKeys["Previous Cam"], Plugin.defaultKeys["Next Cam"]);
            }

            foreach(var key in Plugin.defaultKeys)
            {
                RemappableKey k = new RemappableKey();
                InputActionReference inputActionReference;
                Plugin.Log.LogInfo("Method - LoadBindings | Loading Keybindings for " + key.Key);
                switch (key.Key)
                {
                    case "Previous Cam":
                        inputActionReference = InputActionReference.Create(Plugin.Asset.FindAction(key.Key));
                        k.ControlName = "Radar Previous Cam";
                        k.currentInput = inputActionReference;
                        break;
                    case "Next Cam":
                        inputActionReference = InputActionReference.Create(Plugin.Asset.FindAction(key.Key));
                        k.ControlName = "Radar Next Cam";
                        k.currentInput = inputActionReference;
                        break;
                    default:
                        break;
                }
                __instance.remappableKeys.Add(k);
            }
        }
    }
}
