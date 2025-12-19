using System.Collections.Generic;
using System.Linq;

using BepInEx;

namespace ModMenu {
    /**
     * <summary>
     * The class which is used for registering mods to Mod Menu.
     * </summary>
     */
    public static class ModManager {
        private static Logger logger = new Logger(typeof(ModManager));

        // Registered mods
        internal static Dictionary<BaseUnityPlugin, ModInfo> mods
            = new Dictionary<BaseUnityPlugin, ModInfo>();

        /**
         * <summary>
         * Registers your mod with Mod Menu, returning a <see cref="ModInfo"/>
         * which you can use to customise your mod's config page.
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

        /**
         * <summary>
         * Sorts the mod list.
         * </summary>
         */
        internal static void Sort() {
            mods = mods.OrderBy(entry => entry.Value.name)
                .ToDictionary(entry => entry.Key, entry => entry.Value);
        }
    }
}
