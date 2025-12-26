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

            // Build the UI when UILib is ready
            UIRoot.onInit.AddListener(() => {
                BuildUI();

                // Also assign a shortcut
                Shortcut shortcut = new Shortcut(new[] { config.coolShortcut });
                shortcut.onTrigger.AddListener(() => {
                    Notifier.Notify("Mod Menu Example", "You pressed the shortcut!");
                });
                UIRoot.AddShortcut(shortcut);

                // Note:
                // This shortcut was made using a ConfigEntry. Any changes to
                // the `Value` on the ConfigEntry will automatically update
                // within the shortcut.
                //
                // So there is no reason to have a listener on the `coolShortcut` field
                // to re-update the shortcut every time Mod Menu changes it.
                //
                // This helps simplify larger configs with many keybinds.
            });

            // This is an example of how you could make Mod Menu
            // an optional dependency at runtime
            // Mod Menu must be loaded for the "Register" method to run
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

            // Or you could do this, but it logs a warning when
            // Mod Menu isn't installed
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

            // Apply a heavily customised theme
            Theme customTheme = new Theme("Mod Menu Example Theme");
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

            // Register this mod with mod menu
            ModInfo info = ModManager.Register(this);
            info.theme = customTheme;
            info.thumbnailUrl = "https://avatars.githubusercontent.com/u/67208843";
            info.license = "GPL-3.0";
            info.description = "A mod for testing Mod Menu."
                + " This provides a variety of examples demonstrating how"
                + " you can interact with Mod Menu."
                + " It is not a complete reference, so it's worth checking"
                + " out the official API reference as well.";

            // Add some configs to display on the mod's config view
            info.Add(config);
            info.Add(typeof(StaticConfig));

            // You can also build in some extra custom components
            info.onBuild.AddListener((ModView view) => {
                UIButton button = new UIButton("Open Window", 20);
                button.SetSize(200f, 30f);
                button.onClick.AddListener(() => {
                    window.ToggleVisibility();
                });

                // This places the button under the "Custom Injected Category" section
                // You should see the other versions of `Add` though, as they provide
                // many ways of adding components, including applying metadata for
                // searching and tooltips.
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
