using System.Collections.Generic;

using BepInEx;
using UILib;
using UILib.Behaviours;
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

        internal Overlay overlay { get; private set; }
        internal ScrollView scrollView { get; private set; }

        private Image background;
        private Image topBar;
        private Image controls;

        private ConfigBuilder currentView;

        /**
         * <summary>
         * Initializes Mod Menu's UI.
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

            // Bottom controls
            controls = new Image();
            controls.SetAnchor(AnchorType.BottomMiddle);
            controls.SetFill(FillType.Horizontal);
            controls.SetSize(0f, 100f);
            overlay.Add(controls);

            UIButton backButton = new UIButton("Done", 30);
            backButton.SetSize(150f, 40f);
            controls.Add(backButton);

            Label topBarTitle = new Label("Mod Menu", 55);
            topBarTitle.SetFill(FillType.Vertical);
            topBarTitle.SetSize(300f, 0f);
            topBar.Add(topBarTitle);

            // Mod list
            scrollView = new ScrollView();
            scrollView.SetFill(FillType.All);
            // Leave room for topbar
            scrollView.SetSize(0f, -200f);
            overlay.Add(scrollView);

            modList = new Area();
            modList.SetFill(FillType.All);
            modList.SetAnchor(AnchorType.TopMiddle);
            modList.SetContentLayout(LayoutType.Vertical);
            modList.SetContentPadding(20, 20, 50, 50);
            modList.SetElementSpacing(20);

            // Add manually to prevent theme recursion
            modList.gameObject.transform.SetParent(
                scrollView.scrollContent.gameObject.transform, false
            );
            scrollView.SetContent(modList);

            Label modTitle = new Label("Installed Mods", 45);
            modTitle.SetSize(0f, 40f);
            modTitle.SetFill(FillType.Horizontal);
            modList.Add(modTitle);

            Area space = new Area();
            space.SetSize(0f, 10f);
            modList.Add(space);

            SetTheme(overlay.theme);

            // Handle going back
            backButton.onClick.AddListener(GoBack);

            // Register global keybind for going back
            Shortcut shortcut = UIRoot.AddShortcut(new[] { KeyCode.Escape });
            shortcut.onTrigger.AddListener(GoBack);
        }

        /**
         * <summary>
         * Logic for back button/keybind.
         * </summary>
         */
        internal void GoBack() {
            // Do nothing if not visible
            if (overlay.isVisible == false) {
                return;
            }

            // If there is no view, already on mod list
            // so just quit
            if (currentView == null) {
                overlay.Hide();
                return;
            }

            // Otherwise, switch back to the mod list view
            currentView.Hide();
            currentView = null;

            SetTheme(modList.theme);
            scrollView.SetContent(modList);
            modList.Show();
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

                overlay.Show();
                scrollView.ScrollToTop();
            }
            else {
                overlay.Show();
            }

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
            controls.SetColor(theme.accent);
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
            scrollView.SetContent(currentView.root);
            SetTheme(currentView.modInfo.theme);
            currentView.Show();
        }

        /**
         * <summary>
         * Builds the mod list view.
         * </summary>
         */
        internal void BuildModList() {
            ModManager.Sort();

            foreach (KeyValuePair<BaseUnityPlugin, ModInfo> entry in ModManager.mods) {
                ModInfo modInfo = entry.Value;

                Area listing = new Area();
                listing.SetContentLayout(LayoutType.Horizontal);
                listing.SetElementSpacing(70);
                listing.SetFill(FillType.All);

                Label modName = new Label($"{modInfo.name} ({modInfo.version})", 25);
                modName.SetAlignment(TextAnchor.MiddleRight);
                modName.SetSize(350f, 40f);
                listing.Add(modName);

                Area buttonArea = new Area();
                buttonArea.SetSize(350f, 40f);
                listing.Add(buttonArea);

                UIButton editButton = new UIButton("Edit", 25);
                editButton.SetAnchor(AnchorType.MiddleLeft);
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
