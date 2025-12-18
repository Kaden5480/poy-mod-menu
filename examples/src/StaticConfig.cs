using ModMenu.Config;

namespace ModMenuExamples {
    [IncludeAll]
    internal static class StaticConfig {
        [Field(FieldType.Slider, min=0f, max=10f)]
        internal static float cool = 0f;

        [Field(FieldType.Text, min=0)]
        internal static int anInt = 1;

        internal static string name = "a name";
    }
}
