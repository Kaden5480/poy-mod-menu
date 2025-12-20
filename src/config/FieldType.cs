namespace ModMenu.Config {
    /**
     * <summary>
     * The types of fields which Mod Menu can display.
     * </summary>
     */
    public enum FieldType {
        /**
         * <summary>
         * The default field type, lets Mod Menu
         * infer the most likely type.
         * </summary>
         */
        None,

        /**
         * <summary>
         * Displays a `Toggle` component.
         * </summary>
         */
        Toggle,

        /**
         * <summary>
         * Displays a `ColorField`.
         * </summary>
         */
        Color,

        /**
         * <summary>
         * Displays a `KeyCodeField`.
         * </summary>
         */
        KeyCode,

        /**
         * <summary>
         * Displays a `Slider`.
         * </summary>
         */
        Slider,

        /**
         * <summary>
         * Displays a `TextField`.
         * </summary>
         */
        Text,

        /**
         * <summary>
         * Displays a `Dropdown`.
         * </summary>
         */
        Dropdown,

        /**
         * <summary>
         * Displays the field's value as a `Label`.
         * </summary>
         */
        ReadOnly,
    }
}
