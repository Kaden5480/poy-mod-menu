using System.Collections;
using System.Collections.Generic;

using UILib;
using UILib.Components;
using UILib.Layouts;
using UILib.Notifications;
using UIButton = UILib.Components.Button;
using UnityEngine;
using UnityEngine.Networking;
using UEImage = UnityEngine.UI.Image;

using ModMenu.Config;
using ModMenu.Parsing;

namespace ModMenu {
    /**
     * <summary>
     * A class which handles building the UIs for each mod.
     * </summary>
     */
    public class ConfigBuilder {
        private Logger logger = new Logger(typeof(ConfigBuilder));
        internal ModInfo modInfo { get; private set; }

        // The root of the generated UI
        internal Area root { get; private set; }

        // The info area
        internal Area infoArea { get; private set; }

        // Categorised UI components which will be
        // placed under their `string` categories in the UI
        private Dictionary<string, List<UIComponent>> categories;

        // The header and footer
        private UIComponent header;
        private UIComponent footer;

        // The mod's thumbnail
        private Image thumbnail;

        /**
         * <summary>
         * Initializes a config builder.
         * </summary>
         * <param name="modInfo">The mod info this config builder is for</param>
         */
        internal ConfigBuilder(ModInfo modInfo) {
            this.modInfo = modInfo;
            categories = new Dictionary<string, List<UIComponent>>();

            Start();
        }

        /**
         * <summary>
         * Creates an error message for invalid values.
         * </summary>
         * <param name="field">The field to generate errors for</param>
         * <returns>The string error</returns>
         */
        private string ShowInputError(BaseField field) {
            string error = $"Expected a {TypeChecks.TypeToString(field.type).ToLower()}";

            if (field.min != null && field.max != null) {
                return $"{error} between {field.min} and {field.max}";
            }

            if (field.min != null) {
                return $"{error} that's at least {field.min}";
            }

            if (field.max != null) {
                return $"{error} that's at most {field.max}";
            }

            return error;
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
            Plugin.instance.StartCoroutine(
                DownloadThumbnailRoutine(url)
            );
        }

#region Building Specific Types

        /**
         * <summary>
         * Builds a `Toggle` component.
         * </summary>
         * <param name="field">The field to build for</param>
         * <returns>The component</returns>
         */
        private Toggle BuildToggle(BaseField field) {
            Toggle toggle = new Toggle((bool) field.value);
            toggle.SetSize(40f, 40f);
            toggle.onValueChanged.AddListener((bool value) => {
                field.SetValue(value);
            });

            field.onValueChanged.AddListener((object value) => {
                toggle.SetValue((bool) value);
            });

            return toggle;
        }

        /**
         * <summary>
         * Builds a `ColorField` component.
         * </summary>
         * <param name="field">The field to build for</param>
         * <returns>The component</returns>
         */
        private ColorField BuildColor(BaseField field) {
            ColorField colorField = new ColorField((Color) field.value);
            colorField.SetSize(40f, 40f);
            colorField.onValueChanged.AddListener((Color color) => {
                field.SetValue(color);
            });

            field.onValueChanged.AddListener((object color) => {
                colorField.SetValue((Color) color);
            });

            return colorField;
        }

        /**
         * <summary>
         * Builds a `KeyCodeField` component.
         * </summary>
         * <param name="field">The field to build for</param>
         * <returns>The component</returns>
         */
        private KeyCodeField BuildKeyCode(BaseField field) {
            KeyCodeField keyCodeField = new KeyCodeField(
                (KeyCode) field.value, 20
            );
            keyCodeField.SetSize(200f, 40f);
            keyCodeField.onValueChanged.AddListener((KeyCode keyCode) => {
                field.SetValue(keyCode);
            });

            field.onValueChanged.AddListener((object keyCode) => {
                keyCodeField.SetValue((KeyCode) keyCode);
            });

            return keyCodeField;
        }

        /**
         * <summary>
         * Builds a `Slider` component.
         * </summary>
         * <param name="field">The field to build for</param>
         * <returns>The component</returns>
         */
        private Area BuildSlider(BaseField field) {
            Area area = new Area();
            area.SetContentLayout(LayoutType.Horizontal);
            area.SetElementSpacing(10);
            area.SetSize(200f, 30f);

            Slider slider = new Slider((float) field.min, (float) field.max);
            slider.SetSize(200f*0.6f, 10f);
            slider.onValueChanged.AddListener((float value) => {
                field.SetValue(value);
            });

            field.onValueChanged.AddListener((object value) => {
                slider.SetValue((float) value);
            });
            area.Add(slider);

            TextField textField = BuildText(field);
            textField.SetSize(200f*0.3f, 30f);
            area.Add(textField);

            return area;
        }

