using System;
using System.Linq;
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
            if (!__instance.terminalInUse)
            {
                return;
            }

            if (Plugin.ViewMonitorSubmitted)
            {
                var keyPressed = Array.Find(CustomActions.AllBinds, k => Keyboard.current[k.ConfigEntry.Value].wasPressedThisFrame);

                if (keyPressed != null || keyPressed != default)
                {
                    ManualCameraRenderer __manualCameraRendererInstance = UnityEngine.Object.FindObjectOfType<StartOfRound>().mapScreen;
                    int currentlyViewingPlayer = Plugin.CurrentlyViewingPlayer;
                    int maxSpectablePlayers = Plugin.SelectableObjects.Count;
                    int currentIndex = Plugin.SelectableObjects.IndexOf(currentlyViewingPlayer);

                    switch (keyPressed.Hotkey)
                    {

                        case Key.LeftArrow:
                            Plugin.CurrentlyViewingPlayer = currentIndex > 0 ? Plugin.SelectableObjects[currentIndex - 1] : Plugin.SelectableObjects.Last();
                            break;
                        case Key.RightArrow:
                            Plugin.CurrentlyViewingPlayer = currentIndex < (maxSpectablePlayers - 1) ? Plugin.SelectableObjects[currentIndex + 1] : Plugin.SelectableObjects.First();                    
                            break;
                        default:
                            break;

                    }
                    UnityEngine.Object.FindObjectOfType<StartOfRound>().mapScreen.SwitchRadarTargetAndSync(Plugin.CurrentlyViewingPlayer);
                }
            }
        }

    }
}
