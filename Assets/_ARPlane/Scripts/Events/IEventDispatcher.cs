namespace UniversoAumentado.ARPlane.Events
{
    /// <summary>
    /// Delegate called when an event is dispatched by the EventDispatcher
    /// </summary>
    /// <param name="evt">The target event</param>
    public delegate void EventListenerDelegate(AbstractEvent evt);

    /// <summary>
    /// Defines methods for adding or removing event listeners, checks whether specific types of event listeners are registered, and dispatches events.
    /// 
    /// In general, the easiest way for a user-defined class to gain event dispatching capabilities is to extend EventDispatcher. If this is impossible (that is, if the class is already extending another class), you can instead implement the IEventDispatcher interface, create an EventDispatcher member, and write simple hooks to route calls into the aggregated EventDispatcher.
    /// </summary>
    public interface IEventDispatcher
    {
        void Destroy();

        /// <summary>
        /// Register the target event listener delegate with the given type event in the type parameter. It fails if the listener has already been registered to this event type.
        /// </summary>
        /// <typeparam name="T">The target event type that inherits AbstractEvent</typeparam>
        /// <param name="evtListener">The event listener</param>
        /// <returns>True if listener is successfully added. False, otherwise.</returns>
        bool AddEventListener<T>(EventListenerDelegate evtListener) where T : AbstractEvent;

        /// <summary>
        /// Remove event listener from target event type.
        /// </summary>
        /// <typeparam name="T">The target event type that inherits AbstractEvent</typeparam>
        /// <param name="evtListener">The event listener</param>
        /// <returns>True if listener is found and removed. False, otherwise.</returns>
        bool RemoveEventListener<T>(EventListenerDelegate evtListener) where T : AbstractEvent;

        /// <summary>
        /// Remove all event listeners registered to the given event type in the type parameter
        /// </summary>
        /// <typeparam name="T">The target event type that inherits AbstractEvent</typeparam>
        /// <returns>True if event entry exists. False, otherwise.</returns>
        bool RemoveAllEventListeners<T>() where T : AbstractEvent;

        /// <summary>
        /// Dispatches an event to its registered listeners
        /// </summary>
        /// <param name="evt">The target event</param>
        void DispatchEvent(AbstractEvent evt);

        /// <summary>
        /// Checks whether the EventDispatcher object has any listeners registered for a specific type of event
        /// </summary>
        /// <typeparam name="T">The type of event</typeparam>
        /// <returns>True if a listener of the specified type is registered. False, otherwise</returns>
        bool HasEventListener<T>() where T : AbstractEvent;
    }
}
