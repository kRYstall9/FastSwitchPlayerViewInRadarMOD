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
            if(Plugin.AmountOfPlayers == __instance.ClientPlayerList.Count)
            {
                return;
            }
            
            Plugin.AmountOfPlayers = __instance.ClientPlayerList.Count;
        }
    }
}