        /**
         * <summary>
         * Builds a `Label` component.
         * </summary>
         * <param name="field">The field to build for</param>
         * <returns>The component</returns>
         */
        private Label BuildReadOnly(BaseField field) {
            Label label = new Label(field.ToString(), 20);
            label.SetSize(200f, 40f);

            field.onValueChanged.AddListener((object value) => {
                label.SetText(field.ToString());
            });

            return label;
        }

        /**
         * <summary>
         * Builds a `TextField` component.
         * </summary>
         * <param name="field">The field to build for</param>
         * <returns>The component</returns>
         */
        private TextField BuildText(BaseField field) {
            TextField textField = new TextField(
                TypeChecks.TypeToString(field.type), 20
            );
            textField.SetValue(field.ToString());
            textField.SetSize(200f, 40f);
            textField.SetPredicate((string value) => {
                // Try parsing the value
                if (TypeChecks.TryParse(
                    field.type, value, out object result
                ) == false) {
                    Notifier.Notify("Mod Menu", ShowInputError(field));
                    return false;
                }

                // If no min/max defined, no more validation is needed
                if (field.min == null && field.max == null) {
                    field.SetValue(result);
                    return true;
                }

                // Otherwise validate min and max
                if (TypeChecks.InLimits(result, field.min, field.max) == false) {
                    Notifier.Notify("Mod Menu", ShowInputError(field));
                    return false;
                }

                field.SetValue(result);
                return true;
            });

            field.onValueChanged.AddListener((object value) => {
                textField.SetValue(field.ToString());
            });

            return textField;
        }

#endregion

#region Main Building Logic

        /**
         * <summary>
         * Builds a component for a given field.
         * </summary>
         * <param name="field">The field to build a component for</param>
         * <returns>The component</returns>
         */
        private UIComponent BuildComponent(BaseField field) {
            // TODO: Dropdowns can have custom display names

            switch (field.fieldType) {
                case FieldType.Toggle:
                    return BuildToggle(field);
                case FieldType.Color:
                    return BuildColor(field);
                case FieldType.KeyCode:
                    return BuildKeyCode(field);
                case FieldType.Slider:
                    return BuildSlider(field);
                case FieldType.Text:
                    return BuildText(field);
                //case FieldType.Dropdown:
                //    return BuildDropdown(field);
                case FieldType.ReadOnly:
                    return BuildReadOnly(field);
                default:
                    logger.LogError($"{field.name}: Unexpected field type `{field.fieldType}`");
                    return null;
            }
        }

        /**
         * <summary>
         * Builds components from a given list of fields.
         * </summary>
         * <param name="fields">The fields to build components for</param>
         * <returns>The components</returns>
         */
        private IEnumerable<UIComponent> BuildComponents(List<BaseField> fields) {
            foreach (BaseField field in fields) {
                UIComponent component = BuildComponent(field);
                if (component == null) {
                    continue;
                }

                Area area = new Area();
                area.SetContentLayout(LayoutType.Horizontal);
                area.SetSize(600f, 60f);

                Label label = new Label(field.name, 20);
                label.SetSize(300f, 60f);
                area.Add(label);

                Area controlArea = new Area();
                controlArea.SetSize(300f, 60f);
                controlArea.Add(component);
                area.Add(controlArea);

                yield return area;
            }
        }

        /**
         * <summary>
         * Starts building the preconfigured config into a UI.
         * </summary>
         */
        private void Start() {
            foreach (KeyValuePair<string, List<BaseField>> entry in modInfo.config) {
                foreach (UIComponent component in BuildComponents(entry.Value)) {
                    if (component == null) {
                        continue;
                    }

                    Add(entry.Key, component);
                }
            }
        }

        /**
         * <summary>
         * Places a custom UIComponent under a given category.
         * </summary>
         * <param name="category">The category to place the component under</param>
         * <param name="component">The component to place</param>
         */
        public void Add(string category, UIComponent component) {
            if (categories.ContainsKey(category) == false) {
                categories[category] = new List<UIComponent>();
            }

            categories[category].Add(component);
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

            Label titleLabel = new Label(title, 20);
            titleLabel.SetSize(100f, 30f);
            area.Add(titleLabel);

            Label valueLabel = new Label(value, 20);
            valueLabel.SetSize(100f, 30f);
            area.Add(valueLabel);

            area.SetTheme(modInfo.theme);
            valueLabel.SetColor(valueLabel.theme.selectAltNormal);

            return area;
        }

