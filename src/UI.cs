using System.Collections.Generic;

using BepInEx;
using UILib;
using UILib.Components;
using UILib.Layouts;
using UIButton = UILib.Components.Button;
using UnityEngine;
using UnityEngine.EventSystems;

using ModMenu.Views;

namespace ModMenu {
    internal class UI {
        private Logger logger = new Logger(typeof(UI));

        internal Overlay overlay { get; private set; }
        internal ScrollView scrollView { get; private set; }

        private Image background;
        private Image topBar;
        private Image controls;

        private TextField searchBox;

        // The mod list view (main view)
        private ModListView modListView;

        // The current view
        private View currentView;

        /**
         * <summary>
         * Initializes Mod Menu's UI.
         * </summary>
         */
        internal UI() {
            overlay = new Overlay(0f, 0f);
            overlay.SetFill(FillType.All);
            overlay.SetSortable(false);

            // Lock vanilla inputs
            overlay.SetInputLock(true);

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

            // Build the scroll view which will display views
            scrollView = new ScrollView();
            scrollView.SetFill(FillType.All);
            scrollView.SetSize(0f, -200f);
            overlay.Add(scrollView);

            // Set the theme
            SetTheme(overlay.theme);

            // Handle going back
            backButton.onClick.AddListener(GoBack);

            // Register global keybind for going back
            Shortcut backShortcut = new Shortcut(new[] { KeyCode.Escape });
            backShortcut.onTrigger.AddListener(GoBack);
            overlay.AddShortcut(backShortcut);

            // Register local shortcut for searching
            Shortcut searchShortcut = new Shortcut(new[] { KeyCode.LeftControl, KeyCode.F });
            searchShortcut.onTrigger.AddListener(() => {
                EventSystem.current.SetSelectedGameObject(searchBox.gameObject);
            });
            overlay.AddShortcut(searchShortcut);

            // Register global keybind for toggling
            Shortcut toggleShortcut = new Shortcut(
                new[] { Plugin.config.toggleKeybind }
            );
            toggleShortcut.onTrigger.AddListener(() => {
                ToggleVisibility();
            });
            UIRoot.AddShortcut(toggleShortcut);
        }

        /**
         * <summary>
         * Builds the search bar.
         * </summary>
         */
        private void BuildSearch() {
            Area searchArea = new Area();
            searchArea.SetAnchor(AnchorType.TopRight);
            searchArea.SetOffset(-40f, -20f);
            searchArea.SetSize(300f, 40f);
            searchArea.SetContentLayout(LayoutType.Horizontal);
            searchArea.SetElementSpacing(20f);

            searchBox = new TextField("Search", 20);
            searchBox.SetSize(200f, 40f);
            searchBox.SetClearMode(TextField.ClearMode.Escape);
            searchBox.SetSubmitMode(TextField.SubmitMode.Click);
            searchBox.onInputChanged.AddListener((string query) => {
                if (Plugin.config.autoSearch.Value == false) {
                    return;
                }

                if (currentView != null) {
                    currentView.Search(query.ToLower());
                }

            });
            searchBox.onValidSubmit.AddListener((string query) => {
                if (currentView != null) {
                    currentView.Search(query.ToLower());
                }
            });
            searchBox.onCancel.AddListener(() => {
                if (currentView != null) {
                    currentView.Search("");
                }
            });
            searchArea.Add(searchBox);

            UIButton searchButton = new UIButton(Resources.searchGlass);
            searchButton.image.SetSize(-15f, -15f);
            searchButton.SetSize(40f, 40f);
            searchButton.onClick.AddListener(() => {});
            searchArea.Add(searchButton);

            scrollView.AddDirect(searchArea);
        }

        /**
         * <summary>
         * Builds the mod list view.
         * </summary>
         */
        internal void BuildModList() {
            // Also build the search
            BuildSearch();

            modListView = new ModListView();
            modListView.BuildAll();

            // Add manually to prevent theme recursion
            modListView.root.gameObject.transform.SetParent(
                scrollView.scrollContent.gameObject.transform, false
            );

            SwitchView(modListView);
        }

#region Navigation

        /**
         * <summary>
         * Switches to another view.
         * </summary>
         * <param name="view">The mod to switch to</param>
         */
        internal void SwitchView(View view) {
            if (currentView != null) {
                currentView.Hide();
            }

            currentView = view;
            scrollView.SetContent(currentView.root);
            SetTheme(currentView.theme);

            searchBox.SetValue("");
            currentView.Search("");

            currentView.Show();
        }

        /**
         * <summary>
         * Logic for back button/keybind.
         * </summary>
         */
        private void GoBack() {
            // Do nothing if not visible
            if (overlay.isVisible == false) {
                return;
            }

            // If on mod list view, close
            if (currentView == modListView) {
                overlay.Hide();
                return;
            }

            // Otherwise, switch back to the mod list view
            SwitchView(modListView);
        }

        /**
         * <summary>
         * Shows the UI.
         * </summary>
         */
        internal void Show() {
            if (currentView != null) {
                currentView.Update();
            }

            // Only scroll to top on first open
            if (modListView == null ) {
                BuildModList();
                overlay.Show();
                scrollView.ScrollToTop();
            }
            else {
                overlay.Show();
            }
        }

        /**
         * <summary>
         * Toggles the visibility of the overlay properly.
         * </summary>
         */
        private void ToggleVisibility() {
            if (overlay.isVisible == false) {
                Show();
            }
            else {
                overlay.Hide();
            }
        }

#endregion

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
    }
}
