using System;
using System.Collections.Generic;
using System.Reflection;

using BepInEx;
using BepInEx.Configuration;
using UILib;
using UnityEngine;

using ModMenu.Config;
using ModMenu.Events;
using ModMenu.Parsing;

namespace ModMenu {
    /**
     * <summary>
     * A class containing information about a mod,
     * along with its registered configs.
     *
     * `ModInfo` instances are returned by <see cref="ModManager.Register"/>.
     *
     * When you `Add` a config to a `ModInfo` instance, it will be parsed
     * and displayed within your mod's config page once
     * <see cref="ModView"/> has built it.
     * </summary>
     */
    public class ModInfo {
        internal Logger logger { get; private set; } = new Logger(typeof(ModInfo));

        // Underlying mod and its metadata
        private BaseUnityPlugin mod = null;
        private BepInPlugin metadata { get => mod.Info.Metadata; }

        // Registered configs
        private List<Type> configTypes = new List<Type>();
        private List<object> configObjects = new List<object>();
        private List<ConfigFile> configFiles = new List<ConfigFile>();

        // Generated config
        internal Dictionary<string, List<BaseField>> config { get; private set; }

        // Generated UI
        internal ModView view { get; private set; }

        // Whether this mod info was auto-generated
        internal bool generated = false;

        /**
         * <summary>
         * Invokes listeners with a <see cref="ModView"/>
         * when this mod's config page is being built.
         *
         * This allows you to apply some extra customisations
         * before the page is fully created.
         * </summary>
         */
        public BuildEvent onBuild { get; } = new BuildEvent();

        /**
         * <summary>
         * The name of this mod. (read only)
         *
         * This is the name set in the `BepInPlugin` attribute of your mod.
         * </summary>
         */
        public string name { get => metadata.Name; }

        /**
         * <summary>
         * This mod's version. (read only)
         *
         * This is the version set in the `BepInPlugin` attribute of your mod.
         * </summary>
         */
        public Version version { get => metadata.Version; }

        /**
         * <summary>
         * The `Theme` to apply to this mod's config page.
         * </summary>
         */
        public Theme theme = null;

        /**
         * <summary>
         * A URL to download the mod's thumbnail from.
         *
         * If <see cref="thumbnail"/> is set, it will be used
         * instead of this.
         * </summary>
         */
        public string thumbnailUrl = null;

        /**
         * <summary>
         * A texture to apply for the mod's thumbnail.
         *
         * If <see cref="thumbnailUrl"/> is set, the texture
         * set here will still be used anyway. The URL will be ignored.
         * </summary>
         */
        public Texture2D thumbnail = null;

        /**
         * <summary>
         * A link to the source code for this mod.
         * </summary>
         */
        public string sourceCode = null;

        /**
         * <summary>
         * This mod's license.
         * </summary>
         */
        public string license = null;

        /**
         * <summary>
         * A description for this mod.
         *
         * By default this will be the description set in the
         * <see cref="System.Reflection.AssemblyDescriptionAttribute"/>
         * of the <see cref="System.Reflection.Assembly"/> of your mod.
         * </summary>
         */
        public string description = null;

        /**
         * <summary>
         * Initializes information about a mod.
         * </summary>
         * <param name="mod">The mod to generate info for</param>
         */
        internal ModInfo(BaseUnityPlugin mod) {
            this.mod = mod;

            // Try getting description
            AssemblyDescriptionAttribute attr = mod.GetType().Assembly
                .GetCustomAttribute<AssemblyDescriptionAttribute>();

            if (attr != null) {
                description = attr.Description;
            }
        }

        /**
         * <summary>
         * Adds a given `Type` as a config.
         *
         * This should be used for static configs.
         * </summary>
         * <param name="type">The type to add</param>
         */
        public void Add(Type type) {
            if (configTypes.Contains(type) == true) {
                logger.LogError($"{name} already registered config: {type}");
                return;
            }

            configTypes.Add(type);
        }

        /**
         * <summary>
         * Adds a given `object` as a config.
         *
         * This should be used for configs which have an instance (non static).
         * </summary>
         * <param name="obj">The object to add</param>
         */
        public void Add(object obj) {
            if (configObjects.Contains(obj) == true) {
                logger.LogError($"{name} already registered config: {obj}");
                return;
            }

            configObjects.Add(obj);
        }

        /**
         * <summary>
         * Adds a `ConfigFile` as a config.
         *
         * This allows you to add an entire BepInEx `ConfigFile` at once.
         * Note: you miss out on customisation, but this could help for simplicity.
         * </summary>
         * <param name="configFile">The ConfigFile to add</param>
         */
        public void Add(ConfigFile configFile) {
            if (configFiles.Contains(configFile) == true) {
                logger.LogError($"{name} already registered config file: {configFile}");
                return;
            }

            configFiles.Add(configFile);
        }

        /**
         * <summary>
         * Generate a config from types and instances.
         * </summary>
         */
        private void Generate() {
            if (config != null) {
                logger.LogError("Config has already been generated");
                return;
            }

            config = new TypeParser(
                this, configTypes, configObjects, configFiles
            ).Parse();
        }

        /**
         * <summary>
         * Build the UI for this mod.
         * </summary>
         * <param name="ui">The UI to build onto</param>
         */
        internal void Build(UI ui) {
            if (config == null) {
                Generate();
            }

            if (view != null) {
                return;
            }

            view = new ModView(this);

            // Allow any extra customisations
            onBuild.Invoke(view);

            // Fully build the UI
            view.BuildAll();
        }
    }
}
