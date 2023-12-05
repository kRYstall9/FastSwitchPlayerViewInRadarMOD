using BepInEx.Configuration;
using UnityEngine.InputSystem;

namespace LethalCompanyMonitorMod
{
    public class Keybinding
    {
        public Key Hotkey {  get; set; }
        public string Description {  get; set; }
        public string KeyName {  get; set; }
        public string Section { get; set; } 
        public ConfigEntry<Key> ConfigEntry { get; set; }
       
        public Keybinding(Key hotkey, string description, string keyName, string section)
        {
            Hotkey = hotkey;
            Description = description;
            KeyName = keyName;
            Section = section;
        }

    }
}