        /**
         * <summary>
         * Builds the mod info tab.
         * </summary>
         */
        private void BuildInfo() {
            infoArea = new Area();
            infoArea.SetAnchor(AnchorType.TopRight);
            infoArea.SetOffset(-40f, -20f);
            infoArea.SetContentLayout(LayoutType.Horizontal);
            infoArea.SetElementAlignment(TextAnchor.UpperRight);
            infoArea.SetElementSpacing(20f);
            infoArea.SetTheme(modInfo.theme);

            ScrollView infoScroll = new ScrollView();
            infoScroll.SetSize(500f, 800f);
            infoArea.Add(infoScroll, true);
            infoScroll.background.color = infoScroll.theme.accent;

            UIButton infoButton = new UIButton("i", 30);
            infoButton.SetSize(40f, 40f);
            infoButton.onClick.AddListener(infoScroll.ToggleVisibility);
            infoArea.Add(infoButton, true);

            Area area = new Area();
            area.SetAnchor(AnchorType.TopMiddle);
            area.SetContentLayout(LayoutType.Vertical);
            area.SetContentPadding(20, 20, 20, 20);
            area.SetElementAlignment(TextAnchor.UpperCenter);
            area.SetElementSpacing(10);
            infoScroll.Add(area, true);

            // Add the mod info
            Label title = new Label(modInfo.name, 35);
            title.SetSize(340f, 40f);
            area.Add(title, true);

            if (modInfo.thumbnail != null
                || modInfo.thumbnailUrl != null
            ) {
                thumbnail = new Image();
                thumbnail.SetSize(300f, 300f);
                thumbnail.Hide();
                area.Add(thumbnail, true);
            }

            if (modInfo.thumbnail != null) {
                thumbnail.SetTexture(modInfo.thumbnail);
            }
            else if (modInfo.thumbnailUrl != null) {
                DownloadThumbnail(modInfo.thumbnailUrl);
            }

            Area detailSpace = new Area();
            detailSpace.SetSize(0f, 10f);
            area.Add(detailSpace, true);

            area.Add(BuildInfoEntry("Version", modInfo.version.ToString()));

            if (modInfo.license != null) {
                area.Add(BuildInfoEntry("License", modInfo.license));
            }

            if (modInfo.description != null) {
                Area descSpace = new Area();
                descSpace.SetSize(0f, 10f);
                area.Add(descSpace, true);

                Label description = new Label(modInfo.description, 20);
                description.SetSize(450f, 0f);
                description.SetFill(FillType.Vertical);
                area.Add(description, true);

                description.SetColor(description.theme.selectAltNormal);
            }
        }

        /**
         * <summary>
         * Completes building the UI.
         * </summary>
         * <param name="ui">The UI to build onto</param>
         */
        internal void Build(UI ui) {
            root = new Area();
            root.gameObject.name = $"{modInfo.name} Root";
            root.SetAnchor(AnchorType.TopMiddle);
            root.SetContentLayout(LayoutType.Vertical);
            root.SetContentPadding(top: 40, bottom: 40);
            root.SetElementSpacing(40);
            root.SetTheme(modInfo.theme);

            if (header != null) {
                root.Add(header);
            }

            Label titleLabel = new Label(
                $"{modInfo.name} ({modInfo.version.ToString()})", 30
            );
            titleLabel.SetSize(0f, 40f);
            titleLabel.SetFill(FillType.Horizontal);
            root.Add(titleLabel, true);

            // TODO: Alphabetical order
            // Build all the categories
            foreach (KeyValuePair<string, List<UIComponent>> entry in categories) {
                Area area = new Area();
                area.SetContentLayout(LayoutType.Vertical);
                area.SetElementSpacing(10);
                area.SetFill(FillType.All);
                root.Add(area, true);

                Label title = new Label(entry.Key, 30);
                title.SetSize(600f, 50f);
                area.Add(title, true);

                foreach (UIComponent component in entry.Value) {
                    area.Add(component, true);
                }
            }

            if (footer != null) {
                root.Add(footer);
            }

            BuildInfo();

            // The attaching below is done manually to
            // prevent unnecessary recursion when setting themes

            // Attach the root to the scroll view
            root.gameObject.transform.SetParent(
                ui.scrollView.scrollContent.gameObject.transform, false
            );

            // Attach the info to the scroll view directly
            infoArea.gameObject.transform.SetParent(
                ui.scrollView.gameObject.transform, false
            );
        }

#endregion

        /**
         * <summary>
         * Shows the built UI.
         * </summary>
         */
        internal void Show() {
            root.Show();
            infoArea.Show();
        }

        /**
         * <summary>
         * Hides the built UI.
         * </summary>
         */
        internal void Hide() {
            root.Hide();
            infoArea.Hide();
        }

    }
}
