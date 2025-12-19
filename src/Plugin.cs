using System;

using BepInEx;
using HarmonyLib;
using UILib;
using UILib.Patches;
using UnityEngine;

namespace ModMenu {
    [BepInPlugin("com.github.Kaden5480.poy-mod-menu", "Mod Menu", PluginInfo.PLUGIN_VERSION)]
    internal class Plugin : BaseUnityPlugin {
        internal static Plugin instance { get; private set; }
        internal static Cfg config      { get; private set; }
        internal static UI ui           { get; private set; }

        /**
         * <summary>
         * Executes when the plugin is being loaded.
         * </summary>
         */
        private void Awake() {
            instance = this;

            config = new Cfg(this.Config);
            Harmony.CreateAndPatchAll(typeof(Patches.ControlMenu));

            UIRoot.onInit.AddListener(() => {
                ui = new UI();
            });

            SceneLoads.onLoad.AddListener(
                Patches.MenuButtons.Inject
            );

            // Register with self
            ModInfo info = ModManager.Register(this);
            info.Add(config);
            info.license = "GPLv3.0";
        }

        /**
         * <summary>
         * Executes when the plugin has started.
         * </summary>
         */
        private void Start() {
            // Register mods which haven't registered themselves
            foreach (BaseUnityPlugin mod in GameObject.FindObjectsOfType<BaseUnityPlugin>()) {
                if (ModManager.mods.ContainsKey(mod) == true) {
                    continue;
                }

                ModInfo info = ModManager.Register(mod);

                // Add the config entries
                info.Add(mod.Config);
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
