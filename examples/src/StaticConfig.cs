using ModMenu.Config;

namespace ModMenuExamples {
    [IncludeAll]
    internal static class StaticConfig {
        [Field(fieldType=FieldType.Slider, min=0f, max=100f)]
        internal static float cool = 0f;
        internal static int anInt = 1;
        internal static string name = "a name";
    }
}
