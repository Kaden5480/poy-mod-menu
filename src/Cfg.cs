using BepInEx.Configuration;
using UILib;
using UnityEngine;

using ModMenu.Config;

namespace ModMenu {
    /**
     * <summary>
     * Mod menu's own config.
     * </summary>
     */
    internal class Cfg {
        [Field("Auto-show Info")]
        internal ConfigEntry<bool> autoShowInfo { get; private set; }

        [Field("Auto Search")]
        internal ConfigEntry<bool> autoSearch { get; private set; }

        [Field("Enable Thumbnail Downloads")]
        internal ConfigEntry<bool> enableThumbnailDownloads { get; private set; }

        // Toggle shortcut
        [Field("Toggle Keybind")]
        internal ConfigEntry<KeyCode> toggleKeybind { get; private set; }

        // Extra custom read-only fields
        [Category("Keybinds")]
        [Field("Go Back", "The keybind to go back.")]
        private const string goBack = "Escape";

        [Category("Keybinds")]
        [Field("Search", "The keybind to start searching.")]
        private const string search = "Ctrl + F";

        /**
         * <summary>
         * Initializes the config.
         * </summary>
         * <param name="configFile">The config file to bind to</param>
         */
        internal Cfg(ConfigFile configFile) {
            autoShowInfo = configFile.Bind(
                "General", "autoShowInfo", true,
                "Whether info should automatically display when opening a view."
            );
            autoSearch = configFile.Bind(
                "General", "autoSearch", true,
                "Whether to automatically search while typing the search query."
            );

            toggleKeybind = configFile.Bind(
                "Keybinds", "toggleKeybind", KeyCode.Home,
                "The keybind to quickly toggle Mod Menu."
            );

            enableThumbnailDownloads = configFile.Bind(
                "Privacy", "enableThumbnailDownloads", true,
                "Whether mod thumbnails can be downloaded."
            );
        }
    }
}
