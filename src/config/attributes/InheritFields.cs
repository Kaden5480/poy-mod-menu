using System;

namespace ModMenu.Config {
    /**
     * <summary>
     * An attribute which can be applied to classes
     * which indicates to parse annotations from base
     * types as well.
     * </summary>
     */
    [AttributeUsage(
        AttributeTargets.Class,
        AllowMultiple=false
    )]
    public class InheritFieldsAttribute: Attribute {
        public InheritFieldsAttribute() {}
    }
}
