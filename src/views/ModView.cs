using System.Collections;
using System.Collections.Generic;

using UILib;
using UILib.Components;
using UILib.Layouts;
using UILib.Notifications;
using UnityEngine;
using UnityEngine.Networking;

using ModMenu.Parsing;
using ModMenu.Views;

namespace ModMenu {
    /**
     * <summary>
     * Handles building and displaying a mod's config page.
     *
     * It's worth checking out <see cref="View"/> for the extra
     * functionality this class provides.
     * </summary>
     */
    public class ModView : View {
        // The number of cached views
        internal static int cachedViews = 0;

        // The mod info
        private ModInfo modInfo;

        // The header and footer
        private UIComponent header;
        private UIComponent footer;

        // The mod's thumbnail
        private Image thumbnail;

        /**
         * <summary>
         * Initializes a mod view.
         * </summary>
         * <param name="modInfo">The info of the mod being built for</param>
         */
        internal ModView(ModInfo modInfo) {
            this.modInfo = modInfo;
            theme = modInfo.theme;

            foreach (KeyValuePair<string, List<BaseField>> entry in modInfo.config) {
                string category = entry.Key;

                foreach (BaseField field in entry.Value) {
                    Add(category, new Entry(field));
                }
            }
        }

        /**
         * <summary>
         * Sets the header.
         * This will be placed above all categories, including the title of the mod.
         * </summary>
         * <param name="header">The header to use</param>
         */
        public void SetHeader(UIComponent header) {
            this.header = header;
        }

        /**
         * <summary>
         * Sets the footer.
         * This will be placed at the very bottom of the config, below all categories.
         * </summary>
         * <param name="footer">The footer to use</param>
         */
        public void SetFooter(UIComponent footer) {
            this.footer = footer;
        }

        /**
         * <summary>
         * Builds the mod info tab.
         * </summary>
         */
        private void BuildModInfo() {
            // Make sure the info group exists
            BuildInfoGroup();

            // Add the mod's name as a title
            Label title = new Label(modInfo.name, 35);
            title.SetSize(340f, 40f);
            info.Add(title);

            // If a thumbnail has been configured, get
            // an area ready to store it
            if (modInfo.thumbnail != null
                || modInfo.thumbnailUrl != null
            ) {
                thumbnail = new Image();
                thumbnail.SetSize(300f, 300f);
                thumbnail.Hide();
                info.Add(thumbnail);
            }

            // If a thumbnail has been configured, try using it
            // instead of downloading one
            if (modInfo.thumbnail != null) {
                thumbnail.SetTexture(modInfo.thumbnail);
            }
            // Otherwise, download it
            else if (modInfo.thumbnailUrl != null) {
                DownloadThumbnail(modInfo.thumbnailUrl);
            }

            Area detailSpacing = new Area();
            detailSpacing.SetSize(0f, 10f);
            info.Add(detailSpacing);

            // Add the license and version number
            info.Add(BuildInfoEntry("Version", modInfo.version.ToString()));

            if (modInfo.license != null) {
                info.Add(BuildInfoEntry("License", modInfo.license));
            }

            // Add some spacing and a description if one
            // was configured
            if (modInfo.description != null) {
                Area descSpacing = new Area();
                descSpacing.SetSize(0f, 10f);
                info.Add(descSpacing);

                SmallLabel description = new SmallLabel(modInfo.description, 25);
                description.SetSize(450f, 0f);
                description.SetFill(FillType.Vertical);
                info.Add(description);
            }
        }

        /**
         * <summary>
         * Builds the mod view's UI.
         * </summary>
         */
        internal override void BuildAll() {
            // Build the base layout to add components to
            BuildBase();

            root.gameObject.name = $"{modInfo.name} Root";

            // Custom header
            if (header != null) {
                root.Add(header);
            }

            // This mod's title
            Area titleArea = new Area();
            titleArea.SetFill(FillType.All);
            titleArea.SetContentLayout(LayoutType.Vertical);
            titleArea.SetElementSpacing(5f);

            Label titleLabel = new Label(
                $"{modInfo.name} ({modInfo.version.ToString()})", 35
            );
            titleLabel.SetSize(0f, 40f);
            titleLabel.SetFill(FillType.Horizontal);
            titleArea.Add(titleLabel);

            // Auto-generated label
            if (modInfo.generated == true) {
                SmallLabel generatedLabel = new SmallLabel("(auto-generated)", 16);
                generatedLabel.SetSize(0f, 20f);
                generatedLabel.SetFill(FillType.Horizontal);
                titleArea.Add(generatedLabel);
            }

            root.Add(titleArea);

            // Sections containing the fields for editing
            // along with any custom ones
            BuildSections();

            // Custom footer
            if (footer != null) {
                root.Add(footer);
            }

            // Also build the mod's info into
            // the scroll view on the side
            BuildModInfo();

            // Apply the mod's theme
            root.SetTheme(modInfo.theme);
            infoGroup.SetTheme(modInfo.theme);

            cachedViews++;
        }

        /**
         * <summary>
         * Coroutine for downloading the mod's thumbnail.
         * </summary>
         * <param name="url">The URL to download from</param>
         */
        private IEnumerator DownloadThumbnailRoutine(string url) {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();

            // Check for errors
            if (request.isNetworkError == true
                || request.isHttpError == true
            ) {
                Notifier.Notify($"{modInfo.name}", "Failed downloading thumbnail");
                yield break;
            }

            // If the thumbnail component doesn't exist, fail
            if (thumbnail == null) {
                yield break;
            }

            // Otherwise, update the texture and display it
            thumbnail.SetTexture((
                (DownloadHandlerTexture) request.downloadHandler
            ).texture);
            thumbnail.Show();
        }

        /**
         * <summary>
         * Downloads a texture from a given URL, setting the
         * mod's thumbnail once complete.
         * </summary>
         * <param name="url">The URL to download from</param>
         */
        private void DownloadThumbnail(string url) {
            // If thumbnail downloads were disabled, don't get anything
            if (Plugin.config.enableThumbnailDownloads.Value == false) {
                return;
            }

            Plugin.instance.StartCoroutine(
                DownloadThumbnailRoutine(url)
            );
        }

        /**
         * <summary>
         * Updates all fields being displayed in the mod page.
         * </summary>
         */
        internal override void Update() {
            foreach (List<BaseField> fields in modInfo.config.Values) {
                foreach (BaseField field in fields) {
                    field.Update();
                }
            }
        }

        /**
         * <summary>
         * Destroys this view.
         * </summary>
         */
        internal override void Destroy() {
            base.Destroy();
            cachedViews--;
        }
    }
}
