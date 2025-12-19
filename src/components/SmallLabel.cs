using UILib;
using UILib.Components;

namespace ModMenu {
    internal class SmallLabel : Label {
        /**
         * <summary>
         * Initializes a small label.
         * </summary>
         * <param name="text">The text to display</param>
         * <param name="fontSize">The size of the font</param>
         */
        internal SmallLabel(string text, int fontSize)
            : base(text, fontSize) {}

        /**
         * <summary>
         * Allows setting the theme of this small label.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        protected override void SetThisTheme(Theme theme) {
            base.SetThisTheme(theme);
            SetColor(theme.selectAltNormal);
        }
    }
}
