﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
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
        private static void HandleSentence(Terminal __instance, TerminalNode __result)
        {
            if (__result == null || !__instance.terminalInUse || Plugin.ViewMonitorSubmitted)
                return;

            if (String.Compare(__result.name, "ViewInsideShipCam 1", true) == 0)
            {
                Plugin.ViewMonitorSubmitted = true;
            }
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void HandleTerminalCameraNode(Terminal __instance)
        {
            if (!__instance.terminalInUse)
            {
                return;
            }

            string bindsFromFile;
            List<string> bindsRead = new List<string>();
            if (File.Exists(Plugin.keyMappingPath))
            {
                bindsFromFile = File.ReadAllText(Plugin.keyMappingPath);
                bindsRead = bindsFromFile.Trim().Split('\n').ToList();
            }

            if(Plugin.Asset == null || !Plugin.Asset.enabled)
            {
                if (Plugin.Asset == null)
                {
                    Plugin.Log.LogInfo("Method - HandleTerminalCameraNode | InputActionAsset not yet created");
                    if (File.Exists(Plugin.keyMappingPath))
                    {
                        Plugin.SetAsset(bindsRead[0], bindsRead[1]);
                    }
                    else
                    {
                        Plugin.SetAsset(Plugin.defaultKeys["Previous Cam"], Plugin.defaultKeys["Next Cam"]);
                    }
                    Plugin.Log.LogInfo("Method - HandleTerminalCameraNode | InputActionAsset created successfully");
                }

                if (Plugin.Asset != null)
                {
                    Plugin.Log.LogInfo("Method - HandleTerminalCameraNode | InputActionAsset not yet enabled");
                    Plugin.Asset.Enable();
                    Plugin.Log.LogInfo("Method - HandleTerminalCameraNode | InputActionAsset enabled successfully");
                }
            }

            if (Plugin.ViewMonitorSubmitted)
            {
                try
                {
                    InputAction action = Plugin.Asset.Where(x => x.WasPressedThisFrame()).FirstOrDefault();


                    if (action != null || action != default)
                    {
                        int currentIndex = Plugin.CurrentlyViewingPlayer;
                        ManualCameraRenderer __camInstance = UnityEngine.Object.FindObjectOfType<StartOfRound>().mapScreen;
                        int maxSpectablePlayers = __camInstance.radarTargets.Count;

                        Plugin.Log.LogInfo($"Method - HandleTerminalCameraNode | Currently Max Spectable Players: {maxSpectablePlayers}");

                        PlayerControllerB player = null;
                        RadarBoosterItem radarBooster = null;

                        for (int i = 0; i < __camInstance.radarTargets.Count; i++)
                        {
                            switch (action.name)
                            {
                                case "Previous Cam":
                                    currentIndex = GetRadarTargetIndex(currentIndex, __camInstance.radarTargets.Count);
                                    Plugin.Log.LogInfo($"Method - HandleTerminalCameraNode | Current Index: {currentIndex}");
                                    break;
                                case "Next Cam":
                                    currentIndex = GetRadarTargetIndex(currentIndex, __camInstance.radarTargets.Count, true);
                                    Plugin.Log.LogInfo($"Method - HandleTerminalCameraNode | Current Index: {currentIndex}");
                                    break;
                                default:
                                    break;
                            }

                            if (__camInstance.radarTargets[currentIndex] == null)
                            {
                                Plugin.Log.LogInfo($"Method - HandleTerminalCameraNode | The player at the Index: {currentIndex} cannot be spectated. Getting the next player");
                                continue;
                            }
                            GameObject gameObject = __camInstance.radarTargets[currentIndex].transform.gameObject;
                            gameObject.TryGetComponent<PlayerControllerB>(out player);

                            if (player != null && (player.isPlayerControlled || player.isPlayerDead))
                            {
                                break;
                            }
                            else if (player == null || !player.isPlayerControlled)
                            {
                                gameObject.TryGetComponent<RadarBoosterItem>(out radarBooster);
                                if (radarBooster != null && radarBooster.radarEnabled)
                                {
                                    break;
                                }
                            }
                        }

                        if (maxSpectablePlayers > currentIndex && __camInstance.radarTargets[currentIndex] != null)
                        {
                            string playerUsername = "";
                            if (radarBooster != null)
                            {
                                playerUsername = radarBooster.radarBoosterName;
                            }
                            else
                            {
                                playerUsername = player.playerUsername;
                            }
                            Plugin.Log.LogInfo($"Method - HandleTerminalCameraNode | Switching cam to {playerUsername}");
                            __camInstance.SwitchRadarTargetAndSync(currentIndex);
                        }
                        Plugin.CurrentlyViewingPlayer = currentIndex;
                        player = null;
                        radarBooster = null;
                    }
                }
                catch(Exception e)
                {
                    Plugin.Log.LogInfo("Method - HandleTerminalCameraNode| Error: " + e.Message);
                }
            }
        }


        public static int GetRadarTargetIndex(int index, int loopableItemsCount, bool isIndexPlusOne = false)
        {
            int nextIndex;
            if (isIndexPlusOne)
            {
                nextIndex = index < (loopableItemsCount - 1) ? (index + 1) : 0;
            }
            else
            {
                nextIndex = index > 0 ? (index - 1) : (loopableItemsCount - 1);
            }
            return nextIndex;
        }
    }
}
