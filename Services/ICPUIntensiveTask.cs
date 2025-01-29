namespace PerformanceIssues.Services
{
    public interface ICPUIntensiveTask : IDisposable
    {
        void Start();
        void Stop();
    }
}

namespace PerformanceIssues.Services
{
    public class CPUIntensiveTask : ICPUIntensiveTask
    {
        private bool _disposed = false;
        private HttpClient _httpClient;
        private List<byte[]> _largeByteArrays;
        private List<string> _retainedStrings;

        public CPUIntensiveTask()
        {
            _httpClient = new HttpClient();
            _largeByteArrays = new List<byte[]>();
            _retainedStrings = new List<string>();
        }

        public void Start()
        {
            // Simulating some operations leading to resource allocation.
            _largeByteArrays.Add(new byte[1024 * 1024 * 10]); // Allocate 10MB
            _retainedStrings.Add(new string('A', 1024 * 1024)); // Allocate 1MB string
        }

        public void Stop()
        {
            // Clear large collections to release their memory.
            _largeByteArrays.Clear();
            _retainedStrings.Clear();

            // Cancelling any pending HTTP operations if applicable.
            _httpClient.CancelPendingRequests();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    if (_httpClient != null)
                    {
                        _httpClient.Dispose();
                        _httpClient = null;
                    }

                    if (_largeByteArrays != null)
                    {
                        _largeByteArrays.Clear();
                        _largeByteArrays = null;
                    }

                    if (_retainedStrings != null)
                    {
                        _retainedStrings.Clear();
                        _retainedStrings = null;
                    }
                }
                
                // Cleanup unmanaged resources if any (none in this case)
                _disposed = true;
            }
        }

        ~CPUIntensiveTask()
        {
            Dispose(false);
        }
    }
}