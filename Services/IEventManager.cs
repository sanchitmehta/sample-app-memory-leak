namespace PerformanceIssues.Services
{
    public interface IEventManager : IDisposable
    {
        void Subscribe(Action<string> handler);
        void RaiseEvent(string message);
    }
    
    public class EventManager : IEventManager
    {
        private readonly List<Action<string>> _handlers = new List<Action<string>>();

        public void Subscribe(Action<string> handler)
        {
            _handlers.Add(handler);
        }

        public void RaiseEvent(string message)
        {
            foreach (var handler in _handlers)
            {
                handler.Invoke(message);
            }
        }

        public void Dispose()
        {
            _handlers.Clear();
        }
    }
}