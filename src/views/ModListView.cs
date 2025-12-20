using System.Collections.Generic;

using UILib.Components;
using UILib.Layouts;
using UIButton = UILib.Components.Button;
using UnityEngine;

using ModMenu.Views;

namespace ModMenu {
    /**
     * <summary>
     * A view for the main mod list.
     * </summary>
     */
    internal class ModListView : View {
        /**
         * <summary>
         * Initializes the mod list view.
         * </summary>
         */
        internal ModListView() {}

        /**
         * <summary>
         * Builds the mod list view.
         * </summary>
         */
        internal override void BuildAll() {
            BuildBase();

            root.SetContentPadding(20, 20, 50, 50);
            root.SetElementSpacing(20);

            Label title = new Label("Installed Mods", 45);
            title.SetSize(0f, 40f);
            title.SetFill(FillType.Horizontal);
            root.Add(title);

            Area spacing = new Area();
            spacing.SetSize(0f, 10f);
            root.Add(spacing);

            BuildListings();
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
                Label name = new Label($"{modInfo.name} ({modInfo.version})", 25);
                name.SetAlignment(TextAnchor.MiddleRight);
                name.SetSize(350f, 40f);
                listing.Add(name);

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
                });
                buttonArea.Add(edit);

                root.Add(listing);
            }
        }
    }
}
