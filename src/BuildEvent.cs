using UnityEngine.Events;

namespace ModMenu {
    /**
     * <summary>
     * An event which passes a <see cref="ConfigBuilder"/>
     * to listeners when a config UI is being built.
     * </summary>
     */
    public class BuildEvent : UnityEvent<ConfigBuilder> {}
}
