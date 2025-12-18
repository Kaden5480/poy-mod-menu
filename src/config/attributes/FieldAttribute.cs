using System;

namespace ModMenu.Config {
    /**
     * <summary>
     * An attribute which indicates a field to mod menu.
     *
     * This also provides extra information which can be used
     * to determine how mod menu handles this field.
     * </summary>
     */
    [AttributeUsage(AttributeTargets.Field)]
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
         * Initializes a field attribute with a given name.
         * </summary>
         * <param name="name">The display name to use</param>
         */
        public FieldAttribute(string name) {
            this.name = name;
        }

        /**
         * <summary>
         * Initializes a field attribute of a given type.
         * </summary>
         * <param name="fieldType">The field type to use</param>
         */
        public FieldAttribute(FieldType fieldType) {
            this.fieldType = fieldType;
        }

        /**
         * <summary>
         * Initializes a field attribute with a given name
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
         * The display name of this field.
         * </summary>
         */
        public virtual string name {
            get => _name;
            set => _name = value;
        }

        /**
         * <summary>
         * A brief description of this field.
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
         * </summary>
         */
        public virtual object defaultValue {
            get => _defaultValue;
            set => _defaultValue = value;
        }
    }
}
