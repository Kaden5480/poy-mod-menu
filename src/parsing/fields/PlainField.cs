using System.Reflection;

namespace ModMenu.Parsing {
    /**
     * <summary>
     * A field which wraps around a plain value.
     * </summary>
     */
    internal class PlainField : BaseField {
        private FieldInfo info;

        internal override object value {
            get => info.GetValue(parentInstance);
            set => info.SetValue(parentInstance, value);
        }

        /**
         * <summary>
         * Initializes a plain field.
         * </summary>
         * <param name="info">The field info to wrap around</param>
         * <param name="instance">The instance to access this field with</param>
         */
        internal PlainField(FieldInfo info, object instance) : base(info.FieldType) {
            this.info = info;
            parentInstance = instance;
            name = info.Name;
        }
    }
}
