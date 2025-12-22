using System.Collections.Generic;

using ModMenu.Parsing;

namespace ModMenu.Views {
    /**
     * <summary>
     * Holds search metadata.
     * </summary>
     */
    public class MetaData {
        private string metadata = "";
        internal string description { get; private set; } = null;

        /**
         * <summary>
         * Initializes a blank metadata.
         * </summary>
         */
        public MetaData() {}

        /**
         * <summary>
         * Initializes metadata from a variety of tags.
         *
         * Optionally, you can also provide a description, which
         * will be displayed in a tooltip.
         * </summary>
         * <param name="tags">The tags to use</param>
         * <param name="description">A description for the metadata</param>
         */
        public MetaData(IList<string> tags, string description = null) {
            Add(tags);
            SetDescription(description);
        }

        /**
         * <summary>
         * Initializes metadata from a given field.
         * </summary>
         * <param name="field">The field to generate metadata for</param>
         */
        internal MetaData(BaseField field) {
            SetDescription(field.description);
            Add($"{field.name}{field.description}{field.value}");
        }

        /**
         * <summary>
         * Initializes metadata from mod info.
         * </summary>
         * <param name="modInfo">The mod info to generate metadata for</param>
         */
        internal MetaData(ModInfo modInfo) {
            SetDescription(modInfo.description);
            Add($"{modInfo.name}{modInfo.version}{modInfo.description}");
        }

        /**
         * <summary>
         * Adds another tag to the metadata.
         * </summary>
         * <param name="tag">The tag to add</param>
         */
        public void Add(string tag) {
            metadata += tag.ToLower();
        }

        /**
         * <summary>
         * Adds more tags to the metadata.
         * </summary>
         * <param name="tags">The tags to add</params>
         */
        public void Add(IList<string> tags) {
            if (tags == null) {
                return;
            }

            metadata += string.Join("", tags).ToLower();
        }

        /**
         * <summary>
         * Sets the description of this metadata (displayed
         * in a tooltip).
         * </summary>
         * <param name="description">The description to set</param>
         */
        public void SetDescription(string description) {
            this.description = description;
        }

        /**
         * <summary>
         * Whether this metadata matches a given search query.
         * </summary>
         * <param name="query">The query to match against</param>
         * <returns>Whether it matches</returns>
         */
        internal bool Matches(string query) {
            return metadata.Contains(query) || description.ToLower().Contains(query);
        }
    }
}
