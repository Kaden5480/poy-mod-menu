using System.Reflection;

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
         * <param name="modInfo>The mod this field is for</param>
         * <param name="memberInfo">This field's member info</param>
         * <param name="entry">The entry to wrap around</param>
         */
        internal BepInField(ModInfo modInfo, MemberInfo memberInfo, ConfigEntryBase entry)
            : base(modInfo, memberInfo, entry.SettingType)
        {
            this.entry = entry;
            name = entry.Definition.Key;
            description = entry.Description.Description;
            defaultValue = entry.DefaultValue;
        }
    }
}
