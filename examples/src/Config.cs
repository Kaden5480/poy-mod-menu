using BepInEx.Configuration;
using ModMenu.Config;

namespace ModMenuExamples {
    [Category("Custom Category")]
    internal class Config {
        [Category("Another category override")]
        [Field("My Option", fieldType=FieldType.Toggle)]
        internal ConfigEntry<bool> myOpt;

        [Field]
        internal ConfigEntry<float> myFl;

        internal Config(ConfigFile configFile) {
            myOpt = configFile.Bind(
                "General", "myOption", true,
                "An option which might be true"
            );
            myFl = configFile.Bind(
                "Cool Category", "myFloat", 0f,
                "A random float"
            );
        }
    }
}
