using System;
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
            
            string bindsFromFile;
            List<string> bindsRead = new List<string>();
            if (File.Exists(Plugin.keyMappingPath))
            {
                bindsFromFile = File.ReadAllText(Plugin.keyMappingPath);
                bindsRead = bindsFromFile.Trim().Split('\n').ToList();
            }

            if(Plugin.Asset == null || !Plugin.Asset.enabled)
            {
                Plugin.Log.LogInfo("Method - HandleTerminalCameraNode | InputActionAsset not yet created or enabled");
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
                }
                
                if(Plugin.Asset != null)
                {
                    Plugin.Log.LogInfo("Method - HandleTerminalCameraNode | InputActionAsset not yet enabled");
                    Plugin.Asset.Enable();
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
                        bool stopLoop = false;

                        Plugin.Log.LogInfo($"Method - HandleTerminalCameraNode | Currently Max Spectable Players: {maxSpectablePlayers}");

                        PlayerControllerB component = null;
                        RadarBoosterItem radarBooster = null;

                        for (int i = 0; i < __camInstance.radarTargets.Count; i++)
                        {
                            switch (action.name)
                            {
                                case "Previous Cam":
                                    currentIndex = GetRadarTargetIndex(currentIndex, __camInstance.radarTargets.Count);
                                    Plugin.Log.LogInfo($"Method - HandleTerminalCameraNode | Currently Index: {currentIndex}");


                                    if (__camInstance.radarTargets[currentIndex] == null)
                                    {
                                        Plugin.Log.LogInfo($"Method - HandleTerminalCameraNode | The player at the Index: {currentIndex} is null. Getting the nextIndex");
                                        continue;
                                    }

                                    component = __camInstance.radarTargets[currentIndex].transform.gameObject.GetComponent<PlayerControllerB>();

                                    if (component != null && (component.isPlayerControlled || component.isPlayerDead))
                                    {
                                        stopLoop = true;
                                        break;
                                    }
                                    else if (component == null || !component.isPlayerControlled)
                                    {
                                        radarBooster = __camInstance.radarTargets[currentIndex].transform.gameObject.GetComponent<RadarBoosterItem>();
                                        if (radarBooster != null && radarBooster.radarEnabled)
                                        {
                                            stopLoop = true;
                                            break;
                                        }
                                    }
                                    break;
                                case "Next Cam":

                                    currentIndex = GetRadarTargetIndex(currentIndex, __camInstance.radarTargets.Count, true);
                                    Plugin.Log.LogInfo($"Method - HandleTerminalCameraNode | Current Index: {currentIndex}");


                                    if (__camInstance.radarTargets[currentIndex] == null)
                                    {
                                        Plugin.Log.LogInfo($"Method - HandleTerminalCameraNode | The player at the Index: {currentIndex} is null. Getting the nextIndex");
                                        continue;
                                    }

                                    component = __camInstance.radarTargets[currentIndex].transform.gameObject.GetComponent<PlayerControllerB>();

                                    if (component != null && (component.isPlayerControlled || component.isPlayerDead))
                                    {
                                        stopLoop = true;
                                        break;
                                    }
                                    else if (component == null || !component.isPlayerControlled)
                                    {
                                        radarBooster = __camInstance.radarTargets[currentIndex].transform.gameObject.GetComponent<RadarBoosterItem>();
                                        if (radarBooster != null && radarBooster.radarEnabled)
                                        {
                                            stopLoop = true;
                                            break;
                                        }
                                    }
                                    break;

                                default:
                                    break;
                            }
                            if (stopLoop)
                                break;
                        }

                        if (__camInstance.radarTargets.Count > currentIndex && __camInstance.radarTargets[currentIndex] != null)
                        {
                            string playerUsername = "";
                            if (radarBooster != null)
                            {
                                playerUsername = radarBooster.radarBoosterName;
                            }
                            else
                            {
                                playerUsername = __camInstance.radarTargets[currentIndex].transform.gameObject.GetComponent<PlayerControllerB>().playerUsername;
                            }
                            Plugin.Log.LogInfo($"Method - HandleTerminalCameraNode | Switching cam to {playerUsername}");
                            __camInstance.SwitchRadarTargetAndSync(currentIndex);
                        }
                        Plugin.CurrentlyViewingPlayer = currentIndex;
                    }
                }
                catch(Exception e)
                {
                    Plugin.Log.LogInfo(e.Message);
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
