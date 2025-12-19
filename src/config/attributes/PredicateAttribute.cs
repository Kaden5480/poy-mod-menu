using System;

namespace ModMenu.Config {
    public class PredicateAttribute : Attribute {
        internal Type type   { get; }
        internal string name { get; }

        /**
         * <summary>
         * Initializes a predicate attribute.
         *
         * The predicate attribute takes a `type` and a `name`
         * indicating which type holds the predicate method
         * and what the name of the method is.
         *
         * The method itself must take a single argument which
         * has the same type as the field this attribute is applied to.
         *
         * NOTE:
         * If you use a `ConfigEntry&lt;T&gt;`, ModMenu will unwrap this type
         * and use the inner `T` instead.
         *
         * </summary>
         * <param name="type">The type the predicate method is contained within</param>
         * <param name="name">The name of the predicate method</param>
         */
        public PredicateAttribute(Type type, string name) {
            this.type = type;
            this.name = name;
        }
    }
}
