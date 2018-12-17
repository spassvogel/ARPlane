namespace UniversoAumentado.ARCraft.Events
{
    /// <summary>
    /// Singleton instance for the EventDispatcher - currently this solution is an anti-pattern that can be solved with for example Dependency Injection
    /// </summary>
    public class GlobalEventDispatcher : EventDispatcher
    {
        /// <summary>
        /// The <cref="EventDispatcher"/> Singleton instance
        /// </summary>
        public static EventDispatcher Instance { get { return _instance ?? (_instance = new EventDispatcher()); } }

        private static EventDispatcher _instance = null;
    }
}