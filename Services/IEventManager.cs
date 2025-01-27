namespace PerformanceIssues.Services
{
    public interface IEventManager : IDisposable
    {
        void Subscribe(Action<string> handler);
        void RaiseEvent(string message);
        void Unsubscribe(Action<string> handler);
    }
}

namespace PerformanceIssues.Services
{
    public class EventManager : IEventManager
    {
        private event Action<string> OnEvent;

        public void Subscribe(Action<string> handler)
        {
            if (handler != null) OnEvent += handler;
        }

        public void RaiseEvent(string message)
        {
            OnEvent?.Invoke(message);
        }

        public void Unsubscribe(Action<string> handler)
        {
            if (handler != null) OnEvent -= handler;
        }

        public void Dispose()
        {
            ClearEventHandlers();
        }

        private void ClearEventHandlers()
        {
            if (OnEvent == null) return;
            foreach (var d in OnEvent.GetInvocationList())
            {
                OnEvent -= (Action<string>)d;
            }
        }
    }
}