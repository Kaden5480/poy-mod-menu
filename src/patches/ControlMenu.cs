using HarmonyLib;

namespace ModMenu.Patches {
    /**
     * <summary>
     * Prevents interacting with the InGameMenu while
     * mod menu is open.
     * </summary>
     */
    [HarmonyPatch(typeof(InGameMenu), "Update")]
    internal static class ControlMenu {
        private static bool Prefix() {
            if (Plugin.ui == null) {
                return true;
            }

            if (Plugin.ui.overlay.isVisible == true) {
                return false;
            }

            return true;
        }
    }
}
