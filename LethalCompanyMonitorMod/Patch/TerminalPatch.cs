using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine.InputSystem;

namespace LethalCompanyMonitorMod.Patch
{

    [HarmonyPatch(typeof(Terminal))]
    public class TerminalPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("ParsePlayerSentence")]
        private static void HandleSentence(ref Terminal __instance)
        {
            if (!__instance.terminalInUse)
                return;

            string text = __instance.screenText.text.Substring(__instance.screenText.text.Length - __instance.textAdded);
            if (text.Length > 0)
            {
                if (String.Compare(text, "view monitor", true) == 0)
                {
                    Plugin.Log.LogInfo("Radar Map Activated");
                    Plugin.viewMonitorSubmitted = true;
                    return;
                }
            }
            Plugin.viewMonitorSubmitted = false;
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void HandleTerminalCameraNode(ref Terminal __instance)
        {
            if(!__instance.terminalInUse)
            {
                if (Plugin.viewMonitorSubmitted)
                {
                    Plugin.viewMonitorSubmitted = false;
                }
                return;
            }
            if (Plugin.viewMonitorSubmitted)
            {
                foreach (KeyValuePair<int, Key> kvp in Keybindings.allowedKeys)
                {
                    if (Keyboard.current[kvp.Value].wasPressedThisFrame && kvp.Key < Plugin.amountOfPlayers)
                    {
                        Plugin.Log.LogInfo("Switching Radar to Player " + kvp.Key);
                        UnityEngine.Object.FindObjectOfType<StartOfRound>().mapScreen.SwitchRadarTargetAndSync(kvp.Key);
                        return;
                    }
                }
            }
        }

    }
}
