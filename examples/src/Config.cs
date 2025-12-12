namespace ModMenuExamples {
    internal class Config {
        internal ConfigEntry<bool> myOption;

        internal Config(ConfigFile configFile) {
            configFile.Bind(
                "General", "myOption", true
                "An option which might be true"
            );

        }
    }
}
