using System;

namespace ModMenu.Config {
    /**
     * <summary>
     * An attribute which allows you to customise how
     * a field will be displayed.
     *
     * This also provides extra information which can be
     * used for data validation.
     *
     * `FieldAttributes` can be applied to either "plain" types
     * or BepInEx `ConfigEntry` types.
     *
     * A "plain" type is something like an `int`, `float`, `double`, etc.
     * </summary>
     */
    [AttributeUsage(
        AttributeTargets.Field
        | AttributeTargets.Property,
        AllowMultiple=true
    )]
    public class FieldAttribute : Attribute {
        private string _name = null;
        private string _description = null;
        private FieldType _fieldType = FieldType.None;
        private object _min = null;
        private object _max = null;
        private object _defaultValue = null;

        /**
         * <summary>
         * Initializes a field attribute.
         * </summary>
         */
        public FieldAttribute() {}

        /**
         * <summary>
         * Initializes a field attribute with a specified name
         * and optional description.
         * </summary>
         * <param name="name">The display name to use</param>
         * <param name="description">The description to display in a tooltip</param>
         */
        public FieldAttribute(string name, string description = null) {
            this.name = name;
            this.description = description;
        }

        /**
         * <summary>
         * Initializes a field attribute with a specified field type
         * and an optional description.
         * </summary>
         * <param name="fieldType">The field type to use</param>
         * <param name="description">The description to display in a tooltip</param>
         */
        public FieldAttribute(FieldType fieldType, string description = null) {
            this.fieldType = fieldType;
            this.description = description;
        }

        /**
         * <summary>
         * Initializes a field attribute with a specified name
         * and field type.
         * </summary>
         * <param name="name">The display name to use</param>
         * <param name="fieldType">The field type to use</param>
         */
        public FieldAttribute(string name, FieldType fieldType) {
            this.name = name;
            this.fieldType = fieldType;
        }

        /**
         * <summary>
         * Initializes a field attribute with a specified
         * name, description, and field type.
         * </summary>
         * <param name="name">The display name to use</param>
         * <param name="description">The description to display in a tooltip</param>
         * <param name="fieldType">The field type to use</param>
         */
        public FieldAttribute(string name, string description, FieldType fieldType) {
            this.name = name;
            this.description = description;
            this.fieldType = fieldType;
        }

        /**
         * <summary>
         * The display name of this field.
         *
         * Unspecified (null) names behave like so:
         * - `ConfigEntry`: The name configured in the `ConfigEntry` will be used
         * - Plain: The name of the field will be used
         * </summary>
         */
        public virtual string name {
            get => _name;
            set => _name = value;
        }

        /**
         * <summary>
         * A brief description of this field.
         *
         * This will display in a tooltip.
         *
         * Unspecified (null) descriptions behave like so:
         * - `ConfigEntry`: The description configured in the `ConfigEntry` will be used
         * - Plain: The description will be left blank
         * </summary>
         */
        public virtual string description {
            get => _description;
            set => _description = value;
        }

        /**
         * <summary>
         * The type of this field. This determines
         * which `UIComponent` will be used.
         * </summary>
         */
        public virtual FieldType fieldType {
            get => _fieldType;
            set => _fieldType = value;
        }

        /**
         * <summary>
         * The minimum value permitted.
         *
         * This field must be a numeric type
         * for you to use this.
         * </summary>
         */
        public virtual object min {
            get => _min;
            set => _min = value;
        }

        /**
         * <summary>
         * The maximum value permitted.
         *
         * This field must be a numeric type
         * for you to use this.
         * </summary>
         */
        public virtual object max {
            get => _max;
            set => _max = value;
        }

        /**
         * <summary>
         * The default value.
         *
         * Unspecified (null) defaults behave like so:
         * - `ConfigEntry`: The default value configured in the `ConfigEntry` will be used
         * - Plain: There will be no default value
         * </summary>
         */
        public virtual object defaultValue {
            get => _defaultValue;
            set => _defaultValue = value;
        }
    }
}
