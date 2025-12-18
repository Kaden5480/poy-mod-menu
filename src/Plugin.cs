using System;
using System.Collections.Generic;

using BepInEx;
using UILib;
using UILib.Patches;

using ModMenu.Config;
using ModMenu.Parsing;

namespace ModMenu {
    [BepInPlugin("com.github.Kaden5480.poy-mod-menu", "Mod Menu", PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin {
        internal static Plugin instance { get; private set; }
        internal static UI ui { get; private set; }

        /**
         * <summary>
         * Executes when the plugin is being loaded.
         * </summary>
         */
        private void Awake() {
            instance = this;

            UIRoot.onInit.AddListener(() => {
                ui = new UI();
            });

            SceneLoads.onLoad.AddListener(
                Patches.MenuButtons.Inject
            );
        }

        /**
         * <summary>
         * Logs a debug message.
         * </summary>
         * <param name="message">The message to log</param>
         */
        internal static void LogDebug(string message) {
#if DEBUG
            if (instance == null) {
                Console.WriteLine($"[Debug] ModMenu: {message}");
                return;
            }

            instance.Logger.LogInfo(message);
#else
            if (instance != null) {
                instance.Logger.LogDebug(message);
            }
#endif
        }

        /**
         * <summary>
         * Logs an informational message.
         * </summary>
         * <param name="message">The message to log</param>
         */
        internal static void LogInfo(string message) {
            if (instance == null) {
                Console.WriteLine($"[Info] ModMenu: {message}");
                return;
            }
            instance.Logger.LogInfo(message);
        }

        /**
         * <summary>
         * Logs an error message.
         * </summary>
         * <param name="message">The message to log</param>
         */
        internal static void LogError(string message) {
            if (instance == null) {
                Console.WriteLine($"[Error] ModMenu: {message}");
                return;
            }
            instance.Logger.LogError(message);
        }
    }
}
