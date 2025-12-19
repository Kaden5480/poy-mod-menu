using System;

namespace ModMenu.Config {
    /**
     * <summary>
     * An attribute which tells mod menu to
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
