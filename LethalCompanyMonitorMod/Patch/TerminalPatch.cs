using System;
using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LethalCompanyMonitorMod.Patch
{

    [HarmonyPatch(typeof(Terminal))]
    public class TerminalPatch
    {

        [HarmonyPatch("ParsePlayerSentence")]
        [HarmonyPostfix]
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
                var keyPressed = Array.Find(Configs.AllBinds, k => Keyboard.current[k.ConfigEntry.Value].wasPressedThisFrame);

                if (keyPressed != null || keyPressed != default)
                {
                    int currentlyViewingPlayer = Plugin.CurrentlyViewingPlayer;
                    int maxSpectablePlayers = Plugin.SelectableObjects.Count;
                    List<int> indexes = Plugin.SelectableObjects.Select(x => x.Value).ToList();
                    int currentIndex = indexes.IndexOf(currentlyViewingPlayer);

                    Plugin.Log.LogInfo($"Method - HandleTerminalCameraNode | Currently Viewing Player: {currentlyViewingPlayer}");
                    Plugin.Log.LogInfo($"Method - HandleTerminalCameraNode | Currently Player Index: {currentIndex}");
                    Plugin.Log.LogInfo($"Method - HandleTerminalCameraNode | Currently Max Spectable Players: {maxSpectablePlayers}");


                    if (maxSpectablePlayers <= 1)
                    {
                        Plugin.CurrentlyViewingPlayer = Plugin.SelectableObjects.FirstOrDefault().Value;
                    }
                    else
                    {
                        switch (keyPressed.DefaultValue)
                        {

                            case Key.LeftArrow:
                                Plugin.CurrentlyViewingPlayer = currentIndex > 0 ? indexes[currentIndex - 1] : indexes.Last();
                                break;
                            case Key.RightArrow:
                                Plugin.CurrentlyViewingPlayer = currentIndex < (maxSpectablePlayers - 1) ? indexes[currentIndex + 1] : indexes.First();
                                break;
                            default:
                                break;

                        }
                    }
                    Plugin.Log.LogInfo("Method - HandleTerminalCameraNode | Switching to player " + Plugin.CurrentlyViewingPlayer);
                    UnityEngine.Object.FindObjectOfType<StartOfRound>().mapScreen.SwitchRadarTargetAndSync(Plugin.CurrentlyViewingPlayer);
                }
            }
        }
    }
}
