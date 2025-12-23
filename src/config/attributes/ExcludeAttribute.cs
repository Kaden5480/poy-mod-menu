using System;

namespace ModMenu.Config {
    /**
     * <summary>
     * An attribute which tells Mod Menu to
     * exclude a field or property.
     * </summary>
     */
    [AttributeUsage(
        AttributeTargets.Field
        | AttributeTargets.Property,
        AllowMultiple=false
    )]
    public class ExcludeAttribute : Attribute {
        public ExcludeAttribute() {}
    }
}
