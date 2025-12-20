using System.Collections;

using UILib;
using UILib.Components;
using UILib.Layouts;
using UILib.Notifications;
using UnityEngine;
using UnityEngine.Networking;

using ModMenu.Views;

namespace ModMenu {
    public class ModView : View {
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
         * Builds a single info entry.
         * </summary>
         * <param name="title">The title of the entry</param>
         * <param name="value">The value of the entry</param>
         * <returns>The entry which was built</returns>
         */
        private Area BuildInfoEntry(string title, string value) {
            Area area = new Area();
            area.SetContentLayout(LayoutType.Horizontal);
            area.SetElementSpacing(10);
            area.SetFill(FillType.All);

            Label titleLabel = new Label(title, 25);
            titleLabel.SetSize(100f, 30f);
            area.Add(titleLabel);

            SmallLabel valueLabel = new SmallLabel(value, 25);
            valueLabel.SetSize(100f, 30f);
            area.Add(valueLabel);

            return area;
        }

        /**
         * <summary>
         * Builds the mod info tab.
         * </summary>
         */
        private void BuildModInfo() {
            Label title = new Label(modInfo.name, 35);
            title.SetSize(340f, 40f);
            info.Add(title);

            if (modInfo.thumbnail != null
                || modInfo.thumbnailUrl != null
            ) {
                thumbnail = new Image();
                thumbnail.SetSize(300f, 300f);
                thumbnail.Hide();
                info.Add(thumbnail);
            }

            if (modInfo.thumbnail != null) {
                thumbnail.SetTexture(modInfo.thumbnail);
            }
            else if (modInfo.thumbnailUrl != null) {
                DownloadThumbnail(modInfo.thumbnailUrl);
            }

            Area detailSpacing = new Area();
            detailSpacing.SetSize(0f, 10f);
            info.Add(detailSpacing);

            info.Add(BuildInfoEntry("Version", modInfo.version.ToString()));

            if (modInfo.license != null) {
                info.Add(BuildInfoEntry("License", modInfo.license));
            }

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
            BuildBase();

            root.gameObject.name = $"{modInfo.name} Root";

            if (header != null) {
                root.Add(header);
            }

            Label titleLabel = new Label(
                $"{modInfo.name} ({modInfo.version.ToString()})", 35
            );
            titleLabel.SetSize(0f, 40f);
            titleLabel.SetFill(FillType.Horizontal);
            root.Add(titleLabel);

            BuildSections();

            if (footer != null) {
                root.Add(footer);
            }

            root.SetTheme(modInfo.theme);

            BuildModInfo();
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

            if (request.isNetworkError == true
                || request.isHttpError == true
            ) {
                Notifier.Notify("Mod Menu", "Failed downloading mod's thumbnail");
                yield break;
            }

            if (thumbnail == null) {
                yield break;
            }

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
            if (Plugin.config.enableThumbnailDownloads.Value == false) {
                return;
            }

            Plugin.instance.StartCoroutine(
                DownloadThumbnailRoutine(url)
            );
        }

    }
}
