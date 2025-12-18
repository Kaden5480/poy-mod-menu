using BepInEx;
using ModMenu;
using UILib;
using UILib.Components;
using UILib.Notifications;
using UIButton = UILib.Components.Button;
using UnityEngine;

namespace ModMenuExamples {
    [BepInPlugin("com.github.Kaden5480.poy-mod-menu-examples", "Mod Menu Examples", "0.1.0")]
    internal class Plugin : BaseUnityPlugin {
        private Window window;

        /**
         * <summary>
         * Executes when the plugin is being loaded.
         * </summary>
         */
        private void Awake() {
            Config config = new Config(this.Config);

            UIRoot.onInit.AddListener(BuildUI);

            Font arial = UnityEngine.Resources.GetBuiltinResource<Font>("Arial.ttf");

            Theme customTheme = new Theme() {
                //font               = arial,
                //fontAlt            = arial,
                //fontLineSpacing    = 1f,
                background         = Colors.HSL(120, 10, 10),
                foreground         = Colors.HSL(120, 100, 90),
                accent             = Colors.HSL(120, 10, 15),
                accentAlt          = Colors.HSL(120, 10, 20),
                selectNormal       = Colors.HSL(120, 20, 20),
                selectHighlight    = Colors.HSL(120, 20, 30),
                selectAltNormal    = Colors.HSL(120, 30, 20),
                selectAltHighlight = Colors.HSL(120, 30, 30),
            };

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
            info.onBuild.AddListener((ConfigBuilder builder) => {
                UIButton button = new UIButton("Open Window", 20);
                button.SetSize(200f, 30f);
                button.onClick.AddListener(() => {
                    window.ToggleVisibility();
                });

                builder.Add("Custom Injected Category", button);
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
