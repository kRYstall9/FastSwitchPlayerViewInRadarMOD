using System;
using System.Linq;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LethalCompanyMonitorMod.Patch
{

    [HarmonyPatch(typeof(Terminal))]
    public class TerminalPatch
    {

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void CreateClockObjectInstance(ref Terminal __instance)
        {
            Transform terminalContainer = ((Component)__instance).transform.parent.parent.Find("Canvas").Find("MainContainer");
            try
            {
                if(Plugin.ClockObject == null)
                {
                    Plugin.Log.LogInfo("In the catch ");
                    Plugin.ClockObject = UnityEngine.Object.Instantiate<GameObject>(((Component)terminalContainer.Find("CurrentCreditsNum")).gameObject, terminalContainer);
                    ((UnityEngine.Object)Plugin.ClockObject).name = "ClockContainer";
                    Plugin.ClockObject.transform.localPosition = new Vector3(240f, 205f, -1f);
                    Plugin.ClockObject.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
                    Plugin.TerminalClockText = Plugin.ClockObject.GetComponent<TextMeshProUGUI>();
                    ((TMP_Text)Plugin.TerminalClockText).text = " ";
                    return;
                }
                Plugin.ClockObject = ((Component)terminalContainer.Find("ClockContainer")).gameObject;
                Plugin.TerminalClockText = Plugin.ClockObject.GetComponent<TextMeshProUGUI>();
            }
            catch
            {
                Plugin.Log.LogError("Error while creating the clock object instance");
                Plugin.ClockObject = null;
                Plugin.TerminalClockText = null;
            }
        }


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
                var keyPressed = Array.Find(Configs.AllBinds, k => Keyboard.current[k.ConfigEntry.Value].wasPressedThisFrame);

                if (keyPressed != null || keyPressed != default)
                {
                    ManualCameraRenderer __manualCameraRendererInstance = UnityEngine.Object.FindObjectOfType<StartOfRound>().mapScreen;
                    int currentlyViewingPlayer = Plugin.CurrentlyViewingPlayer;
                    int maxSpectablePlayers = Plugin.SelectableObjects.Count;
                    int currentIndex = Plugin.SelectableObjects.IndexOf(currentlyViewingPlayer);
                    
                    if(maxSpectablePlayers <= 1)
                    {
                        Plugin.CurrentlyViewingPlayer = currentlyViewingPlayer;
                    }
                    else
                    {
                        switch (keyPressed.DefaultValue)
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
                    }
                    
                    UnityEngine.Object.FindObjectOfType<StartOfRound>().mapScreen.SwitchRadarTargetAndSync(Plugin.CurrentlyViewingPlayer);
                }
            }
        }
    }
}
