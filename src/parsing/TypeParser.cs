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
        private static T GetAttr<T>(MemberInfo info) where T : Attribute {
            return (T) Attribute.GetCustomAttribute(info, typeof(T));
        }

        /**
         * <summary>
         * Gets attributes of type T from a given field.
         * </summary>
         * <param name="info">The field info to get attributes for</param>
         */
        private static T[] GetAttrs<T>(MemberInfo info) where T : Attribute {
            return (T[]) Attribute.GetCustomAttributes(info, typeof(T));
        }

        /**
         * <summary>
         * Parses extra metadata from attributes on a given field.
         * </summary>
         * <param name="field">The field to add metadata to</param>
         * <param name="info">The field info to parse attributes for</param>
         */
        private void ParseMetadata(BaseField field, MemberInfo info) {
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

            // Extract predicates
            foreach (PredicateAttribute attr in GetAttrs<PredicateAttribute>(info)) {
                MethodInfo method = AccessTools.Method(
                    attr.type, attr.name, new Type[] { field.type }, attr.generics
                );

                if (method == null) {
                    string predicateName = $"{attr.type}.{attr.name}({field.type})";
                    Plugin.LogError($"{field.name}: Unable to find predicate `{predicateName}`");
                    continue;
                }

                field.AddPredicate(method);
            }

            // Extract listeners
            foreach (ListenerAttribute attr in GetAttrs<ListenerAttribute>(info)) {
                MethodInfo method = AccessTools.Method(
                    attr.type, attr.name, new Type[] { field.type }, attr.generics
                );

                if (method == null) {
                    string listenerName = $"{attr.type}.{attr.name}.({field.type})";
                    Plugin.LogError($"{field.name}: Unable to find listener `{listenerName}`");
                }

                field.AddListener(method);
            }
        }

        /**
         * <summary>
         * Parses a field.
         * </summary>
         * <param name="category">The category this field is in</param>
         * <param name="field">The field to parse</param>
         * <param name="info">The member info</param>
         */
        private void ParseField(string category, BaseField field, MemberInfo info) {
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
         * Parses a member, generating a BaseField.
         * </summary>
         * <param name="category">The category the member is in</param>
         * <param name="member">The member to parse</param>
         * <param name="instance">The parent instance</param>
         * <returns>The generated field, if valid</returns>
         */
        private void ParseMember<T>(string category, MemberInfo info, object instance) {
            BaseField field;

            // Properties
            if (typeof(T) == typeof(PropertyInfo)) {
                PropertyInfo propInfo = (PropertyInfo) info;

                if (propInfo.PropertyType.IsSubclassOf(typeof(ConfigEntryBase)) == true) {
                    ConfigEntryBase entry = (ConfigEntryBase) propInfo.GetValue(instance);
                    category = entry.Definition.Section;
                    field = new BepInField(entry);
                }
                else {
                    field = new PlainProperty(propInfo, instance);
                }
            }
            // Fields
            else if (typeof(T) == typeof(FieldInfo)) {
                FieldInfo fieldInfo = (FieldInfo) info;

                if (fieldInfo.FieldType.IsSubclassOf(typeof(ConfigEntryBase)) == true) {
                    ConfigEntryBase entry = (ConfigEntryBase) fieldInfo.GetValue(instance);
                    category = entry.Definition.Section;
                    field = new BepInField(entry);
                }
                else {
                    field = new PlainField(fieldInfo, instance);
                }
            }
            else {
                return;
            }

            // Parse attributes
            ParseMetadata(field, info);

            // Parse the field into the categories
            ParseField(category, field, info);
        }

        /**
         * <summary>
         * Checks if a given MemberInfo is static.
         * </summary>
         * <param name="member">The member info to check</param>
         * <returns>Whether the info is static</returns>
         */
        private bool IsStatic<T>(MemberInfo info) {
            if (typeof(T) == typeof(FieldInfo)) {
                return ((FieldInfo) info).IsStatic;
            }

            if (typeof(T) == typeof(PropertyInfo)) {
                PropertyInfo propInfo = (PropertyInfo) info;
                return propInfo.GetMethod != null
                    && propInfo.GetMethod.IsStatic;
            }

            return false;
        }

        /**
         * <summary>
         * Parses members of a type.
         * </summary>
         * <param name="type">The type being parsed</param>
         * <param name="obj">An instance of the type, if any</param>
         * <param name="members">The members to parse</param>
         */
        private void ParseMembers<T>(Type type, object obj, IList<T> members) where T : MemberInfo {
            // If a CategoryAttribute doesn't exist, just default to the name of the type
            string category = type.Name;

            CategoryAttribute categoryAttr = GetAttr<CategoryAttribute>(type);
            if (categoryAttr != null) {
                category = categoryAttr.name;
            }

            IncludeAllAttribute includeAll = GetAttr<IncludeAllAttribute>(type);

            foreach (MemberInfo info in members) {
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

                ParseMember<T>(
                    category, info,
                    (IsStatic<T>(info) == true) ? null : obj
                );
            }
        }

        /**
         * <summary>
         * Generates config info for a given type and instance.
         * </summary>
         * <param name="type">The type to generate config info for</param>
         * <param name="obj">The instance to generate config info for</param>
         */
        private void Parse(Type type, object obj) {
            // Parse fields and properties
            ParseMembers<FieldInfo>(type, obj, AccessTools.GetDeclaredFields(type));
            ParseMembers<PropertyInfo>(type, obj, AccessTools.GetDeclaredProperties(type));
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
