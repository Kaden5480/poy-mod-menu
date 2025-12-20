using System;

namespace ModMenu.Config {
    /**
     * <summary>
     * An attribute which provides a way of knowing
     * when Mod Menu has changed a field's value.
     *
     * Listeners *must* be static methods.
     *
     * They must also take a single value, which must be of the type of the field
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
    public class ListenerAttribute : Attribute {
        internal Type type       { get; }
        internal string name     { get; }
        internal Type[] generics { get; }

        /**
         * <summary>
         * Initializes a listener attribute.
         * </summary>
         * <param name="type">The type the listener method is contained within</param>
         * <param name="name">The name of the listener method</param>
         */
        public ListenerAttribute(Type type, string name) {
            this.type = type;
            this.name = name;
        }

        /**
         * <summary>
         * Initializes a listener attribute.
         * </summary>
         * <param name="type">The type the listener method is contained within</param>
         * <param name="name">The name of the listener method</param>
         * <param name="generics">Specific generic parameters, if necessary</param>
         */
        public ListenerAttribute(Type type, string name, Type[] generics) : this(type, name) {
            this.generics = generics;
        }
    }
}
