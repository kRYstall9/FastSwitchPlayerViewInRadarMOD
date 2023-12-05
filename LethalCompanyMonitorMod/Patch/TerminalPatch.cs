using System;
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
                    Plugin.ViewMonitorSubmitted = true;
                    return;
                }
            }
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void HandleTerminalCameraNode(ref Terminal __instance)
        {
            if(!__instance.terminalInUse)
            {
                if (Plugin.ViewMonitorSubmitted)
                {
                    Plugin.CurrentlyViewingPlayer = 0;
                    Plugin.FirstTimeInViewMonitor = true;
                }
                return;
            }  

            if (Plugin.ViewMonitorSubmitted)
            {
                var keyPressed = Array.Find(CustomActions.AllBinds, k => Keyboard.current[k.ConfigEntry.Value].wasPressedThisFrame);

                if (keyPressed != null || keyPressed != default)
                {
                    int connectedPlayers = Plugin.AmountOfPlayers;
                    int currentlyViewingPlayer = Plugin.CurrentlyViewingPlayer;
                    
                    switch (keyPressed.Hotkey)
                    {

                        case Key.LeftArrow:
                            Plugin.CurrentlyViewingPlayer = currentlyViewingPlayer > 0 ? (currentlyViewingPlayer - 1) : (connectedPlayers - 1);
                            break;
                        case Key.RightArrow:
                            Plugin.CurrentlyViewingPlayer = currentlyViewingPlayer < (connectedPlayers - 1) ? (currentlyViewingPlayer + 1) : 0;
                            break;
                        default:
                            break;

                    }
                    Plugin.Log.LogInfo("Switching Radar to Player " + Plugin.CurrentlyViewingPlayer);
                    UnityEngine.Object.FindObjectOfType<StartOfRound>().mapScreen.SwitchRadarTargetAndSync(Plugin.CurrentlyViewingPlayer);
                }

                if (Plugin.FirstTimeInViewMonitor)
                {
                    UnityEngine.Object.FindObjectOfType<StartOfRound>().mapScreen.SwitchRadarTargetAndSync(0);
                    Plugin.FirstTimeInViewMonitor = false;
                }
            }
        }

    }
}
