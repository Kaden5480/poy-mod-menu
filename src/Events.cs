using UnityEngine.Events;

namespace ModMenu.Events {
    /**
     * <summary>
     * An event which passes a <see cref="ModView"/>
     * to listeners when a config UI is being built.
     * </summary>
     */
    public class BuildEvent : UnityEvent<ModView> {}

    /**
     * <summary>
     * An event which passes a value to listeners.
     * </summary>
     */
    public class ValueEvent<T> : UnityEvent<T> {}
}
