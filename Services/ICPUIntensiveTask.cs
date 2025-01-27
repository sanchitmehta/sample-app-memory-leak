using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Net;
using System.Net.Http;
using System.Text;

namespace PerformanceIssues.Services
{
    public interface ICPUIntensiveTask : IDisposable
    {
        void Start();
        void Stop();
    }

    public class CPUIntensiveTask : ICPUIntensiveTask
    {
        private HttpClient _httpClient;
        private List<byte[]> _byteArrays;
        private StringBuilder _logs;
        private Pipe _pipe;
        private List<CancellationTokenSource> _cancellationTokens;

        public CPUIntensiveTask()
        {
            _httpClient = new HttpClient();
            _byteArrays = new List<byte[]>();
            _logs = new StringBuilder();
            _pipe = new Pipe();
            _cancellationTokens = new List<CancellationTokenSource>();
        }

        public void Start()
        {
            // Simulate adding byte arrays
            _byteArrays.Add(new byte[1024 * 1024]);

            // Simulate some logging
            _logs.AppendLine("Task started");

            // Simulate network operation
            _httpClient.GetAsync("https://example.com").GetAwaiter().GetResult();

            // Simulate pipeline usage
            _pipe.Writer.WriteAsync(new ReadOnlyMemory<byte>(new byte[1024])).GetAwaiter().GetResult();

            // Simulate using cancellation tokens
            var cts = new CancellationTokenSource();
            _cancellationTokens.Add(cts);
        }

        public void Stop()
        {
            // Clear log and byte array collections
            _logs.Clear();
            _byteArrays.Clear();

            // Cancel and dispose of all CancellationTokenSource instances
            foreach (var cts in _cancellationTokens)
            {
                cts.Cancel();
                cts.Dispose();
            }
            _cancellationTokens.Clear();

            // Complete the pipe writer to free resources
            _pipe.Writer.Complete();
        }

        public void Dispose()
        {
            Stop();
            _httpClient.Dispose();
            _pipe.Reader.Complete();
            _pipe.Dispose();
        }
    }
}