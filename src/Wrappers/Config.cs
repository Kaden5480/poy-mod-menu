using System.Collections.Generic;

namespace ModMenu.Wrappers {
    public abstract class Config {
        public string name { get; }
        public Dictionary<string, Category> categories { get; }

        public Config(string name) {
            this.name = name;
            categories = new Dictionary<string, Category>();
        }

        public Category GetAddCategory(string name) {
            if (categories.ContainsKey(name) == false) {
                categories[name] = new Category(name);
            }

            return categories[name];
        }

        public abstract void Save();
    }
}
