namespace PerformanceIssues.Services
{
    public interface ICPUIntensiveTask : IDisposable
    {
        void Start();
        void Stop();
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PerformanceIssues.Services
{
    public class CPUIntensiveTask : ICPUIntensiveTask
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly List<byte[]> _dataBuffers = new List<byte[]>();
        private readonly StringBuilder _logBuilder = new StringBuilder();
        private CancellationTokenSource _cancellationTokenSource;
        private bool _disposed;

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => PerformTask(_cancellationTokenSource.Token));
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
        }

        private async Task PerformTask(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var data = await _httpClient.GetByteArrayAsync("http://example.com");
                    _dataBuffers.Add(data);

                    _logBuilder.AppendLine("Data fetched at " + DateTime.Now);
                    PerformCPUIntensiveOperation(data);

                    if (_dataBuffers.Count > 100) // Example condition to clear memory
                    {
                        _dataBuffers.Clear();
                        _logBuilder.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                _logBuilder.AppendLine("Exception: " + ex.Message);
            }
        }

        private void PerformCPUIntensiveOperation(byte[] data)
        {
            // Simulate CPU intensive operation
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(data[i] ^ 0xAA);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _httpClient.Dispose();
                _cancellationTokenSource?.Dispose();
                _dataBuffers.Clear();
                _logBuilder.Clear();
            }

            _disposed = true;
        }
    }
}