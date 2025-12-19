using System;
using System.Collections.Generic;
using System.Reflection;

using BepInEx.Configuration;
using HarmonyLib;

using ModMenu.Config;

namespace ModMenu.Parsing {
    internal class TypeParser {
        // Things to parse
        private List<Type> types;
        private List<object> objects;
        private List<ConfigFile> configFiles;

        // Generated config info
        private Dictionary<string, List<BaseField>> config;

        /**
         * <summary>
         * Initializes a type parser.
         * </summary>
         * <param name="types">The types to parse</param>
         * <param name="objects">The objects to parse</param>
         */
        internal TypeParser(
            List<Type> types,
            List<object> objects,
            List<ConfigFile> configFiles
        ) {
            this.types = types;
            this.objects = objects;
            this.configFiles = configFiles;

            config = new Dictionary<string, List<BaseField>>();
        }

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
         * Parses extra metadata from attributes on a given field.
         * </summary>
         * <param name="field">The field to add metadata to</param>
         * <param name="info">The field info to parse attributes for</param>
         */
        private void ParseMetadata(BaseField field, FieldInfo info) {
            // FieldAttributes can override the default data in
            // fields, so iterate over them and see what gets changed
            foreach (FieldAttribute attr in GetAttrs<FieldAttribute>(info)) {
                if (attr.name != null) {
                    field.name = attr.name;
                }

                if (attr.description != null) {
                    field.description = attr.description;
                }

                if (attr.fieldType != FieldType.None) {
                    field.fieldType = attr.fieldType;
                }

                if (attr.min != null) {
                    field.min = attr.min;
                }

                if (attr.max != null) {
                    field.max = attr.max;
                }

                if (attr.defaultValue != null) {
                    field.defaultValue = attr.defaultValue;
                }
            }

            // If the field type hasn't been defined
            // by an attribute, try guessing the best one
            if (field.fieldType == FieldType.None) {
                field.GuessFieldType();
            }
        }

        /**
         * <summary>
         * Parses a field.
         * </summary>
         * <param name="category">The category this field is in</param>
         * <param name="info">This field's info</param>
         * <param name="instance">The parent instance</param>
         */
        private void ParseField(string category, FieldInfo info, object instance) {
            BaseField field;

            // Determine the kind of field wrapper to use
            if (info.FieldType.IsSubclassOf(typeof(ConfigEntryBase)) == true) {
                ConfigEntryBase entry = (ConfigEntryBase) info.GetValue(instance);
                category = entry.Definition.Section;

                field = new BepInField(entry);
            }
            else {
                field = new PlainField(info, instance);
            }

            // Parse attributes
            ParseMetadata(field, info);

            // Check if the field is valid
            if (field.Validate() == false) {
                return;
            }

            // Override category if defined
            CategoryAttribute categoryAttr = GetAttr<CategoryAttribute>(info);
            if (categoryAttr != null) {
                category = categoryAttr.name;
            }

            // Add to the config
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
        private void Parse(Type type, object obj) {
            // If a CategoryAttribute doesn't exist, just default to the name of the type
            string category = type.Name;

            CategoryAttribute categoryAttr = GetAttr<CategoryAttribute>(type);
            if (categoryAttr != null) {
                category = categoryAttr.name;
            }

            IncludeAllAttribute includeAll = GetAttr<IncludeAllAttribute>(type);

            // Iterate over all fields declared under this type and parse them
            foreach (FieldInfo info in AccessTools.GetDeclaredFields(type)) {
                // Check if this even has a field attribute
                if (includeAll == null
                    && GetAttr<FieldAttribute>(info) == null
                ) {
                    continue;
                }

                // Check if this field has an exclude attribute
                if (GetAttr<ExcludeAttribute>(info) != null) {
                    continue;
                }

                ParseField(
                    category, info,
                    (info.IsStatic == true) ? null : obj
                );
            }
        }

        /**
         * <summary>
         * Parses a `ConfigFile`.
         * </summary>
         * <param name="configFile">The config file to parse</param>
         */
        private void ParseConfigFile(ConfigFile configFile) {
            Dictionary<ConfigDefinition, ConfigEntryBase> entries;

            MethodInfo entriesInfo = AccessTools.PropertyGetter(
                typeof(ConfigFile), "Entries"
            );

            entries = (Dictionary<ConfigDefinition, ConfigEntryBase>)
                entriesInfo.Invoke(configFile, new object[] {});

            foreach (KeyValuePair<ConfigDefinition, ConfigEntryBase> entry in entries) {
                BepInField field = new BepInField(entry.Value);
                string category = entry.Value.Definition.Section;

                if (field.GuessFieldType(true) == false) {
                    continue;
                }

                if (field.Validate() == false) {
                    continue;
                }

                // Add to the config
                if (config.ContainsKey(category) == false) {
                    config[category] = new List<BaseField>();
                }

                config[category].Add(field);
            }
        }

        /**
         * <summary>
         * Parses provided types and objects to generate a config.
         * </summary>
         * <returns>The generated config</returns>
         */
        internal Dictionary<string, List<BaseField>> Parse() {
            foreach (Type type in types) {
                Parse(type, null);
            }

            foreach (object obj in objects) {
                Parse(obj.GetType(), obj);
            }

            foreach (ConfigFile configFile in configFiles) {
                ParseConfigFile(configFile);
            }

            // Return generated config
            return config;
        }
    }
}
