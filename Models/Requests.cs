using System;
using System.Threading;
using System.IO;
using Microsoft.Extensions.Logging;

namespace PerformanceIssues.Models
{
    public class CacheEntryRequest : IDisposable
    {
        public int SizeMB { get; set; }

        public void Dispose()
        {
            // Implement appropriate dispose logic if necessary
        }
    }

    public class CPUTaskRequest : IDisposable
    {
        public int Complexity { get; set; }

        public void Dispose()
        {
            // Implement appropriate dispose logic if necessary
        }
    }

    public class DataGenerationRequest : IDisposable
    {
        public int RecordCount { get; set; }

        public void Dispose()
        {
            // Implement appropriate dispose logic if necessary
        }
    }

    public class ResourceConsumer
    {
        private readonly ILogger<ResourceConsumer> _logger;
        private CancellationTokenSource _cts;
        private MemoryStream _memoryStream;
        private Stream _dataStream;

        public ResourceConsumer(ILogger<ResourceConsumer> logger)
        {
            _logger = logger;
        }

        public void StartNewTask()
        {
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            _memoryStream?.Dispose();
            _memoryStream = new MemoryStream();
            // Simulate other resources initialization and disposal
        }

        public void PerformIOOperations()
        {
            using (_dataStream = new FileStream("dummyfile.txt", FileMode.OpenOrCreate))
            {
                // Perform actual stream operations
            }
        }

        public void LogAndConsumeResources(string message)
        {
            using var scope = _logger.BeginScope("Processing request");
            _logger.LogInformation(message);
            // Simulate other resource-consuming activities
        }

        public void Cleanup()
        {
            _cts?.Dispose();
            _memoryStream?.Dispose();
            _dataStream?.Dispose();
        }
    }
}