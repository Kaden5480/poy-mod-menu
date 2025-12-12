using System;

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
        private Type parentType       = null;
        private object parentInstance = null;

        // The underlying type this field holds
        private Type type;

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
    }
}
