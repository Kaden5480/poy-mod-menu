using System.Collections.Generic;

using BepInEx;

namespace ModMenu {
    /**
     * <summary>
     * The class which generates <see cref="ModInfo"/> objects
     * and allows mods to start registering their configs.
     * </summary>
     */
    public static class ModManager {
        private static Logger logger = new Logger(typeof(ModManager));

        // Registered mods
        internal static Dictionary<BaseUnityPlugin, ModInfo> mods
            = new Dictionary<BaseUnityPlugin, ModInfo>();

        /**
         * <summary>
         * Registers your mod with Mod Menu.
         * </summary>
         * <param name="mod">The mod to register</param>
         * <returns>A `ModInfo` for your mod</returns>
         */
        public static ModInfo Register(BaseUnityPlugin mod) {
            if (mods.ContainsKey(mod) == true) {
                logger.LogError($"{mod.GetType()} has already been registered");
                return null;
            }

            ModInfo info = new ModInfo(mod);
            mods[mod] = info;

            return info;
        }
    }
}
