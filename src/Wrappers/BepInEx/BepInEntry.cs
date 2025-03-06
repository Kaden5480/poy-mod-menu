using BepInEx.Configuration;

namespace ModMenu.Wrappers {
    public class BepInEntry : Entry {
        private ConfigEntryBase _entry;

        public BepInEntry(ConfigEntryBase entry) : base(entry.SettingType) {
            _entry = entry;
        }

        public override string name {
            get => _entry.Definition.Key;
        }

        public override object value {
            get => _entry.BoxedValue;
            set => _entry.BoxedValue = value;
        }
    }
}
