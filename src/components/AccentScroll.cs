using UILib;
using UILib.Components;

namespace ModMenu {
    internal class AccentScroll : ScrollView {
        /**
         * <summary>
         * Allows setting the theme of this scroll iew.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        protected override void SetThisTheme(Theme theme) {
            base.SetThisTheme(theme);
            background.color = theme.accent;
        }
    }
}
