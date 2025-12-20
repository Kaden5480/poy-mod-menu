using BepInEx.Configuration;
using UILib.Behaviours;
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
        [Listener(typeof(Cfg), nameof(UpdateKeybind))]
        [Field("Toggle Keybind")]
        internal ConfigEntry<KeyCode> toggleKeybind { get; private set; }

        // The shortcut holding the keybind
        internal Shortcut toggleShortcut;

        // Extra custom read-only fields
        [Category("Keybinds")]
        [Field("Go Back")]
        private const string goBack = "Escape";

        [Category("Keybinds")]
        [Field("Search")]
        private const string search = "Ctrl+F";

        /**
         * <summary>
         * Immediately updates Mod Menu's toggle keybind.
         * </summary>
         * <param name="keyCode">The new KeyCode to use</param>
         */
        internal static void UpdateKeybind(KeyCode keyCode) {
            Plugin.config.toggleShortcut.SetShortcut(new[] { keyCode });
        }

        /**
         * <summary>
         * Initializes the config.
         * </summary>
         * <param name="configFile">The config file to bind to</param>
         */
        internal Cfg(ConfigFile configFile) {
            autoShowInfo = configFile.Bind(
                "General", "autoShowInfo", true,
                "Whether info should automatically display when opening a view"
            );
            autoSearch = configFile.Bind(
                "General", "autoSearch", true,
                "Whether to automatically search while typing the search query"
            );

            toggleKeybind = configFile.Bind(
                "Keybinds", "toggleKeybind", KeyCode.Home,
                "The keybind to quickly toggle Mod Menu"
            );

            enableThumbnailDownloads = configFile.Bind(
                "Privacy", "enableThumbnailDownloads", true,
                "Whether mod thumbnails can be downloaded"
            );
        }
    }
}
