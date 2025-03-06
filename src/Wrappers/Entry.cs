using System;

namespace ModMenu.Wrappers {
    public abstract class Entry {
        public Type type { get; }

        public abstract string name { get; }
        public abstract object value { get; set; }

        public Entry(Type type) {
            this.type = type;
        }
    }
}
