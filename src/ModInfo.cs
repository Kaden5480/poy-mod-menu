using System;
using System.Collections.Generic;
using System.Reflection;

using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

using ModMenu.Config;

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
        private List<object> configObjects = new List<object>();
        private List<Type> configTypes = new List<Type>();

        // Generated config info
        internal Dictionary<string, List<BaseField>> config
            = new Dictionary<string, List<BaseField>>();

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
         * A brief description of this mod.
         * </summary>
         */
        public string description = null;

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

#region Config Info Generation

        /**
         * <summary>
         * Gets an attribute of type T from a given type.
         * </summary>
         * <param name="type">The type to get the attribute from</param>
         */
        private static T GetAttr<T>(Type type) where T : Attribute {
            return (T) Attribute.GetCustomAttribute(type, typeof(T));
        }

        /**
         * <summary>
         * Gets an attribute of type T from a given field.
         * </summary>
         * <param name="info">The field info to get attributes for</param>
         */
        private static T GetAttr<T>(FieldInfo info) where T : Attribute {
            return (T) Attribute.GetCustomAttribute(info, typeof(T));
        }

        /**
         * <summary>
         * Gets attributes of type T from a given field.
         * </summary>
         * <param name="info">The field info to get attributes for</param>
         */
        private static T[] GetAttrs<T>(FieldInfo info) where T : Attribute {
            return (T[]) Attribute.GetCustomAttributes(info, typeof(T));
        }

        /**
         * <summary>
         * Generates config info for a given field.
         * </summary>
         * <param name="category">The category this field is in</param>
         * <param name="info">This field's info</param>
         * <param name="instance">The parent instance</param>
         */
        private void GenerateField(string category, FieldInfo info, object instance) {
            BaseField field;
            Type type = info.FieldType;

            // Override category
            CategoryAttribute categoryAttr = GetAttr<CategoryAttribute>(info);
            if (categoryAttr != null) {
                category = categoryAttr.name;
            }

            // Determine the kind of field wrapper to use
            if (type.IsSubclassOf(typeof(ConfigEntryBase)) == true) {
                field = new BepInField((ConfigEntryBase) info.GetValue(instance));
            }
            else {
                field = new PlainField(info, instance);
            }

            // Determine extra metadata
            foreach (FieldAttribute fieldAttr in GetAttrs<FieldAttribute>(info)) {
                if (fieldAttr.name != null) {
                    field.name = fieldAttr.name;
                }

                if (fieldAttr.description != null) {
                    field.description = fieldAttr.description;
                }

                if (fieldAttr.fieldType != FieldType.None) {
                    field.fieldType = fieldAttr.fieldType;
                }
                else {
                    field.GuessFieldType();
                }
            }

            if (config.ContainsKey(category) == false) {
                config[category] = new List<BaseField>();
            }

            config[category].Add(field);
        }

        /**
         * <summary>
         * Generates config info for a given type and instance.
         * </summary>
         * <param name="type">The type to generate config info for</param>
         * <param name="obj">The instance to generate config info for</param>
         */
        private void Generate(Type type, object obj) {
            string category = "General";

            CategoryAttribute categoryAttr = GetAttr<CategoryAttribute>(type);
            if (categoryAttr != null) {
                category = categoryAttr.name;
            }

            foreach (FieldInfo info in AccessTools.GetDeclaredFields(type)) {
                GenerateField(
                    category,
                    info,
                    (info.IsStatic == true) ? null : obj
                );
            }
        }

        /**
         * <summary>
         * Generates config info from the registered configs.
         * </summary>
         */
        internal void Generate() {
            foreach (Type type in configTypes) {
                Generate(type, null);
            }

            foreach (object obj in configObjects) {
                Generate(obj.GetType(), obj);
            }
        }

#endregion

    }
}
