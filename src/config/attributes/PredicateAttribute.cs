using System;

namespace ModMenu.Config {
    public class PredicateAttribute : Attribute {
        internal Type type   { get; }
        internal string name { get; }

        public PredicateAttribute(Type type, string name) {
            this.type = type;
            this.name = name;
        }
    }
}
