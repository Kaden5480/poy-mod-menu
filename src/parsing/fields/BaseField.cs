using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using ModMenu.Config;

namespace ModMenu.Parsing {
    /**
     * <summary>
     * A class which wraps around types which
     * have been annotated with <see cref="FieldAttribute"/>.
     * </summary>
     */
    internal abstract class BaseField {
        // Information about the parent class this
        // is stored in
        protected object parentInstance = null;

        // The underlying type this field holds
        internal Type type { get; private set; }

        // Which type of UI component should
        // be displayed for this field
        internal FieldType fieldType = FieldType.None;

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
            { typeof(Enum),    FieldType.Dropdown },
            { typeof(bool),    FieldType.Toggle   },
            { typeof(string),  FieldType.Text     },
        };

        /**
         * <summary>
         * Initializes a base field.
         * </summary>
         * <param name="type">The underlying type this field wraps around</param>
         */
        internal BaseField(Type type) {
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
         * Sets the value of this field, also
         * invoking `onValueChanged`.
         * </summary>
         * <param name="value">The value to set this field to</param>
         */
        internal void SetValue(object value) {
            this.value = value;
            Update();
        }

        /**
         * <summary>
         * Restores the default value of this field,
         * also invoking `onValueChanged`.
         * </summary>
         */
        internal void Restore() {
            SetValue(defaultValue);
        }

        /**
         * <summary>
         * Guesses the most likely field type for
         * the underlying type.
         * </summary>
         */
        internal void GuessFieldType() {
            if (fieldType != FieldType.None) {
                return;
            }

            if (TypeChecks.IsNumeric(type) == true) {
                fieldType = FieldType.Text;
            }
            else if (defaultTypes.TryGetValue(type, out FieldType newType) == true) {
                fieldType = newType;
            }

            if (fieldType == FieldType.None) {
                Plugin.LogError(
                    $"{type}({name}): Unable to guess best FieldType."
                    + " You may want to explicitly define/exclude this field,"
                    + " you may even be trying to use a type that ModMenu can't use."
                );
            }
        }

        /**
         * <summary>
         * Validates whether the current configuration of this field is compatible.
         * </summary>
         * <returns>Whether this configuration is valid</returns>
         */
        internal bool Validate() {
            bool valid = true;

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
                Plugin.LogError($"{name}: A `{type}` field can't be a `{compare}`");
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
