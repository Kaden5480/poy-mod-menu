using System.Collections.Generic;

using ModMenu.Parsing;

namespace ModMenu.Views {
    /**
     * <summary>
     * Holds search metadata.
     * </summary>
     */
    public class MetaData {
        private string metadata;

        /**
         * <summary>
         * Initializes metadata from a variety of tags.
         * </summary>
         * <param name="tags">The tags to use</param>
         */
        public MetaData(IList<string> tags) {
            metadata = string.Join("", tags).ToLower();
        }

        /**
         * <summary>
         * Initializes metadata from a given field.
         * </summary>
         * <param name="field">The field to generate metadata for</param>
         */
        internal MetaData(BaseField field) {
            metadata = $"{field.name}{field.description}{field.value}".ToLower();
        }

        /**
         * <summary>
         * Initializes metadata from mod info.
         * </summary>
         * <param name="modInfo">The mod info to generate metadata for</param>
         */
        internal MetaData(ModInfo modInfo) {
            metadata = $"{modInfo.name}{modInfo.version}{modInfo.description}".ToLower();
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
