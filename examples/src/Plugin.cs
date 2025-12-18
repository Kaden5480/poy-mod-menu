using BepInEx;
using ModMenu;
using UILib;
using UILib.Components;
using UILib.Notifications;
using UIButton = UILib.Components.Button;

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

            ModInfo info = ModManager.Register(this);
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
