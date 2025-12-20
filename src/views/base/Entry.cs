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
        //private MetaData metaData;

        /**
         * <summary>
         * Creates an entry using the provided field.
         * </summary>
         * <param name="field>The field to build the component with</param>
         */
        internal Entry(BaseField field) {
            this.field = field;
            component = BuildComponent(field);
        }

        /**
         * <summary>
         * Creates an entry using the provided component.
         * </summary>
         * <param name="component">The component to use</param>
         */
        internal Entry(UIComponent component) {
            this.component = component;
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
            textField.SetSize(0.3f*compWidth, compHeight);

            Slider slider = new Slider((float) field.min, (float) field.max);
            slider.SetValue((float) field.value);
            slider.SetSize(0.6f*compWidth, 10f);
            slider.onValueChanged.AddListener((float value) => {
                if (SetFieldValue(field, value) == true) {
                    textField.SetValue(field.ToString());
                }
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
        private UIComponent BuildComponent(BaseField field) {
            UIComponent input;

            switch (field.fieldType) {
                case FieldType.Toggle:
                    input = BuildToggle(field);
                    break;
                case FieldType.Color:
                    input = BuildColor(field);
                    break;
                case FieldType.KeyCode:
                    input = BuildKeyCode(field);
                    break;
                case FieldType.Slider:
                    input = BuildSlider(field);
                    break;
                case FieldType.Text:
                    input = BuildText(field);
                    break;
                case FieldType.ReadOnly:
                    input = BuildReadOnly(field);
                    break;
                default:
                    Plugin.LogError($"{field.name}: Unexpected field type `{field.fieldType}`");
                    return null;
            }

            input.SetOffset(-50f, 0f);

            Area area = new Area();
            area.SetSize(-1f, 60f);
            area.SetFill(FillType.Horizontal);
            area.SetContentLayout(LayoutType.Horizontal);
            area.SetElementSpacing(70);

            SmallLabel label = new SmallLabel(field.name, 25);
            label.SetSize(300f, 60f);
            label.SetAlignment(TextAnchor.MiddleRight);
            area.Add(label);

            Area controlArea = new Area();
            controlArea.SetSize(300f, 60f);
            controlArea.Add(input);
            area.Add(controlArea);

            return area;
        }
    }

#endregion

}
