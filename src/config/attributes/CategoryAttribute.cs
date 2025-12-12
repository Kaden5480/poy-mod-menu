using System;

namespace ModMenu.Config {
    [AttributeUsage(
        AttributeTargets.Class
        | AttributeTargets.Field,
        AllowMultiple=false
    )]
    public class CategoryAttribute : Attribute {
        internal string name;

        public CategoryAttribute(string name) {
            this.name = name;
        }
    }
}
