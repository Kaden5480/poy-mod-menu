using UILib;
using UILib.Components;
using UILib.Layouts;
using UnityEngine;

using UIButton = UILib.Components.Button;

namespace ModMenu {
    internal class UI {
        private Logger logger = new Logger(typeof(UI));

        private Overlay overlay;
        private ScrollView modList;

        /**
         * <summary>
         * Initializes mod menu's UI.
         * </summary>
         */
        internal UI() {
            Theme theme = new Theme();

            overlay = new Overlay(0f, 0f);
            overlay.SetFill(FillType.All);
            overlay.SetSortable(false);

            Image background = new Image(theme.background);
            background.SetFill(FillType.All);
            overlay.Add(background);

#region Top Bar

            Image topBar = new Image(theme.accent);
            topBar.SetAnchor(AnchorType.TopMiddle);
            topBar.SetFill(FillType.Horizontal);
            topBar.SetSize(0f, 100f);
            overlay.Add(topBar);

            Label topBarTitle = new Label("Mod Menu", 55);
            topBarTitle.SetFill(FillType.Vertical);
            topBarTitle.SetSize(300f, 0f);
            topBar.Add(topBarTitle);

#endregion

#region Main Content

            modList = new ScrollView();
            modList.SetContentLayout(LayoutType.Vertical);
            modList.SetContentPadding(20, 20, 60, 60);
            modList.SetElementSpacing(20);
            modList.SetFill(FillType.All);
            // Leave room for topbar
            modList.SetSize(0f, -100f);
            modList.SetOffset(0, -50f);
            overlay.Add(modList);

            Label modTitle = new Label("Installed Mods", 45);
            modTitle.SetSize(400f, 40f);
            modList.Add(modTitle);

            Area space = new Area();
            space.SetSize(0f, 20f);
            modList.Add(space);

#endregion

#region Mod Listing

            string[] modNames = new[] {
                "Bag With Friends",
                "Colorblind Holds",
                "Fast Coffee",
                "Fast Reset",
                "In Game Logs",
                "Mod Menu",
                "PeakSweeper",
                "Potato Mod",
                "Time Attack Resetter",
                "Tweaks of Yore",
                "UILib",
                "Unity Logger",
                "Velocity HUD",
                "Workshop ID Editor",
            };

            foreach (string name in modNames) {
                Area listing = new Area();
                listing.SetContentLayout(LayoutType.Horizontal);
                listing.SetSize(800f, 40f);
                modList.Add(listing);

                Label modName = new Label(name, 25);
                modName.SetSize(350f, 40f);
                listing.Add(modName);

                Area buttonArea = new Area();
                buttonArea.SetSize(350f, 40f);
                listing.Add(buttonArea);

                UIButton modConfig = new UIButton("Edit", 25);
                modConfig.SetSize(150f, 40f);
                buttonArea.Add(modConfig);
            }

#endregion

        }

        internal void Show() {
            overlay.Show();
            modList.ScrollToTop();
        }
    }
}
