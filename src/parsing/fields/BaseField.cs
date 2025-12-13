using System;
using System.Collections.Generic;

using UnityEngine;

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
         * Checks if the provided type is
         * numeric.
         * </summary>
         */
        private bool IsNumeric(Type type) {
            return type == typeof(byte)
                || type == typeof(sbyte)
                || type == typeof(char)
                || type == typeof(int)
                || type == typeof(long)
                || type == typeof(float)
                || type == typeof(double);
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

            if (IsNumeric(type) == true) {
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
            // Sliders must be numeric
            if (fieldType == FieldType.Slider && IsNumeric(type) == false) {
                Plugin.LogError($"{name}: A `Slider` field must be numeric");
                return false;
            }

            // Min or max requires numerics
            if ((min != null || max != null) && IsNumeric(type) == false) {
                Plugin.LogError($"{name}: Fields with `min`/`max` must be numeric");
                return false;
            }

            if (defaultTypes.TryGetValue(type, out FieldType compare) == true
                && fieldType != compare
            ) {
                Plugin.LogError($"{name}: A `{type}` field can't be a `{compare}`");
            }

            return true;
        }
    }
}
