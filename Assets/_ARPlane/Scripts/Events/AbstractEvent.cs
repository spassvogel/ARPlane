namespace UniversoAumentado.ARCraft.Events {

    public abstract class AbstractEvent {

        public bool IsCancelled { get; private set; }

        protected AbstractEvent() {
            IsCancelled = false;
        }

        public void StopPropagation() {
            IsCancelled = true;
        }

        public virtual AbstractEvent Clone() {
            throw new System.NotImplementedException();
        }

        public T Cast<T>() where T : AbstractEvent {
            return this as T;
        }
    }
}