using System;
using System.Collections.Generic;

using BepInEx;
using ModMenu.Config;
using ModMenu.Parsing;

namespace ModMenu {
    [BepInPlugin("com.github.Kaden5480.poy-mod-menu", "Mod Menu", PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin {
        private static Plugin instance;

        /**
         * <summary>
         * Executes when the plugin is being loaded.
         * </summary>
         */
        private void Awake() {
            instance = this;
        }

        /**
         * <summary>
         * Executes when the plugin has started.
         * </summary>
         */
        private void Start() {
            foreach (KeyValuePair<BaseUnityPlugin, ModInfo> entry in ModManager.mods) {
                LogDebug($"Looking at: \"{entry.Key}\": \"{entry.Value.name}\"");
                entry.Value.Generate();

                foreach (KeyValuePair<string, List<BaseField>> category in entry.Value.config) {
                    LogDebug($"> {category.Key}:");

                    foreach (BaseField field in category.Value) {
                        LogDebug($"  - {field.name}, {field.fieldType}, {field.value}");
                    }
                }
            }
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
