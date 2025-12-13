using BepInEx.Configuration;
using ModMenu.Config;

namespace ModMenuExamples {
    [Category("Custom Category")]
    [IncludeAll]
    internal class Config {
        [Category("Another category override")]
        [Field("My Option", fieldType=FieldType.Toggle)]
        internal ConfigEntry<bool> myOpt;

        internal ConfigEntry<float> myFl;

        internal ConfigEntry<int> myHiddenInt;

        [Exclude]
        internal ConfigFile avoid = null;

        internal Config(ConfigFile configFile) {
            myOpt = configFile.Bind(
                "General", "myOption", true,
                "An option which might be true"
            );
            myFl = configFile.Bind(
                "Cool Category", "myFloat", 0f,
                "A random float"
            );
            myHiddenInt = configFile.Bind(
                "General", "hidden", 123,
                "A hidden int"
            );
        }
    }
}
