using System;

namespace ModMenu.Config {
    /**
     * <summary>
     * An attribute which tells mod menu to include
     * all fields in a class (apart from
     * <see cref="ExcludeAttribute">excluded ones</see>).
     * </summary>
     */
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public class IncludeAllAttribute : Attribute {
        public IncludeAllAttribute() {}
    }
}
