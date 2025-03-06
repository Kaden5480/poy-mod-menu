using System;
using System.Collections.Generic;

using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

#if BEPINEX
using BepInEx;

namespace ModMenu {
    [BepInPlugin("com.github.Kaden5480.poy-mod-menu", "ModMenu", PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin {
        private bool hasLoaded = false;
        private Dictionary<string, Wrappers.Config> configs = new Dictionary<string, Wrappers.Config>();

        public void Awake() {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void OnDestroy() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void LoadConfigs() {
            if (hasLoaded == true) {
                return;
            }

            foreach (BaseUnityPlugin plugin in GameObject.FindObjectsOfType<BaseUnityPlugin>()) {
                string name = plugin.Info.Metadata.Name;
                configs[name] = new Wrappers.BepInConfig(name, plugin.Config);
            }

            hasLoaded = true;
            Console.WriteLine($"Mod configs loaded: {configs.Count}");
        }

        private void ShowConfigs() {
            foreach (KeyValuePair<string, Wrappers.Config> config in configs) {
                Console.WriteLine("");
                Console.WriteLine($"== Mod: {config.Key} ==");

                foreach (KeyValuePair<string, Wrappers.Category> category in config.Value.categories) {
                    Console.WriteLine($"-- Category: {category.Key} --");

                    foreach (KeyValuePair<string, Wrappers.Entry> entry in category.Value.entries) {
                        Console.WriteLine($"[{entry.Value.type}] {entry.Value.name} -> {entry.Value.value}");
                    }
                }
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            LoadConfigs();
            ShowConfigs();
        }

#elif MELONLOADER
using MelonLoader;

[assembly: MelonInfo(typeof(ModMenu.Plugin), "ModMenu", PluginInfo.PLUGIN_VERSION, "Kaden5480")]
[assembly: MelonGame("TraipseWare", "Peaks of Yore")]

namespace ModMenu {
    public class Plugin : MelonMod {

#endif

    }
}
