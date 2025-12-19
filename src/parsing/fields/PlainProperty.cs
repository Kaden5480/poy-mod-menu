using System.Reflection;

using ModMenu.Config;

namespace ModMenu.Parsing {
    /**
     * <summary>
     * A property which wraps around a plain value.
     * </summary>
     */
    internal class PlainProperty : BaseField {
        private PropertyInfo info;

        internal override object value {
            get => info.GetValue(parentInstance);
            set => info.SetValue(parentInstance, value);
        }

        /**
         * <summary>
         * Performs extra validation specific to properties.
         * </summary>
         * <returns>Whether this property is valid</returns>
         */
        internal override bool Validate() {
            bool valid = true;

            if (base.Validate() == false) {
                valid = false;
            }

            if (info.GetMethod == null) {
                Plugin.LogError($"{name}: Properties must have a getter");
                valid = false;
            }

            if (info.SetMethod == null && fieldType != FieldType.ReadOnly) {
                Plugin.LogError($"{name}: Properties must have a setter if they aren't a `ReadOnly` field type");
                valid = false;
            }

            return valid;
        }

        /**
         * <summary>
         * Initializes a plain property.
         * </summary>
         * <param name="info">The property info to wrap around</param>
         * <param name="instance">The instance to access this property with</param>
         */
        internal PlainProperty(PropertyInfo info, object instance) : base(info.PropertyType) {
            this.info = info;
            parentInstance = instance;
            name = info.Name;
        }
    }
}
