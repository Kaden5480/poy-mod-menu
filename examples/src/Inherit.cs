using BepInEx.Configuration;
using ModMenu.Config;

namespace ModMenuExamples {
    // Notice how this class has no fields itself
    // You can use the `InheritFields` attribute to tell mod
    // menu to parse extra fields from the base class
    [InheritFields]
    internal class Inherit : BaseInherit {
        /**
         * <summary>
         * Initializes the config.
         * </summary>
         * <param name="configFile">The config file to bind to</param>
         */
        internal Inherit(ConfigFile configFile) : base(configFile) {}
    }
}
