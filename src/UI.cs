using System.Collections.Generic;

using BepInEx;
using UILib;
using UILib.Components;
using UILib.Layouts;
using UnityEngine;

using UIButton = UILib.Components.Button;

namespace ModMenu {
    internal class UI {
        private static UI instance;

        private Logger logger = new Logger(typeof(UI));

        private bool builtModList = false;
        private Area modList;

        internal Overlay overlay;
        internal ScrollView scrollView;

        private Image background;
        private Image topBar;

        private ConfigBuilder currentView;

        /**
         * <summary>
         * Initializes mod menu's UI.
         * </summary>
         */
        internal UI() {
            instance = this;

            overlay = new Overlay(0f, 0f);
            overlay.SetFill(FillType.All);
            overlay.SetSortable(false);

            background = new Image();
            background.SetFill(FillType.All);
            overlay.Add(background);

            // Top bar
            topBar = new Image();
            topBar.SetAnchor(AnchorType.TopMiddle);
            topBar.SetFill(FillType.Horizontal);
            topBar.SetSize(0f, 100f);
            overlay.Add(topBar);

            Label topBarTitle = new Label("Mod Menu", 55);
            topBarTitle.SetFill(FillType.Vertical);
            topBarTitle.SetSize(300f, 0f);
            topBar.Add(topBarTitle);

            // Mod list
            scrollView = new ScrollView();
            scrollView.SetFill(FillType.All);
            // Leave room for topbar
            scrollView.SetSize(0f, -100f);
            scrollView.SetOffset(0, -50f);
            overlay.Add(scrollView);

            modList = new Area();
            modList.SetAnchor(AnchorType.TopMiddle);
            modList.SetContentLayout(LayoutType.Vertical);
            modList.SetContentPadding(20, 20, 60, 60);
            modList.SetElementSpacing(20);

            // Add manually to prevent theme recursion
            modList.gameObject.transform.SetParent(
                scrollView.scrollContent.gameObject.transform, false
            );

            Label modTitle = new Label("Installed Mods", 45);
            modTitle.SetSize(400f, 40f);
            modList.Add(modTitle);

            Area space = new Area();
            space.SetSize(0f, 20f);
            modList.Add(space);

            SetTheme(overlay.theme);
        }

        /**
         * <summary>
         * Shows the UI.
         * </summary>
         */
        internal void Show() {
            if (builtModList == false) {
                BuildModList();
                builtModList = true;
            }

            overlay.Show();
            scrollView.ScrollToTop();
        }

        /**
         * <summary>
         * Sets the theme.
         * </summary>
         * <param name="theme">The theme to use</param>
         */
        private void SetTheme(Theme theme) {
            if (theme == null) {
                return;
            }

            overlay.SetTheme(theme);

            background.SetColor(theme.background);
            topBar.SetColor(theme.accent);
        }

        /**
         * <summary>
         * Switches to another view.
         * </summary>
         * <param name="view">The mod to switch to</param>
         */
        private void SwitchView(ConfigBuilder view) {
            modList.Hide();

            if (currentView != null) {
                currentView.Hide();
            }

            currentView = view;
            SetTheme(currentView.modInfo.theme);
            currentView.Show();
        }

        /**
         * <summary>
         * Builds the mod list view.
         * </summary>
         */
        internal void BuildModList() {
            foreach (KeyValuePair<BaseUnityPlugin, ModInfo> entry in ModManager.mods) {
                ModInfo modInfo = entry.Value;

                Area listing = new Area();
                listing.SetContentLayout(LayoutType.Horizontal);
                listing.SetSize(800f, 40f);

                Label modName = new Label(modInfo.name, 25);
                modName.SetSize(350f, 40f);
                listing.Add(modName);

                Area buttonArea = new Area();
                buttonArea.SetSize(350f, 40f);
                listing.Add(buttonArea);

                UIButton editButton = new UIButton("Edit", 25);
                editButton.SetSize(150f, 40f);
                editButton.onClick.AddListener(() => {
                    modInfo.Build(this);
                    SwitchView(modInfo.builder);
                });
                buttonArea.Add(editButton);

                modList.Add(listing);
            }
        }
    }
}
