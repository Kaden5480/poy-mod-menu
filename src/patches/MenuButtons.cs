using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UEButton = UnityEngine.UI.Button;

using UILib;
using UILib.Notifications;

namespace ModMenu.Patches {
    /**
     * <summary>
     * Injects the custom "Mods" button into
     * the main menu and the pause menu.
     * </summary>
     */
    internal static class MenuButtons {
        private static Logger logger = new Logger(typeof(MenuButtons));

        /**
         * <summary>
         * Instantiates the mod options button.
         * </summary>
         * <param name="root">The root to instantiate from</param>
         */
        private static void ModsInit(string root) {
            GameObject menuContainer = GameObject.Find(root);
            if (menuContainer == null) {
                logger.LogDebug("Unable to find menu container");
                return;
            }

            Transform options = menuContainer.transform.Find("Options");
            if (options == null) {
                logger.LogDebug("Unable to find options");
                return;
            }

            // Instantiate as the mod options button
            GameObject obj = GameObject.Instantiate(
                options.gameObject, menuContainer.transform
            );
            if (obj == null) {
                logger.LogDebug("Failed instantiating mod options object");
                return;
            }
            obj.name = "ModMenuOptions";
            obj.transform.SetSiblingIndex(2);

            // Remove old components
            UEButton oldButton = obj.GetComponent<UEButton>();
            if (oldButton == null) {
                logger.LogDebug("Original button doesn't exist");
                return;
            }

            // Save the original color block
            ColorBlock originalBlock = oldButton.colors;
            GameObject.DestroyImmediate(oldButton);

            // Destroy the image, it's unnecessary
            Image image = obj.GetComponent<Image>();
            if (image != null) {
                GameObject.DestroyImmediate(image);
            }

            // Get the text
            Transform textT = obj.transform.Find("Text");
            if (textT == null) {
                logger.LogDebug("Unable to find text object");
                return;
            }

            Text text = textT.gameObject.GetComponent<Text>();
            if (text == null) {
                logger.LogDebug("Unable to find text component");
                return;
            }

            text.text = "Mods";

            // Add a new button
            UEButton button = obj.AddComponent<UEButton>();
            button.onClick.AddListener(() => {
                Audio.PlayNavigation();
                EventSystem.current.SetSelectedGameObject(null);
                if (Plugin.ui != null) {
                    Plugin.ui.Show();
                }
            });

            button.colors = originalBlock;
            button.targetGraphic = text;
        }

        /**
         * <summary>
         * Called on a scene load and determines which
         * UI to inject a button into.
         * </summary>
         * <param name="scene">The scene which loaded</param>
         */
        internal static void Inject(Scene scene) {
            if (scene.buildIndex == 0) {
                logger.LogDebug("Building main menu inject");
                ModsInit("Canvas/InGameMenu/InGameMenuObj_DisableMe/menu_pg/MainLayoutMenu");
                return;
            }

            logger.LogDebug("Building pause menu object");
            ModsInit("Canvas/InGameMenu/InGameMenuObj_DisableMe/menu_pg/MenuContainer");
        }
    }
}
