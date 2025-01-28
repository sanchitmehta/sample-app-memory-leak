namespace PerformanceIssues.Models
{
    public class CacheEntryRequest : IDisposable
    {
        public int SizeMB { get; set; }

        public void Dispose()
        {
            // Cleanup code here
        }
    }

    public class CPUTaskRequest : IDisposable
    {
        public int Complexity { get; set; }

        public void Dispose()
        {
            // Cleanup code here
        }
    }

    public class DataGenerationRequest : IDisposable
    {
        public int RecordCount { get; set; }

        public void Dispose()
        {
            // Cleanup code here
        }
    }

    // Assuming additional classes/scenarios where we clear large collections or disposable objects
    public class ResourceHandler : IDisposable
    {
        private List<byte[]> _byteDataList = new List<byte[]>();
        private List<string> _stringDataList = new List<string>();
        private List<object> _genericObjectsList = new List<object>();
        private List<Http1Connection> _httpConnectionsList = new List<Http1Connection>();
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Pipe _pipe;

        public void ClearResources()
        {
            _byteDataList.Clear();
            _stringDataList.Clear();
            _genericObjectsList.Clear();

            foreach (var connection in _httpConnectionsList)
            {
                connection.Dispose();
            }
            _httpConnectionsList.Clear();

            _cancellationTokenSource.Dispose();
            _pipe?.Reader.Complete();
            _pipe?.Writer.Complete();
        }

        public void Dispose()
        {
            ClearResources();

            _cancellationTokenSource?.Dispose();
            _pipe?.Reader.Complete();
            _pipe?.Writer.Complete();
        }
    }
}