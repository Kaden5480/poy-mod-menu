using System;

namespace ModMenu.Config {
    public class FieldAttribute : Attribute {
        private string _name;
        private string _description;
        private FieldType _fieldType;

        private object _min;
        private object _max;

        public FieldAttribute() {}

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
