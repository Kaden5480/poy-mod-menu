using System;
using System.Collections.Generic;

namespace ModMenu.Wrappers {
    public class Category {
        public string name { get; }
        public Dictionary<string, Entry> entries { get; }

        public Category(string name) {
            this.name = name;
            entries = new Dictionary<string, Entry>();
        }

        public void AddEntry(Entry entry) {
            if (entries.ContainsKey(entry.name) == true) {
                throw new Exception($"{name}: duplicate entry: {entry.name}");
            }

            entries[entry.name] = entry;
        }
    }
}
