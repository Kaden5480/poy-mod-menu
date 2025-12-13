using System;

namespace ModMenu.Config {
    /**
     * <summary>
     * An attribute which tells mod menu to include
     * all fields in a given type, saving you having
     * to define each one.
     * </summary>
     */
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public class IncludeAllAttribute : Attribute {
        public IncludeAllAttribute() {}
    }
}
