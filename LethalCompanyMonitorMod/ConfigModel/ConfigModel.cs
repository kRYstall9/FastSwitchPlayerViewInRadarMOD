using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace LethalCompanyMonitorMod.ConfigModel
{
    public class ConfigModel<T>
    {
        public T DefaultValue { get; set; }
        public string Description { get; set; }
        public string Key { get; set; }
        public string Section { get; set; }
        public ConfigEntry<T> ConfigEntry { get; set; }

        public ConfigModel(T defaultValue, string description, string key, string section) 
        {
            DefaultValue = defaultValue;
            Description = description;
            Key = key;
            Section = section;
        }
    }
}
