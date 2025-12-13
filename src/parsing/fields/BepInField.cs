using BepInEx.Configuration;

namespace ModMenu.Parsing {
    /**
     * <summary>
     * A field which wraps around a BepInEx ConfigEntry.
     * </summary>
     */
    internal class BepInField : BaseField {
        private ConfigEntryBase entry;

        internal override object value {
            get => entry.BoxedValue;
            set => entry.BoxedValue = value;
        }

        /**
         * <summary>
         * Initializes a BepIn field.
         * </summary>
         * <param name="entry">The entry to wrap around</param>
         */
        internal BepInField(ConfigEntryBase entry) : base(entry.SettingType) {
            this.entry = entry;
            name = entry.Definition.Key;
            description = entry.Description.Description;
            defaultValue = entry.DefaultValue;
        }
    }
}
