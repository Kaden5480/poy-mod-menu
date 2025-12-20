using System.IO;
using System.Reflection;

using UnityEngine;

namespace ModMenu {
    /**
     * <summary>
     * A class holding resources used by Mod Menu.
     * These resources are loaded from an `AssetBundle`.
     * </summary>
     */
    internal static class Resources {
        private const string bundlePath = "modmenu.bundle";
        private static AssetBundle bundle;

        /**
         * <summary>
         * The default checkmark used by Peaks of Yore.
         * </summary>
         */
        public static Texture2D searchGlass { get; private set; } = LoadFromBundle<Texture2D>(
            "SearchGlass"
        );

        /**
         * <summary>
         * Loads a file with the specified filename
         * into a byte array.
         *
         * The files are loaded from res/, so passing
         * a name of "image.png" will load res/image.png.
         * </summary>
         * <param name="name">The name of the file to load</param>
         * <returns>The file's bytes</returns>
         */
        internal static byte[] LoadBytes(string name) {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string assemblyName = assembly.GetName().Name;

            using (Stream stream = assembly.GetManifestResourceStream(
                $"{assemblyName}.res.{name}"
            )) {
            using (MemoryStream mem = new MemoryStream()) {
                stream.CopyTo(mem);
                return mem.ToArray();
            }}
        }

        /**
         * <summary>
         * Loads an asset by name.
         * </summary>
         * <param name="name">The name of the asset to load</param>
         * <returns>The loaded asset</returns>
         */
        internal static T LoadFromBundle<T>(string name) where T : UnityEngine.Object {
            if (bundle == null) {
                byte[] bundleData = LoadBytes(bundlePath);
                bundle = AssetBundle.LoadFromMemory(bundleData);
            }

            return bundle.LoadAsset<T>(name);
        }
    }
}
