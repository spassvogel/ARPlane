using System.Collections.Generic;
using System;

namespace UniversoAumentado.ARPlane.Events
{

    /// <summary>
    /// Base class for all classes that dispatch events.
    /// </summary>
    public class EventDispatcher : IEventDispatcher
    {
        private Dictionary<Type, List<EventListenerDelegate>> _eventListeners;

        public EventDispatcher()
        {
            _eventListeners = new Dictionary<Type, List<EventListenerDelegate>>();
        }

        public virtual void Destroy()
        {
            // clear every list of listeners for each event type
            foreach(KeyValuePair<Type, List<EventListenerDelegate>> pair in _eventListeners)
            {
                pair.Value.Clear();
            }
            // clear map of listeners
            _eventListeners.Clear();
            _eventListeners = null;
        }

        public bool AddEventListener<T>(EventListenerDelegate evtListener) where T : AbstractEvent
        {
            // get listeners list, if it doesn't exist yet create a new entry
            List<EventListenerDelegate> listeners = null;
            if (!_eventListeners.TryGetValue(typeof(T), out listeners))
            {
                listeners = new List<EventListenerDelegate>();
                _eventListeners.Add(typeof(T), listeners);
            }
            // add listener delegate to event's listeners list if it is not already added
            if (!listeners.Contains(evtListener))
            {
                listeners.Add(evtListener);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveEventListener<T>(EventListenerDelegate evtListener) where T : AbstractEvent
        {
            List<EventListenerDelegate> listeners = null;
            if (_eventListeners.TryGetValue(typeof(T), out listeners))
            {
                return listeners.Remove(evtListener);
            }
            else
            {
                return false;
            }
        }

        public bool RemoveAllEventListeners<T>() where T : AbstractEvent
        {
            List<EventListenerDelegate> listeners = null;
            if (_eventListeners.TryGetValue(typeof(T), out listeners))
            {
                // note that event entry is not removed from the Dictionary in order to reduce amount of work for the Garbage Collector
                // therefore, we only clear the list of event listeners
                listeners.Clear();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void DispatchEvent(AbstractEvent evt)
        {
            List<EventListenerDelegate> listeners = null;
            if (_eventListeners.TryGetValue(evt.GetType(), out listeners) && listeners.Count > 0)
            {
                // create a clone in order to traverse and allow listener removals by one of the invoked listeners
                EventListenerDelegate[] listenersArr = listeners.ToArray();
                // traverse listeners and invoke them, if event has not been cancelled
                for (int i = 0, length = listenersArr.Length; i < length && !evt.IsCancelled; i++)
                {
                    listenersArr[i].Invoke(evt);
                }
            }
        }

        public bool HasEventListener<T>() where T : AbstractEvent
        {
            List<EventListenerDelegate> listeners = null;
            return _eventListeners.TryGetValue(typeof(T), out listeners) && listeners.Count > 0;
        }
    }
}