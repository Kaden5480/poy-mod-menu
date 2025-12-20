using System.Collections.Generic;

using UILib;
using UILib.Components;
using UILib.Layouts;
using UIButton = UILib.Components.Button;
using UnityEngine;

namespace ModMenu.Views {
    /**
     * <summary>
     * The base class for a view in Mod Menu's UI.
     * </summary>
     */
    public class View {
        private static Theme defaultTheme = new Theme();

        internal Logger logger { get; private set; }

        // The theme for this view
        internal Theme theme;

        // The root of the view
        internal Area root { get; private set; }

        // The info group (including the button)
        internal Area infoGroup { get; private set; }

        // The info scroll view
        private AccentScroll infoScroll;

        // The area to add info onto (within the scroll view)
        internal Area info { get; private set; }

        // The sections under this view
        private Dictionary<string, Section> sections;

        /**
         * <summary>
         * Initializes a view.
         * </summary>
         */
        internal View() {
            logger = new Logger(GetType());
            theme = defaultTheme;
            sections = new Dictionary<string, Section>();
        }

        /**
         * <summary>
         * Adds an entry under a given section.
         * </summary>
         * <param name="string">The name of the section to add under</param>
         * <param name="entry">The entry to add</param>
         */
        internal void Add(string section, Entry entry) {
            if (sections.ContainsKey(section) == false) {
                sections[section] = new Section(section);
            }

            sections[section].Add(entry);
        }

        /**
         * <summary>
         * Adds a UIComponent under a given category.
         * </summary>
         * <param name="string">The name of the category to add under</param>
         * <param name="component">The component to add</param>
         */
        public void Add(string category, UIComponent component) {
            Add(category, new Entry(component));
        }

        /**
         * <summary>
         * Builds the info group.
         * </summary>
         */
        private void BuildInfoGroup() {
            infoGroup = new Area();
            infoGroup.SetAnchor(AnchorType.TopRight);
            infoGroup.SetOffset(-40f, -20f);
            infoGroup.SetContentLayout(LayoutType.Horizontal);
            infoGroup.SetElementAlignment(TextAnchor.UpperRight);
            infoGroup.SetElementSpacing(20f);

            infoScroll = new AccentScroll();
            infoScroll.SetSize(500f, 800f);
            infoScroll.Hide();
            infoGroup.Add(infoScroll);

            // The button for toggling the info scroll on/off
            UIButton infoButton = new UIButton("i", 30);
            infoButton.SetSize(40f, 40f);
            infoButton.onClick.AddListener(
                infoScroll.ToggleVisibility
            );
            infoGroup.Add(infoButton);

            // This is the area where the info will go
            info = new Area();
            info.SetAnchor(AnchorType.TopMiddle);
            info.SetContentLayout(LayoutType.Vertical);
            info.SetContentPadding(20, 20, 20, 20);
            info.SetElementAlignment(TextAnchor.UpperCenter);
            info.SetElementSpacing(10);
            infoScroll.Add(info);
        }

        /**
         * <summary>
         * Builds all sections.
         * </summary>
         */
        protected void BuildSections() {
            foreach (Section section in sections.Values) {
                root.Add(section.Build());
            }
        }

        /**
         * <summary>
         * Builds the view's basic UI layout.
         * </summary>
         */
        protected void BuildBase() {
            if (root != null) {
                return;
            }

            root = new Area();
            root.SetAnchor(AnchorType.TopMiddle);
            root.SetContentLayout(LayoutType.Vertical);
            root.SetContentPadding(top: 40, bottom: 40);
            root.SetElementSpacing(40);

            // Build the info scroll view and button
            BuildInfoGroup();

            // The attaching below is done manually to
            // prevent unnecessary recursion when setting themes

            // Attach the root to the scroll view
            root.gameObject.transform.SetParent(
                Plugin.ui.scrollView.scrollContent.gameObject.transform, false
            );

            // Attach the info group to the scroll view directly
            infoGroup.gameObject.transform.SetParent(
                Plugin.ui.scrollView.gameObject.transform, false
            );
        }

        /**
         * <summary>
         * Builds everything for the UI.
         * </summary>
         */
        internal virtual void BuildAll() {
            BuildBase();
            BuildSections();
        }

        /**
         * <summary>
         * Shows the built UI.
         * </summary>
         */
        internal void Show() {
            root.Show();
            infoGroup.Show();

            // Check if auto-show is enabled
            if (Plugin.config.autoShowInfo.Value == true) {
                infoScroll.Show();
            }
        }

        /**
         * <summary>
         * Hides the UI.
         * </summary>
         */
        internal void Hide() {
            root.Hide();
            infoGroup.Hide();
        }
    }
}
