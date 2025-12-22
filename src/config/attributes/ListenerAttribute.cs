using System;

namespace ModMenu.Config {
    /**
     * <summary>
     * An attribute which provides a way of knowing
     * when Mod Menu has changed a field's value.
     *
     * Listeners *must* be static methods.
     *
     * For listener attributes applied to fields:
     * - They must take a single value, which must match the type of the field
     * - If you use a `ConfigEntry&lt;T&gt;`, Mod Menu will unwrap this type
     *   and use the inner `T` instead.
     *
     * For listener attributes applied to classes:
     * - They must take two values, which are a
     *   <see cref="System.Reflection.MemberInfo"/> and an <see cref="object"/>
     *   (in this order).
     * - The first argument is the `MemberInfo` of the field/property which was updated
     * - The second argument is the value it was set to
     * </summary>
     */
    [AttributeUsage(
        AttributeTargets.Class
        | AttributeTargets.Field
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
