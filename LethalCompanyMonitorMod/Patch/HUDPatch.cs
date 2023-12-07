using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

namespace LethalCompanyMonitorMod.Patch
{
    [HarmonyPatch(typeof(HUDManager))]
    public class HUDPatch
    {

        [HarmonyPatch("SetClock")]
        [HarmonyPostfix]
        public static void SetClock(ref HUDManager __instance)
        {
            if(Plugin.ClockObject == null)
            {
                return;
            }

            ((TMP_Text)Plugin.TerminalClockText).text = ((TMP_Text)__instance.clockNumber).text.Replace('\n', ' ');
            string color = ChangeClockTextColor(Plugin.TerminalClockText.text);

            switch (color)
            {
                case "":
                    Plugin.TerminalClockText.color = Color.green;
                    break;
                default:
                    Color c = Configs.TerminalClockColors.Where(x => x.Key == color).FirstOrDefault().DefaultValue;
                    Plugin.TerminalClockText.color = c;
                    break;
            }
        }

        [HarmonyPatch("FillEndGameStats")]
        [HarmonyPostfix]
        public static void SetInSpace(ref HUDManager __instance)
        {
            ((TMP_Text)Plugin.TerminalClockText).text = "In Space";
            Plugin.TerminalClockText.color = Color.green;
        }

        public static string ChangeClockTextColor(string time) 
        {
            //As it is it will only work for 12h time format
            string[] timeSplitted = time.Split(':');

            int hour = int.Parse(timeSplitted[0]);
            string timeOfDay = timeSplitted[1].Split(' ')[1];

            if(hour >= 4 && hour < 8 && String.Compare(timeOfDay, "PM", true) == 0)
            {
                return "Afternoon";
            }
            
            else if((hour >= 8 && hour < 12) && String.Compare(timeOfDay, "PM", true) == 0)
            {
                return "Evening";
            }
            else if((hour >= 1 && hour < 6) && String.Compare(timeOfDay, "AM", true) == 0)
            {
                return "Night";
            }
            else if(hour >= 6 && String.Compare(timeOfDay, "AM", true)== 0)
            {
                return "Morning";
            }
                

            return "";
        }


    }
}
