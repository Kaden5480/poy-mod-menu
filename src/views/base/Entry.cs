using UILib;
using UILib.Components;
using UILib.Layouts;
using UIButton = UILib.Components.Button;
using UILib.Notifications;
using UnityEngine;

using ModMenu.Config;
using ModMenu.Parsing;

namespace ModMenu.Views {
    /**
     * <summary>
     * A single entry under a section.
     * </summary>
     */
    internal class Entry {
        internal BaseField field { get; private set; }
        internal UIComponent component { get; private set; }
        internal MetaData metaData { get; private set; }

        /**
         * <summary>
         * Creates an entry using the provided field.
         * </summary>
         * <param name="field>The field to build the component with</param>
         */
        internal Entry(BaseField field) {
            this.field = field;
            metaData = new MetaData(field);
            component = BuildInputArea(field.name, BuildField(field));
        }

        /**
         * <summary>
         * Creates an entry using the provided component.
         * </summary>
         * <param name="name">The name of the component</param>
         * <param name="component">The component to use</param>
         * <param name="metaData">The search metadata</param>
         */
        internal Entry(string name, UIComponent component, MetaData metaData) {
            this.metaData = metaData;
            this.component = BuildInputArea(name, component);
        }

        /**
         * <summary>
         * Creates an entry using the provided component.
         * </summary>
         * <param name="component">The component to use</param>
         * <param name="metaData">The search metadata</param>
         */
        internal Entry(UIComponent component, MetaData metaData) {
            this.metaData = metaData;
            this.component = component;
        }

        /**
         * <summary>
         * Applies a search query to this entry.
         * </summary>
         * <param name="query">The search query</param>
         */
        internal void Search(string query) {
            if (metaData == null) {
                if (string.IsNullOrEmpty(query) == false) {
                    component.Hide();
                }
                else {
                    component.Show();
                }
                return;
            }

            if (metaData.Matches(query) == false) {
                component.Hide();
            }
            else {
                component.Show();
            }
        }

#region BaseField Components

        private const float compWidth = 200f;
        private const float compHeight = 40f;
        private const int compFontSize = 20;

        /**
         * <summary>
         * Sends a notification.
         * </summary>
         * <param name="message">The message to display</param>
         */
        private void Notify(string message) {
            Notifier.Notify(
                field.modInfo.name,
                message,
                theme: field.modInfo.theme
            );
        }

        /**
         * <summary>
         * Try setting a value for a field, notifying on errors.
         * </summary>
         * <param name="field">The field to try setting for</param>
         * <param name="value">The value to try setting</param>
         * <returns>Whether setting was successful</returns>
         */
        private bool SetFieldValue(BaseField field, object value) {
            string error = field.CheckPredicates(value);

            if (error != null) {
                Notify(error);
                return false;
            }

            field.SetValue(value);
            return true;
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
         * Builds a `Toggle` component.
         * </summary>
         * <param name="field">The field to build for</param>
         * <returns>The component</returns>
         */
        private Toggle BuildToggle(BaseField field) {
            Toggle toggle = new Toggle((bool) field.value);
            toggle.SetSize(compHeight, compHeight);
            toggle.onValueChanged.AddListener((bool value) => {
                SetFieldValue(field, value);
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
            colorField.SetSize(compHeight, compHeight);
            colorField.onValueChanged.AddListener((Color color) => {
                SetFieldValue(field, color);
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
                (KeyCode) field.value, compFontSize
            );
            keyCodeField.SetSize(compWidth, compHeight);
            keyCodeField.onValueChanged.AddListener((KeyCode keyCode) => {
                SetFieldValue(field, keyCode);
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
            area.SetElementSpacing(0.1f*compWidth);
            area.SetSize(0f, compHeight);

            TextField textField = BuildText(field);
            textField.SetSize(0.3f*compWidth, 0.8f*compHeight);

            Slider slider = new Slider((float) field.min, (float) field.max);
            slider.SetValue((float) field.value);
            slider.SetSize(0.6f*compWidth, 10f);
            slider.onSubmit.AddListener((float value) => {
                if (SetFieldValue(field, value) == true) {
                    textField.SetValue(field.ToString());
                }
            });
            slider.onValueChanged.AddListener((float value) => {
                textField.SetValue(value.ToString());
            });

            field.onValueChanged.AddListener((object value) => {
                slider.SetValue((float) value);
            });
            area.Add(slider);

            textField.onValidSubmit.AddListener(delegate {
                slider.SetValue((float) field.value);
            });
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
            Label label = new Label(field.ToString(), compFontSize);
            label.SetSize(compWidth, compHeight);

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
                TypeChecks.TypeToString(field.type), compFontSize
            );
            textField.SetValue(field.ToString());
            textField.SetSize(compWidth, compHeight);
            textField.SetPredicate((string value) => {
                // Try parsing the value
                if (TypeChecks.TryParse(
                    field.type, value, out object result
                ) == false) {
                    Notify(ShowInputError(field));
                    return false;
                }

                // Validate min/max if defined
                if (field.min != null || field.max != null) {
                    if (TypeChecks.InLimits(result, field.min, field.max) == false) {
                        Notify(ShowInputError(field));
                        return false;
                    }
                }

                // Try setting the value
                return SetFieldValue(field, result);
            });

            field.onValueChanged.AddListener((object value) => {
                textField.SetValue(field.ToString());
            });

            return textField;
        }

        /**
         * <summary>
         * Builds a component using this entry's field.
         * </summary>
         * <returns>The component</returns>
         */
        private UIComponent BuildField(BaseField field) {
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
                case FieldType.ReadOnly:
                    return BuildReadOnly(field);
                default:
                    field.LogError($"Unexpected field type `{field.fieldType}`");
                    return null;
            }
        }

#endregion

        /**
         * <summary>
         * Build an area with a label and a given component.
         * </summary>
         * <param name="name">The name to label this component with</param>
         * <param name="component">The component to build an area for</param>
         * <returns>The component</returns>
         */
        private Area BuildInputArea(string name, UIComponent component) {
            component.SetOffset(-150f, 0f);

            Area area = new Area();
            area.SetSize(-1f, 60f);
            area.SetFill(FillType.Horizontal);
            area.SetContentLayout(LayoutType.Horizontal);
            area.SetElementSpacing(70);

            // The title for the input
            Area labelArea = new Area();
            labelArea.SetSize(500f, 60f);
            labelArea.SetContentLayout(LayoutType.Vertical);
            labelArea.SetElementAlignment(AnchorType.MiddleRight);
            area.Add(labelArea);

            SmallLabel label = new SmallLabel(name, 25);
            label.text.verticalOverflow = VerticalWrapMode.Overflow;
            label.SetFill(FillType.All);
            if (metaData != null) {
                label.SetTooltip(metaData.description);
            }
            labelArea.Add(label);

            // Add the input itself
            Area controlArea = new Area();
            controlArea.SetSize(500f, 60f);
            controlArea.Add(component);
            area.Add(controlArea);

            return area;
        }
    }
}
