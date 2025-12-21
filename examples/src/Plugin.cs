using System.Linq;
using System.Reflection;

using BepInEx;
using HarmonyLib;
using ModMenu;
using UILib;
using UILib.Components;
using UILib.Notifications;
using UIButton = UILib.Components.Button;
using UnityEngine;

namespace ModMenuExamples {
    [BepInPlugin("com.github.Kaden5480.poy-mod-menu-examples", "Mod Menu Examples", "0.1.0")]
    internal class Plugin : BaseUnityPlugin {
        private Config config;
        private Window window;

        /**
         * <summary>
         * Executes when the plugin is being loaded.
         * </summary>
         */
        private void Awake() {
            config = new Config(this.Config);
            UIRoot.onInit.AddListener(BuildUI);

            if (AccessTools.AllAssemblies().FirstOrDefault(
                    a => a.GetName().Name == "ModMenu"
                ) != null
            ) {
                Logger.LogInfo("Registering");
                Register();
            }
            else {
                Logger.LogInfo("Not registering");
            }

            // Or this, but it logs a warning
            // if (AccessTools.TypeByName("ModMenu.ModManager") != null) {
            //     // Register
            // }
        }

        /**
         * <summary>
         * Sets up Mod Menu integration.
         * </summary>
         */
        private void Register() {
            Font arial = UnityEngine.Resources.GetBuiltinResource<Font>("Arial.ttf");

            Theme customTheme = Theme.GetTheme();
            customTheme.font               = arial;
            customTheme.fontAlt            = arial;
            customTheme.fontLineSpacing    = 1f;
            customTheme.fontScaler         = 0.98f;
            customTheme.fontScalerAlt      = 1f;
            customTheme.background         = Colors.HSL(240, 10, 10);
            customTheme.foreground         = Colors.HSL(240, 100, 90);
            customTheme.accent             = Colors.HSL(240, 10, 15);
            customTheme.accentAlt          = Colors.HSL(240, 10, 20);
            customTheme.selectNormal       = Colors.HSL(240, 20, 40);
            customTheme.selectHighlight    = Colors.HSL(240, 20, 50);
            customTheme.selectAltNormal    = Colors.HSL(240, 30, 60);
            customTheme.selectAltHighlight = Colors.HSL(240, 30, 70);

            ModInfo info = ModManager.Register(this);
            info.theme = customTheme;
            info.thumbnailUrl = "https://avatars.githubusercontent.com/u/67208843";
            info.license = "GPLv3.0";
            info.description = "A mod for testing Mod Menu."
                + " This provides a variety of examples demonstrating how"
                + " you can interact with Mod Menu."
                + " It is not a complete reference, so it's worth checking"
                + " out the official API reference as well.";

            info.Add(config);
            info.Add(typeof(StaticConfig));
            info.onBuild.AddListener((ModView view) => {
                UIButton button = new UIButton("Open Window", 20);
                button.SetSize(200f, 30f);
                button.onClick.AddListener(() => {
                    window.ToggleVisibility();
                });

                view.Add("Custom Injected Category", button);
            });
        }

        /**
         * <summary>
         * Builds a custom UI.
         * </summary>
         */
        private void BuildUI() {
            window = new Window("Custom Made Window", 800f, 600f);

            UIButton button = new UIButton("Click", 20);
            button.onClick.AddListener(() => {
                Notifier.Notify("Button", "You clicked the button");
            });
            button.SetSize(200f, 30f);

            window.Add(button);
        }
    }
}
