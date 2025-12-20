using System.Collections.Generic;

using UILib;
using UILib.Components;
using UILib.Layouts;
using UIButton = UILib.Components.Button;
using UnityEngine;

namespace ModMenu.Views {
    public class View {
        internal Logger logger;

        // The root of the view
        internal Area root { get; private set; }

        // The info group (including the button)
        private Area infoGroup;

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

            UIButton infoButton = new UIButton("i", 30);
            infoButton.SetSize(40f, 40f);
            infoButton.onClick.AddListener(
                infoScroll.ToggleVisibility
            );
            infoGroup.Add(infoButton);

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
        internal void BuildBase() {
            if (root != null) {
                return;
            }

            root = new Area();
            root.SetAnchor(AnchorType.TopMiddle);
            root.SetContentLayout(LayoutType.Vertical);
            root.SetContentPadding(top: 40, bottom: 40);
            root.SetElementSpacing(40);

            BuildInfoGroup();

            // The attaching below is done manually to
            // prevent unnecessary recursion when setting themes

            // Attach the root to the scroll view
            //root.gameObject.transform.SetParent(
            //    ui.scrollView.scrollContent.gameObject.transform, false
            //);

            //// Attach the info to the scroll view directly
            //infoArea.gameObject.transform.SetParent(
            //    ui.scrollView.gameObject.transform, false
            //);
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
