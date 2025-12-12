using BepInEx;
using ModMenu;

namespace ModMenuExamples {
    [BepInPlugin("com.github.Kaden5480.poy-mod-menu-examples", "Mod Menu Examples", "0.1.0")]
    internal class Plugin : BaseUnityPlugin {

        /**
         * <summary>
         * Executes when the plugin is being loaded.
         * </summary>
         */
        private void Awake() {
            Config config = new Config(this.Config);

            ModInfo info = ModManager.Register(this);
            info.Add(config);
        }
    }
}
