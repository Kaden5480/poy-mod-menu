using System.Reflection;

using BepInEx.Configuration;
using ModMenu.Config;
using UILib.Notifications;

namespace ModMenuExamples {
    // By default, category attributes applied to types
    // will be inherited by all fields
    [Category("Custom Category")]

    // A listener applied to classes will trigger when
    // any field (under this type) that is registered with mod menu changes
    [Listener(typeof(Config), nameof(Config.ClassListener))]

    // Specifying this attribute means you don't have to place
    // [Field] above each field, all of them will be included
    // in the mod's config view
    [IncludeAll]
    internal class Config {
        // If you want, you can override the category
        [Category("Another category override")]
        [Field("My Option", FieldType.Toggle)]
        internal ConfigEntry<bool> myOpt;

        // This is included implicitly, as the [IncludeAll] attribute
        // was applied to `Config`
        internal ConfigEntry<int> myRandomInt;

        // If you want to explicitly exclude a field, you can use
        // this attribute
        [Exclude]
        internal ConfigFile avoid = null;

        // Predicates allow you to perform arbitrary checks
        // on values before mod menu updates them.
        [Predicate(typeof(Config), nameof(TestPredicate))]

        // Apply a field listener (see TestListener below)
        [Listener(typeof(Config), nameof(TestListener))]

        // Make this field a slider between certain limits
        [Field(FieldType.Slider, min=0f, max=100f)]
        internal ConfigEntry<float> myFl;

        /**
         * <summary>
         * Predicates must take one argument of the same
         * type as the field.
         *
         * They return a string error on failure, which will be
         * sent in a notification.
         *
         * On success, they return null to indicate no error.
         * </summary>
         * <param name="num">The number to verify</param>
         * <returns>An error if the number is valid, null otherwise</returns>
         */
        private static string TestPredicate(float num) {
            if (num == 60f) {
                return "The value cannot be 60";
            }

            return null;
        }

        /**
         * <summary>
         * A class listener, this one was configured by
         * the [Listener] attribute on the `Config` type.
         *
         * This will execute whenever any field that mod menu has
         * access to (under this type) is changed.
         * </summary>
         * <param name="info">The member info of the field that changed</param>
         * <param name="value">The value it was changed to</param>
         */
        private static void ClassListener(MemberInfo info, object value) {
            Notifier.Notify("Mod Menu Examples", $"{info} changed to {value}");
        }

        /**
         * <summary>
         * A field listener, this was placed above the `myFl` field.
         *
         * This is invoked whenever the field is updated
         * and must take 1 argument of the type of the field.
         * </summary>
         * <param name="value">The value the field changed to</param>
         */
        private static void TestListener(float value) {
            Notifier.Notify("Mod Menu Examples", $"Got value from listener: {value}");
        }

        /**
         * <summary>
         * Initializes the config.
         * </summary>
         * <param name="configFile">The config file to bind to</param>
         */
        internal Config(ConfigFile configFile) {
            // Something to note about `ConfigEntry` types is if you don't
            // specify a [Category] on a class containing them, or
            // on their field, they will use the "Section" configured
            // within them instead.
            // In this case, "General" would be used (unless the category was overriden)
            myOpt = configFile.Bind(
                "General", "myOption", true,
                "An option which might be true."
            );
            myFl = configFile.Bind(
                "Cool Category", "myFloat", 0f,
                "A random float."
            );
            myRandomInt = configFile.Bind(
                "General", "random", 123,
                "A random int."
            );
        }
    }
}
