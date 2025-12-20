using BepInEx.Configuration;
using ModMenu.Config;

namespace ModMenuExamples {
    [Category("Custom Category")]
    [IncludeAll]
    internal class Config {
        [Category("Another category override")]
        [Field("My Option", FieldType.Toggle)]
        internal ConfigEntry<bool> myOpt;

        internal ConfigEntry<int> myHiddenInt;

        [Exclude]
        internal ConfigFile avoid = null;

        // Testing predicates
        [Predicate(typeof(Config), nameof(TestPredicate))]
        [Field(FieldType.Slider, min=0f, max=100f)]
        internal ConfigEntry<float> myFl;

        private static string TestPredicate(float num) {
            if (num == 60f) {
                return "The value cannot be 60";
            }

            return null;
        }


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
