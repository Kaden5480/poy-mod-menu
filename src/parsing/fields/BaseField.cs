using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.Events;

using ModMenu.Config;
using ModMenu.Events;

namespace ModMenu.Parsing {
    /**
     * <summary>
     * A class which wraps around types which
     * have been annotated with <see cref="FieldAttribute"/>.
     * </summary>
     */
    internal abstract class BaseField {
        // The mod this field is for
        internal ModInfo modInfo { get; private set; }

        // Information about the parent class this
        // is stored in
        protected object parentInstance = null;

        // The underlying type this field holds
        internal Type type { get; private set; }

        // Which type of UI component should
        // be displayed for this field
        internal FieldType fieldType = FieldType.None;

        // The predicates to run
        internal List<MethodInfo> predicates = new List<MethodInfo>();

        // Listeners to invoke
        internal List<MethodInfo> listeners = new List<MethodInfo>();

        internal string name         = null;
        internal string description  = null;
        internal object defaultValue = null;
        internal object min          = null;
        internal object max          = null;

        internal abstract object value { get; set; }

        internal ValueEvent<object> onValueChanged { get; }
            = new ValueEvent<object>();

        // Types which are correct
        private static Dictionary<Type, FieldType> defaultTypes
            = new Dictionary<Type, FieldType>() {
            { typeof(Color),   FieldType.Color    },
            { typeof(KeyCode), FieldType.KeyCode  },
            { typeof(bool),    FieldType.Toggle   },
            { typeof(string),  FieldType.Text     },
        };

        /**
         * <summary>
         * Initializes a base field.
         * </summary>
         * <param name="modInfo">The mod this field is for</param>
         * <param name="type">The underlying type this field wraps around</param>
         */
        internal BaseField(ModInfo modInfo, Type type) {
            this.modInfo = modInfo;
            this.type = type;
        }

        /**
         * <summary>
         * Invokes `onValueChanged` with the current value.
         * </summary>
         */
        internal void Update() {
            onValueChanged.Invoke(value);
        }

        /**
         * <summary>
         * Sets the value of this field.
         * </summary>
         * <param name="value">The value to set this field to</param>
         */
        internal void SetValue(object value) {
            this.value = value;

            foreach (MethodInfo listener in listeners) {
                listener.Invoke(null, new object[] { value });
            }
        }

        /**
         * <summary>
         * Restores the default value of this field,
         * also invoking `onValueChanged`.
         * </summary>
         */
        internal void Restore() {
            SetValue(defaultValue);
            Update();
        }

        /**
         * <summary>
         * Adds a predicate to this field.
         * </summary>
         * <param name="predicate">The predicate to add</param>
         */
        internal void AddPredicate(MethodInfo predicate) {
            if (predicates.Contains(predicate) == true) {
                Plugin.LogError($"{name}: The predicate `{predicate}` has already been specified");
                return;
            }

            if (predicate.ReturnType != typeof(string)) {
                Plugin.LogError($"{name}: The predicate `{predicate}` must return a string");
                return;
            }

            if (predicate.IsStatic == false) {
                Plugin.LogError($"{name}: Can't use non-static predicate `{predicate}`");
                return;
            }

            predicates.Add(predicate);
        }

        /**
         * <summary>
         * Checks a value against all predicates.
         * </summary>
         * <param name="value">The value to check</param>
         * <returns>An error message, or null</returns>
         */
        internal string CheckPredicates(object value) {
            foreach (MethodInfo predicate in predicates) {
                string error = (string) predicate.Invoke(null, new[] { value });

                if (error != null) {
                    return error;
                }
            }

            return null;
        }

        /**
         * <summary>
         * Adds a listener to this field.
         * </summary>
         * <param name="listener">The listener to add</param>
         */
        internal void AddListener(MethodInfo listener) {
            if (listeners.Contains(listener) == true) {
                Plugin.LogError($"{name}: The listener `{listener}` has already been specified");
                return;
            }

            if (listener.IsStatic == false) {
                Plugin.LogError($"{name}: Can't use non-static listener `{listener}`");
                return;
            }

            listeners.Add(listener);
        }

        /**
         * <summary>
         * Guesses the most likely field type for
         * the underlying type.
         * </summary>
         * <param name="quiet">Whether to prevent logging</param>
         * <returns>Whether guessing the type was successful</returns>
         */
        internal virtual bool GuessFieldType(bool quiet = false) {
            if (fieldType != FieldType.None) {
                return true;
            }

            if (TypeChecks.IsNumeric(type) == true) {
                fieldType = FieldType.Text;
            }
            else if (defaultTypes.TryGetValue(type, out FieldType newType) == true) {
                fieldType = newType;
            }

            if (fieldType == FieldType.None) {
                if (quiet == false) {
                    Plugin.LogError(
                        $"{type}({name}): Unable to guess best FieldType."
                        + " You may want to explicitly define/exclude this field,"
                        + " you may even be trying to use a type that Mod Menu can't use."
                    );
                }
                return false;
            }

            return true;
        }

        /**
         * <summary>
         * Validates whether the current configuration of this field is compatible.
         * </summary>
         * <returns>Whether this configuration is valid</returns>
         */
        internal virtual bool Validate() {
            bool valid = true;

            // Can't have a `None` type
            if (fieldType == FieldType.None) {
                Plugin.LogError($"{name}: Can't use a `None` field type");
                valid = false;
            }

            // Sliders must be numeric
            if (fieldType == FieldType.Slider && TypeChecks.IsNumeric(type) == false) {
                Plugin.LogError($"{name}: A `Slider` field must be numeric");
                valid = false;
            }

            // Sliders require both limits
            if (fieldType == FieldType.Slider && (min == null || max == null)) {
                Plugin.LogError($"{name}: A `Slider` field requires both `min` and `max` to be defined");
                valid = false;
            }

            // Min/max are only valid on some fields
            if (min != null || max != null) {
                switch (fieldType) {
                    case FieldType.Slider:
                    case FieldType.Text:
                        break;
                    default:
                        Plugin.LogError($"{name}: min/max can't be applied to `{fieldType}`");
                        valid = false;
                        break;
                }
            }

            // Min or max requires numerics
            if ((min != null || max != null) && TypeChecks.IsNumeric(type) == false) {
                Plugin.LogError($"{name}: Fields with `min`/`max` must be numeric");
                valid = false;
            }

            // Min and max must be same as type
            if (min != null && (min.GetType() != type)) {
                Plugin.LogError($"{name}: `min` must be the same type as the field");
                valid = false;
            }
            if (max != null && (max.GetType() != type)) {
                Plugin.LogError($"{name}: `max` must be the same type as the field");
                valid = false;
            }

            // Check for weird choices
            if (defaultTypes.TryGetValue(type, out FieldType compare) == true
                && fieldType != compare
            ) {
                Plugin.LogError($"{name}: A `{type}` field can't be a `{fieldType}`");
                valid = false;
            }

            return valid;
        }

        /**
         * <summary>
         * Converts this field's value to a string.
         * </summary>
         * <returns>This field's current value as a string</returns>
         */
        public override string ToString() {
            if (value == null) {
                return "";
            }

            if (TypeChecks.IsInteger(type) == true) {
                return value.ToString();
            }

            if (type == typeof(float)) {
                return ((float) value).ToString("0.00");
            }

            if (type == typeof(double)) {
                return ((double) value).ToString("0.00");
            }

            return value.ToString();
        }
    }
}
