using System.Collections.Generic;

using UILib.Components;
using UILib.Layouts;
using UIButton = UILib.Components.Button;

using ModMenu.Views;

namespace ModMenu {
    /**
     * <summary>
     * A view for the main mod list.
     * </summary>
     */
    internal class ModListView : View {
        // Cached count label
        private SmallLabel cachedLabel;

        /**
         * <summary>
         * Initializes the mod list view.
         * </summary>
         */
        internal ModListView() {}

        /**
         * <summary>
         * Builds the info group for displaying
         * current statistics.
         * </summary>
         */
        private void BuildStats() {
            // Make sure the info group exists
            BuildInfoGroup();

            // Add the title
            Label title = new Label("Statistics", 35);
            title.SetSize(340f, 40f);
            info.Add(title);

            info.Add(new Area(0f, 10f));

            info.Add(BuildInfoEntry("Installed Mods", ModManager.mods.Count.ToString()));
            info.Add(BuildInfoEntry(
                "Cached Mods", $"{ModView.cachedViews}",
                out Label cachedTitle, out cachedLabel
            ));

            cachedTitle.SetTooltip("The number of mod page UIs currently cached.");

            info.Add(new Area(0f, 10f));

            UIButton clearButton = new UIButton("Clear Cache", 25);
            clearButton.SetTooltip("Clears all currently built mod page UIs.");
            clearButton.SetSize(200f, 40f);
            clearButton.onClick.AddListener(() => {
                ClearCache();
            });
            info.Add(clearButton);

        }

        /**
         * <summary>
         * Builds the mod list view.
         * </summary>
         */
        internal override void BuildAll() {
            BuildBase();

            root.SetContentPadding(20, 50);
            root.SetElementSpacing(20);

            Label title = new Label("Installed Mods", 45);
            title.SetSize(0f, 40f);
            title.SetFill(FillType.Horizontal);
            root.Add(title);

            Area spacing = new Area();
            spacing.SetSize(0f, 10f);
            root.Add(spacing);

            BuildListings();

            BuildStats();
        }

        /**
         * <summary>
         * Builds the mod listings.
         * </summary>
         * <param name="mods">The mods to build listings for</param>
         */
        internal void BuildListings() {
            // Sort mods first
            ModManager.Sort();

            foreach (ModInfo modInfo in ModManager.mods.Values) {
                Area listing = new Area();
                listing.SetContentLayout(LayoutType.Horizontal);
                listing.SetElementSpacing(70);
                listing.SetFill(FillType.All);

                // The name of the mod
                Area nameArea = new Area(350f, 40f);
                nameArea.SetContentLayout(LayoutType.Vertical);
                nameArea.SetElementAlignment(AnchorType.MiddleRight);
                listing.Add(nameArea);

                Label name = new Label($"{modInfo.name} ({modInfo.version})", 25);
                name.SetFill(FillType.All);
                name.SetTooltip(modInfo.description);
                nameArea.Add(name);

                // The mod's edit button
                Area buttonArea = new Area();
                buttonArea.SetSize(350f, 40f);
                listing.Add(buttonArea);

                UIButton edit = new UIButton("Edit", 25);
                edit.SetAnchor(AnchorType.MiddleLeft);
                edit.SetSize(150f, 40f);
                edit.onClick.AddListener(() => {
                    modInfo.Build(Plugin.ui);
                    Plugin.ui.SwitchView(modInfo.view);
                    cachedLabel.SetText($"{ModView.cachedViews}");
                });
                buttonArea.Add(edit);

                root.Add(listing);

                // Add the listing for searching
                AddCustom(listing, new MetaData(modInfo));
            }
        }

        /**
         * <summary>
         * Clears the mod view cache.
         * </summary>
         */
        private void ClearCache() {
            foreach (ModInfo info in ModManager.mods.Values) {
                info.Destroy();
            }

            cachedLabel.SetText($"{ModView.cachedViews}");
        }
    }
}
