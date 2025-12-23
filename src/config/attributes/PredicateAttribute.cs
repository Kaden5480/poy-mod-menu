using System;

namespace ModMenu.Config {
    /**
     * <summary>
     * An attribute which allows you to check a user input
     * before Mod Menu updates a field's value.
     *
     * Predicates *must* be static methods.
     *
     * When the predicate returns `null`, the field can be updated.
     * If a string is returned, this prevents the field from updating.
     *
     * The predicate attribute takes a `type` and a `name`
     * indicating which type holds the predicate method
     * and what the name of the method is.
     *
     * You can also provide `generics` for specific
     * overloads of a given method.
     *
     * The method itself must take a single argument which
     * has the same type as the field this attribute is applied to
     * and it must return a `string` (or `null`).
     *
     * You can also apply multiple predicates, but the first to fail
     * will be the one which displays its error.
     *
     * NOTE:
     * If you use a `ConfigEntry&lt;T&gt;`, Mod Menu will unwrap this type
     * and use the inner `T` instead.
     * </summary>
     */
    [AttributeUsage(
        AttributeTargets.Field
        | AttributeTargets.Property,
        AllowMultiple=true
    )]
    public class PredicateAttribute : Attribute {
        internal Type type       { get; }
        internal string name     { get; }
        internal Type[] generics { get; }

        /**
         * <summary>
         * Initializes a predicate attribute.
         * </summary>
         * <param name="type">The type the predicate method is contained within</param>
         * <param name="name">The name of the predicate method</param>
         */
        public PredicateAttribute(Type type, string name) {
            this.type = type;
            this.name = name;
        }

        /**
         * <summary>
         * Initializes a predicate attribute.
         * </summary>
         * <param name="type">The type the predicate method is contained within</param>
         * <param name="name">The name of the predicate method</param>
         * <param name="generics">Specific generic parameters, if necessary</param>
         */
        public PredicateAttribute(Type type, string name, Type[] generics) : this(type, name) {
            this.generics = generics;
        }
    }
}
