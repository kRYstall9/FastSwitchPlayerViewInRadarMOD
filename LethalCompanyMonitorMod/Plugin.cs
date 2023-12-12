using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LethalCompanyMonitorMod
{
    [BepInPlugin(PluginStaticInfo.PLUGIN_GUID, PluginStaticInfo.PLUGIN_NAME, PluginStaticInfo.PLUGIN_VERSION)]
    [BepInProcess("Lethal Company.exe")]
    public class Plugin : BaseUnityPlugin
    {
        internal static Plugin Instance { get; private set; }
        internal static ManualLogSource Log { get; private set; }
        internal static bool ViewMonitorSubmitted { get;  set; } = false;
        internal static int CurrentlyViewingPlayer { get; set; } = 0;
        internal static InputActionAsset Asset;
        internal static string keyMappingPath = Application.persistentDataPath + "/switch_radar_cam.txt";
        internal static Dictionary<string, string> defaultKeys = new Dictionary<string, string>();

        private void Awake()
        {
            Instance = this;
            Log = base.Logger;


            foreach(var bind in Configs.AllBinds)
            {
                var hotkey = Config.Bind(
                        bind.Section,
                        bind.Key,
                        bind.DefaultValue,
                        bind.Description
                    );
                bind.ConfigEntry = hotkey;
            }

            defaultKeys.Add("Previous Cam" , "/Keyboard/LeftArrow");
            defaultKeys.Add("Next Cam", "/Keyboard/RightArrow");

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            Logger.LogInfo($"{PluginStaticInfo.PLUGIN_GUID} plugin has been loaded!");

        }
        
        public static void SetAsset(string previousCam, string nextCam)
        {
            Plugin.Asset = InputActionAsset.FromJson("\r\n                {\r\n                    \"maps\" : [\r\n                        {\r\n                            \"name\" : \"FastSwitchPlayerViewInRadar\",\r\n                            \"actions\": [\r\n                                {\"name\": \"Previous Cam\", \"type\" : \"button\"},\r\n                            {\"name\": \"Next Cam\", \"type\" : \"button\"}\r\n                            ],\r\n                            \"bindings\" : [\r\n                                {\"path\" : \"" + previousCam + "\", \"action\": \"Previous Cam\"},\r\n                            {\"path\" : \"" + nextCam + "\", \"action\": \"Next Cam\"}\r\n                            ]\r\n                        }\r\n                    ]\r\n                }");
        }
    }
}


