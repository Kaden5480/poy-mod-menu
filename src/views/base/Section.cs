using System.Collections.Generic;

using UILib;
using UILib.Components;
using UILib.Layouts;

using ModMenu.Parsing;

namespace ModMenu.Views {
    /**
     * <summary>
     * A class representing a section within a view.
     * </summary>
     */
    internal class Section {
        private string name;
        private List<Entry> entries;

        internal UIComponent root { get; private set; }

        /**
         * <summary>
         * Initializes a section.
         * </summary>
         * <param name="name">The name of this section</param>
         */
        internal Section(string name) {
            this.name = name;
            entries = new List<Entry>();
        }

        /**
         * <summary>
         * Adds an entry to this section.
         * </summary>
         * <param name="entry">The entry to add</param>
         */
        internal void Add(Entry entry) {
            entries.Add(entry);
        }

        /**
         * <summary>
         * Builds the UIComponent for this section.
         * </summary>
         * <returns>The component which was built</returns>
         */
        internal UIComponent Build() {
            if (root != null) {
                return root;
            }

            root = new Area();
            root.SetContentLayout(LayoutType.Vertical);
            root.SetElementSpacing(10);
            root.SetFill(FillType.All);

            // Add the title
            Label title = new Label(name, 30);
            title.SetFill(FillType.Horizontal);
            title.SetSize(0f, 50f);
            root.Add(title);

            // Add each entry
            foreach (Entry entry in entries) {
                if (entry.component == null) {
                    continue;
                }

                root.Add(entry.component);
            }

            return root;
        }
    }
}
