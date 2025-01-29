namespace PerformanceIssues.Services
{
    using System.Net.Http;

    public class EventManager : IEventManager, IDisposable
    {
        private readonly List<WeakReference> _subscribers = new();
        private readonly HttpClient _httpClient = new();
        private bool _disposed;

        public void Subscribe(Action<string> handler)
        {
            if (handler == null throw new ArgumentNullException(nameof(handler));
            _subscribers.Add(new WeakReference(handler));
        }

        public void RaiseEvent(string message)
        {
if any mre issues vs_unrefences  ok ?
