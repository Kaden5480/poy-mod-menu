using BepInEx.Configuration;
using ModMenu.Config;

namespace ModMenuExamples {
    [Category("Inherited Category")]
    [IncludeAll]
    internal class BaseInherit {
        internal ConfigEntry<string> anInheritedString;

        /**
         * <summary>
         * Initializes the config.
         * </summary>
         * <param name="configFile">The config file to bind to</param>
         */
        internal BaseInherit(ConfigFile configFile) {
            anInheritedString = configFile.Bind(
                "General", "anInheritedString",
                "Hello!",
                "An entry which should be inherited."
            );
        }
    }
}
