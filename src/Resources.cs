using System.Reflection;

using UILib;
using UnityEngine;

namespace ModMenu {
    /**
     * <summary>
     * A class holding resources used by Mod Menu.
     * These resources are loaded from an `AssetBundle`.
     * </summary>
     */
    internal static class Resources {
        private const string bundlePath = "res.modmenu.bundle";
        private static AssetBundle bundle;

        /**
         * <summary>
         * The default checkmark used by Peaks of Yore.
         * </summary>
         */
        public static Texture2D searchGlass { get; private set; } = LoadAsset<Texture2D>(
            "SearchGlass"
        );

        /**
         * <summary>
         * Mod Menu bundle loading.
         * </summary>
         * <param name="name">The name of the asset to load</param>
         * <returns>The loaded asset</returns>
         */
        private static T LoadAsset<T>(string name) where T : UnityEngine.Object {
            if (bundle == null) {
                bundle = UILib.Resources
                    .LoadBundle(Assembly.GetExecutingAssembly(), bundlePath);
            }

            return bundle.LoadAsset<T>(name);
        }
    }
}
