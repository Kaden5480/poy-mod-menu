using System;

namespace ModMenu.Config {
    public class FieldAttribute : Attribute {
        private string _name = null;
        private string _description = null;
        private FieldType _fieldType = FieldType.None;

        private object _min = null;
        private object _max = null;

        public FieldAttribute() {}

        public FieldAttribute(string name) {
            this.name = name;
        }

        public virtual string name {
            get => _name;
            set => _name = value;
        }

        public virtual string description {
            get => _description;
            set => _description = value;
        }

        public virtual FieldType fieldType {
            get => _fieldType;
            set => _fieldType = value;
        }

        public virtual object min {
            get => _min;
            set => _min = value;
        }

        public virtual object max {
            get => _max;
            set => _max = value;
        }
    }
}
