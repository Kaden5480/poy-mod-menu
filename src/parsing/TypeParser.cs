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

        // Generated config info
        private Dictionary<string, List<BaseField>> config;

        /**
         * <summary>
         * Initializes a type parser.
         * </summary>
         * <param name="types">The types to parse</param>
         * <param name="objects">The objects to parse</param>
         */
        internal TypeParser(List<Type> types, List<object> objects) {
            this.types = types;
            this.objects = objects;

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
            }

            // If the field type hasn't been defined yet,
            // try guessing the best one
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
                field = new BepInField((ConfigEntryBase) info.GetValue(instance));
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
            // If a CategoryAttribute doesn't exist, just default to "General"
            string category = "General";

            CategoryAttribute categoryAttr = GetAttr<CategoryAttribute>(type);
            if (categoryAttr != null) {
                category = categoryAttr.name;
            }

            // Iterate over all fields declared under this type and parse them
            foreach (FieldInfo info in AccessTools.GetDeclaredFields(type)) {
                ParseField(
                    category, info,
                    (info.IsStatic == true) ? null : obj
                );
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

            // Return generated config
            return config;
        }
    }
}
