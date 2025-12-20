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
         * Guesses the most likely field type for
         * the underlying type.
         * </summary>
         * <param name="quiet">Whether to prevent logging</param>
         * <returns>Whether guessing the type was successful</returns>
         */
        internal override bool GuessFieldType(bool quiet = false) {
            if (fieldType != FieldType.None) {
                return true;
            }

            if (info.SetMethod == null) {
                fieldType = FieldType.ReadOnly;
                return true;
            }

            return base.GuessFieldType(quiet);
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
         * <param name="modInfo">The mod this property is for</param>
         * <param name="info">The property info to wrap around</param>
         * <param name="instance">The instance to access this property with</param>
         */
        internal PlainProperty(ModInfo modInfo, PropertyInfo info, object instance)
            : base(modInfo, info.PropertyType)
        {
            this.info = info;
            parentInstance = instance;
            name = info.Name;
        }
    }
}
