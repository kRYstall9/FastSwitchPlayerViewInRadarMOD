using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Linq;
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

            if (!Plugin.ViewMonitorSubmitted)
            {
                return;
            }

            try
            {
                InputAction action = Plugin.KeyBindingsInstance.Asset.Where(x=> x.triggered).FirstOrDefault();

                if (action == default)
                {
                    return;
                }

                if (Plugin.CameraRendererInstance == null)
                {
                    // Check if TwoRadarMaps Mod is being used
                    if (Plugin.TwoRadarMapsFound)
                    {
                        Plugin.Log.LogDebug("Method - HandleTerminalCameraNode | TwoRadarMaps Camera Found");
                        Plugin.CameraRendererInstance = __instance.GetComponents<ManualCameraRenderer>().Where(x => x.cam.name == "TerminalMapCamera").FirstOrDefault();
                    }
                    else
                    {
                        Plugin.Log.LogDebug("Method - HandleTerminalCameraNode | TwoRadarMaps Camera Not Found. Using Default Ship Camera");
                        Plugin.CameraRendererInstance = UnityEngine.Object.FindAnyObjectByType<StartOfRound>().mapScreen;
                    }
                }
                int currentIndex = Plugin.CurrentlyViewingPlayer;

                int maxSpectablePlayers = Plugin.CameraRendererInstance.radarTargets.Count;

                Plugin.Log.LogDebug($"Method - HandleTerminalCameraNode | Currently Max Spectable Players: {maxSpectablePlayers}");

                PlayerControllerB player = null;
                RadarBoosterItem radarBooster = null;

                for (int i = 0; i < maxSpectablePlayers; i++)
                {
                    switch (action.name)
                    {
                        case "PreviousPlayerCamKey":
                            currentIndex = GetRadarTargetIndex(currentIndex, maxSpectablePlayers);
                            Plugin.Log.LogDebug($"Method - HandleTerminalCameraNode | Current Index: {currentIndex}");
                            break;
                        case "NextPlayerCamKey":
                            currentIndex = GetRadarTargetIndex(currentIndex, maxSpectablePlayers, true);
                            Plugin.Log.LogDebug($"Method - HandleTerminalCameraNode | Current Index: {currentIndex}");
                            break;
                        default:
                            break;
                    }

                    if (Plugin.CameraRendererInstance.radarTargets[currentIndex] == null)
                    {
                        Plugin.Log.LogDebug($"Method - HandleTerminalCameraNode | The player at the Index: {currentIndex} cannot be spectated. Getting the next player");
                        continue;
                    }
                    GameObject gameObject = Plugin.CameraRendererInstance.radarTargets[currentIndex].transform.gameObject;
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

                if (maxSpectablePlayers > currentIndex && Plugin.CameraRendererInstance.radarTargets[currentIndex] != null)
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
                    Plugin.Log.LogDebug($"Method - HandleTerminalCameraNode | Switching cam to {playerUsername}");
                    Plugin.CameraRendererInstance.SwitchRadarTargetAndSync(currentIndex);
                }
                Plugin.CurrentlyViewingPlayer = currentIndex;
                player = null;
                radarBooster = null;
            }
            catch(Exception e)
            {
                Plugin.Log.LogError("Method - HandleTerminalCameraNode| Error: " + e.Message);
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
