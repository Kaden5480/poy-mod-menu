using System;

namespace ModMenu.Config {
    /**
     * <summary>
     * An attribute which can be applied to classes and fields
     * that indicates the category they should be displayed under.
     * </summary>
     */
    [AttributeUsage(
        AttributeTargets.Class
        | AttributeTargets.Field
        | AttributeTargets.Property,
        AllowMultiple=false
    )]
    public class CategoryAttribute : Attribute {
        internal string name { get; }

        /**
         * <summary>
         * Initializes a category attribute with a given name.
         * </summary>
         * <param name="name">The name of the category</param>
         */
        public CategoryAttribute(string name) {
            this.name = name;
        }
    }
}
