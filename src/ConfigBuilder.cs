using System.Collections.Generic;

using UILib;
using UILib.Components;
using UILib.Layouts;
using UnityEngine;

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
        private ModInfo modInfo;

        // Categorised UI components which will be
        // placed under their `string` categories in the UI
        private Dictionary<string, List<UIComponent>> categories;

        // The root of the generated UI
        internal Area root;

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

            return keyCodeField;
        }

        /**
         * <summary>
         * Builds a `Slider` component.
         * </summary>
         * <param name="field">The field to build for</param>
         * <returns>The component</returns>
         */
        private Slider BuildSlider(BaseField field) {
            Slider slider = new Slider((float) field.min, (float) field.max);
            slider.SetSize(200f, 10f);
            slider.onValueChanged.AddListener((float value) => {
                field.SetValue(value);
            });

            return slider;
        }

        /**
         * <summary>
         * Builds a `Label` component.
         * </summary>
         * <param name="field">The field to build for</param>
         * <returns>The component</returns>
         */
        private Label BuildReadOnly(BaseField field) {
            string display = (field.value == null)
                ? "" : field.value.ToString();

            Label label = new Label(display, 20);
            label.SetSize(200f, 40f);

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
            TextField textField = new TextField(field.value.ToString(), 20);
            textField.SetSize(200f, 40f);
            textField.SetPredicate((string value) => {
                // Try parsing the value
                if (TypeChecks.TryParse(
                    field.type, value, out object result
                ) == false) {
                    return false;
                }

                // If no min/max defined, no more validation is needed
                if (field.min == null || field.max == null) {
                    field.SetValue(result);
                    return true;
                }

                // Otherwise validate min and max
                if (TypeChecks.InLimits(result, field.min, field.max) == false) {
                    return false;
                }

                field.SetValue(result);
                return true;
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
         * Completes building the UI.
         * </summary>
         */
        internal void Build() {
            root = new Area();
            root.gameObject.name = $"{modInfo.name} Root";
            GameObject.DontDestroyOnLoad(root.gameObject);
            root.SetContentLayout(LayoutType.Vertical);

            // TODO: Alphabetical order
            // Build all the categories
            foreach (KeyValuePair<string, List<UIComponent>> entry in categories) {
                Area area = new Area();
                area.SetContentLayout(LayoutType.Vertical);
                area.SetSize(600f, 0f);

                Label title = new Label(entry.Key, 50);
                title.SetSize(600f, 50f);
                area.Add(title);

                foreach (UIComponent component in entry.Value) {
                    area.Add(component);
                }

                root.Add(area);
            }
        }

#endregion

    }
}
