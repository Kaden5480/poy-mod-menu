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

        /**
         * <summary>
         * Initializes a blank metadata.
         * </summary>
         */
        public MetaData() {}

        /**
         * <summary>
         * Initializes metadata from a variety of tags.
         * </summary>
         * <param name="tags">The tags to use</param>
         */
        public MetaData(IList<string> tags) {
            Add(tags);
        }

        /**
         * <summary>
         * Initializes metadata from a given field.
         * </summary>
         * <param name="field">The field to generate metadata for</param>
         */
        internal MetaData(BaseField field) {
            Add($"{field.name}{field.description}{field.value}");
        }

        /**
         * <summary>
         * Initializes metadata from mod info.
         * </summary>
         * <param name="modInfo">The mod info to generate metadata for</param>
         */
        internal MetaData(ModInfo modInfo) {
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
            metadata += string.Join("", tags).ToLower();
        }

        /**
         * <summary>
         * Whether this metadata matches a given search query.
         * </summary>
         * <param name="query">The query to match against</param>
         * <returns>Whether it matches</returns>
         */
        internal bool Matches(string query) {
            return metadata.Contains(query);
        }
    }
}
