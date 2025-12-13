using System;
using System.Collections.Generic;

using UnityEngine;

namespace ModMenu.Config {
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

        internal abstract object value { get; set; }

        /**
         * <summary>
         * Initializes a base field.
         * </summary>
         * <param name="type">The underlying type this field wraps around</param>
         */
        internal BaseField(Type type) {
            this.type = type;
        }

        private bool IsNumeric(Type type) {
            return type == typeof(int)
                || type == typeof(float)
                || type == typeof(double);
        }

        internal bool Validate() {
            return true;
        }

        internal void GuessFieldType() {
            if (fieldType != FieldType.None) {
                return;
            }

            if (type == typeof(Color)) {
                fieldType = FieldType.Color;
            }
            else if (type == typeof(KeyCode)) {
                fieldType = FieldType.KeyCode;
            }
            else if (type == typeof(bool)) {
                fieldType = FieldType.Toggle;
            }
            else if (IsNumeric(type) == true) {
                fieldType = FieldType.Slider;
            }
            else if (type == typeof(string)) {
                fieldType = FieldType.Text;
            }
            else if (type == typeof(Enum)) {
                fieldType = FieldType.Dropdown;
            }

            if (fieldType == FieldType.None) {
                Plugin.LogError($"Unable to guess best field type for: {type}");
            }
        }
    }
}
