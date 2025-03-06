using System.Collections.Generic;

using BepInEx.Configuration;

namespace ModMenu.Wrappers {
    public class BepInConfig : Config {
        private ConfigFile config;

        public BepInConfig(string name, ConfigFile config) : base(name) {
            this.config = config;

            foreach (KeyValuePair<ConfigDefinition, ConfigEntryBase> pair in config) {
                ConfigDefinition definition = pair.Key;
                ConfigEntryBase entry = pair.Value;

                Category category = GetAddCategory(definition.Section);
                category.AddEntry(new BepInEntry(entry));
            }
        }

        public override void Save() {
            config.Save();
        }
    }
}
