using System.Reflection;

using ModMenu.Config;

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

            if (info.IsLiteral == true || info.IsInitOnly == true) {
                fieldType = FieldType.ReadOnly;
                return true;
            }

            return base.GuessFieldType(quiet);
        }

        /**
         * <summary>
         * Validates whether the current configuration of this field is compatible.
         * </summary>
         * <returns>Whether this configuration is valid</returns>
         */
        internal override bool Validate() {
            bool valid = true;

            if (base.Validate() == false) {
                valid = false;
            }

            if (info.IsInitOnly == true && fieldType != FieldType.ReadOnly) {
                LogError($"Read-only fields must be a `ReadOnly` type");
                return false;
            }

            return valid;
        }

        /**
         * <summary>
         * Initializes a plain field.
         * </summary>
         * <param name="modInfo">The mod this field is for</param>
         * <param name="info">The field info to wrap around</param>
         * <param name="instance">The instance to access this field with</param>
         */
        internal PlainField(ModInfo modInfo, FieldInfo info, object instance)
            : base(modInfo, info, info.FieldType)
        {
            this.info = info;
            parentInstance = instance;
            name = info.Name;
        }
    }
}
