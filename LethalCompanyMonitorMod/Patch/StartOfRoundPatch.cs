using GameNetcodeStuff;
using HarmonyLib;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace LethalCompanyMonitorMod.Patch
{
    [HarmonyPatch(typeof(StartOfRound))]
    public class StartOfRoundPatch
    {

        [HarmonyPatch("StartGame")]
        [HarmonyPostfix]
        public static void StartGamePostFix(ref StartOfRound __instance)
        {
            Plugin.SelectableObjects.Clear();

            int index = 0;
            foreach(var radarTarget in __instance.mapScreen.radarTargets)
            {
                if (!radarTarget.isNonPlayer)
                {
                    var playerComponent = radarTarget.transform.GetComponent<PlayerControllerB>();
                    if (playerComponent.playerSteamId != 0 && !playerComponent.disconnectedMidGame)
                    {
                        Plugin.SelectableObjects.Add(playerComponent.playerUsername, index);
                    }
                }      
                index++;
            }
            string text = Plugin.SelectableObjects.Count > 0 ? "players" : "player";
            Plugin.Log.LogInfo($"Method - StartGamePostFix | {Plugin.SelectableObjects.Count} {text} can be selected in the radar");
        }

        [HarmonyPatch("OnClientDisconnect")]
        [HarmonyPostfix]
        public static void PlayerDisconnected (ref StartOfRound __instance, ref ulong clientId)
        {
            Plugin.Log.LogInfo("Method - PlayerDisconnected | Deleting player from radar objects");
            Plugin.Log.LogInfo("PLAYER ID DISCONNECTED: " + clientId);

            var playerToDelete = __instance.allPlayerScripts.Where(x => x.disconnectedMidGame).ToList();

            foreach (var p in playerToDelete)
            {
                Plugin.Log.LogInfo("Player to delete: " + p.playerUsername);
                if (Plugin.SelectableObjects.Remove(p.playerUsername))
                {
                    Plugin.Log.LogInfo($"{p.playerUsername} successfully deleted");
                }
            }
            
            Plugin.CurrentlyViewingPlayer = Plugin.SelectableObjects.FirstOrDefault().Value;
            __instance.mapScreen.SwitchRadarTargetAndSync(Plugin.CurrentlyViewingPlayer);
            Plugin.UpdateIndexes(__instance.mapScreen.radarTargets);
        }
    }
}
