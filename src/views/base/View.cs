using System.Collections.Generic;

using UILib;
using UILib.Components;
using UILib.Layouts;
using UIButton = UILib.Components.Button;

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

        // Extra custom entries under this view
        // These must be built manually, they're just here
        // for searching purposes
        private List<Entry> customEntries;

        /**
         * <summary>
         * Initializes a view.
         * </summary>
         */
        internal View() {
            logger = new Logger(GetType());
            theme = defaultTheme;
            sections = new Dictionary<string, Section>();
            customEntries = new List<Entry>();
        }

        /**
         * <summary>
         * Applies a search to this view, enabling/disabling
         * content.
         * </summary>
         * <param name="query">The search query</param>
         */
        internal void Search(string query) {
            foreach (Section section in sections.Values) {
                section.Search(query);
            }

            foreach (Entry entry in customEntries) {
                entry.Search(query);
            }
        }

        /**
         * <summary>
         * Adds a custom entry.
         * </summary>
         * <param name="entry">The entry to add</param>
         */
        internal void AddCustom(Entry entry) {
            customEntries.Add(entry);
        }

        /**
         * <summary>
         * Adds a UIComponent as a custom entry.
         * </summary>
         * <param name="entry">The entry to add</param>
         * <param name="metaData">Optional metdata to provide (for searching)</param>
         */
        internal void AddCustom(UIComponent component, MetaData metaData = null) {
            AddCustom(new Entry(component, metaData));
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
         * <param name="category">The name of the category to add under</param>
         * <param name="component">The component to add</param>
         * <param name="metaData">Optional metadata to provide (for searching)</param>
         */
        public void Add(string category, UIComponent component, MetaData metaData = null) {
            Add(category, new Entry(component, metaData));
        }

        /**
         * <summary>
         * Adds a UIComponent under a given category with a specified name.
         *
         * This will display the provided UIComponent with a label, like other inputs
         * which Mod Menu generates.
         * </summary>
         * <param name="category">The name of the category to add under</param>
         * <param name="name">The name to display in the label</param>
         * <param name="component">The component to add</param>
         * <param name="metaData">Optional metadata to provide (for searching)</param>
         */
        public void Add(string category, string name, UIComponent component, MetaData metaData = null) {
            if (metaData == null) {
                metaData = new MetaData();
            }

            metaData.Add(name);

            Add(category, new Entry(name, component, metaData));
        }

        /**
         * <summary>
         * Builds the info group.
         * </summary>
         */
        protected void BuildInfoGroup() {
            infoGroup = new Area();
            infoGroup.SetAnchor(AnchorType.TopRight);
            infoGroup.SetOffset(-40f, -80f);
            infoGroup.SetContentLayout(LayoutType.Horizontal);
            infoGroup.SetElementAlignment(AnchorType.TopRight);
            infoGroup.SetElementSpacing(20f);

            infoScroll = new AccentScroll();
            infoScroll.SetSize(500f, 720f);
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
            info.SetContentPadding(20);
            info.SetElementAlignment(AnchorType.TopMiddle);
            info.SetElementSpacing(10);
            infoScroll.Add(info);

            // Attach the info group to the main UI scroll view directly
            infoGroup.gameObject.transform.SetParent(
                Plugin.ui.scrollView.gameObject.transform, false
            );
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

            // Attach the root to the main UI scroll view
            root.gameObject.transform.SetParent(
                Plugin.ui.scrollView.scrollContent.gameObject.transform, false
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
         * Runs some extra code just before this view is shown.
         * </summary>
         */
        internal virtual void Update() {}

        /**
         * <summary>
         * Shows the built UI.
         * </summary>
         */
        internal void Show() {
            Update();

            root.Show();

            if (infoGroup == null) {
                return;
            }

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

            if (infoGroup != null) {
                infoGroup.Hide();
            }
        }
    }
}
