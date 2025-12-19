using BepInEx.Configuration;
using UnityEngine;

using ModMenu.Config;

namespace ModMenu {
    /**
     * <summary>
     * Mod menu's own config.
     * </summary>
     */
    internal class Cfg {
        [Field("Toggle Keybind")]
        internal ConfigEntry<KeyCode> toggleKeybind         { get; private set; }

        [Field("Auto-show Mod Info")]
        internal ConfigEntry<bool> autoShowModInfo          { get; private set; }

        [Field("Enable Thumbnail Downloads")]
        internal ConfigEntry<bool> enableThumbnailDownloads { get; private set; }

        /**
         * <summary>
         * Initializes the config.
         * </summary>
         * <param name="configFile">The config file to bind to</param>
         */
        internal Cfg(ConfigFile configFile) {
            toggleKeybind = configFile.Bind(
                "General", "toggleKeybind", KeyCode.Home,
                "The keybind to quickly toggle mod menu"
            );
            autoShowModInfo = configFile.Bind(
                "General", "autoShowModInfo", true,
                "Whether mod info should automatically display when opening a mod page"
            );
            enableThumbnailDownloads = configFile.Bind(
                "Privacy", "enableThumbnailDownloads", true,
                "Whether mod thumbnails can be downloaded"
            );
        }
    }
}
