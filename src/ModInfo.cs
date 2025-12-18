using System;
using System.Collections.Generic;

using BepInEx;
using UILib;
using UnityEngine;

using ModMenu.Config;
using ModMenu.Parsing;

namespace ModMenu {
    /**
     * <summary>
     * A class containing information about a mod, along
     * with its registered configs.
     * </summary>
     */
    public class ModInfo {
        private Logger logger = new Logger(typeof(ModInfo));

        // Underlying mod and its metadata
        private BaseUnityPlugin mod = null;
        private BepInPlugin metadata { get => mod.Info.Metadata; }

        // Registered configs
        private List<Type> configTypes = new List<Type>();
        private List<object> configObjects = new List<object>();

        // Generated config
        internal Dictionary<string, List<BaseField>> config;

        // Generated UI
        internal ConfigBuilder builder;

        /**
         * <summary>
         * Invokes listeners with a <see cref="ConfigBuilder"/>
         * when this mod's config UI is being built.
         * </summary>
         */
        public BuildEvent onBuild { get; } = new BuildEvent();

        /**
         * <summary>
         * The name of this mod.
         * </summary>
         */
        public string name { get => metadata.Name; }

        /**
         * <summary>
         * This mod's current version.
         * </summary>
         */
        public Version version { get => metadata.Version; }

        /**
         * <summary>
         * This mod's theme.
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
         * A brief description of this mod.
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
        }

        /**
         * <summary>
         * Adds a given type as a config to display in mod menu.
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
         * Adds a given object as a config to display in mod menu.
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
         * Generate a config from types and instances.
         * </summary>
         */
        internal void Generate() {
            if (config != null) {
                logger.LogError("Config has already been generated");
                return;
            }

            config = new TypeParser(
                configTypes, configObjects
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

            if (builder != null) {
                return;
            }

            builder = new ConfigBuilder(this);

            // Allow any extra customisations
            onBuild.Invoke(builder);

            // Fully build the UI
            builder.Build(ui);
        }
    }
}
