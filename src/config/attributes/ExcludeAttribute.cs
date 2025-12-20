using System;

namespace ModMenu.Config {
    /**
     * <summary>
     * An attribute which tells Mod Menu to
     * exclude a field.
     * </summary>
     */
    [AttributeUsage(
        AttributeTargets.Field,
        AllowMultiple=false
    )]
    public class ExcludeAttribute : Attribute {
        public ExcludeAttribute() {}
    }
}
