using UnityEngine.Events;

namespace ModMenu {
    /**
     * <summary>
     * An event which passes a <see cref="ConfigBuilder"/>
     * to listeners when a config UI is being built.
     * </summary>
     */
    public class BuildEvent : UnityEvent<ConfigBuilder> {}

    /**
     * <summary>
     * An event which passes a value to listeners.
     * </summary>
     */
    public class ValueEvent<T> : UnityEvent<T> {}
}
