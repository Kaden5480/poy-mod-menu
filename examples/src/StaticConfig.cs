using ModMenu.Config;

namespace ModMenuExamples {
    // You can also pass static configs to mod menu
    [IncludeAll]

    // By not specifying a [Category] mod menu
    // will use the name of this class instead
    internal static class StaticConfig {
        // Just like with `ConfigEntry` types, you can
        // use plain types
        [Field(FieldType.Slider, min=0f, max=10f)]
        internal static float cool = 0f;

        // Text fields can also have min/max
        // as long as the field is numeric
        [Field(FieldType.Text, min=0)]
        internal static int anInt = 1;

        // In general, mod menu is decent at inferring
        // the most likely field to use
        // So you don't always have to use a [Field] attribute
        internal static string name = "a name";

        // But, if you're not including everything using [IncludeAll],
        // you can still use a [Field] attribute to make sure the field
        // gets included
    }
}
