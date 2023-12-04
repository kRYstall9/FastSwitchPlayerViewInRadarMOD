using HarmonyLib;


namespace LethalCompanyMonitorMod.Patch
{
    [HarmonyPatch(typeof(StartOfRound))]
    public class StartOfRoundPatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void GetAmountOfPlayersConnected(ref StartOfRound __instance) 
        {
            if(Plugin.amountOfPlayers == __instance.ClientPlayerList.Count)
            {
                return;
            }
            
            Plugin.amountOfPlayers = __instance.ClientPlayerList.Count;
        }
    }
}
