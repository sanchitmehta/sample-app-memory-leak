using System;
using System.IO; // For IDisposable related functionality such as Streams
using System.Net.Http; // To manage any disposable Http-related resources
using System.Buffers; // For efficient buffer reuse in memory

namespace PerformanceIssues.Models
{
    public class CacheEntryRequest
    {
        public int SizeMB { get; set; }
    }

    public class CPUTaskRequest
    {
        public int Complexity { get; set; }
    }

    public class DataGenerationRequest
    {
        public int RecordCount { get; set; }
        
        // If large amounts of memory might be used during operations with DataGenerationRequest,
        // consider explicitly managing buffer-related logic.
        private byte[] _temporaryBuffer;

        public void AllocateMemory(int bufferSize)
        {
            // Example of proper memory handling: avoid long-lived allocations and ensure cleanup
            if (_temporaryBuffer != null)
            {
                // If there is already a buffer allocated, release it first
                _temporaryBuffer = null;
            }

            _temporaryBuffer = ArrayPool<byte>.Shared.Rent(bufferSize);
        }

        public void DisposeMemory()
        {
            if (_temporaryBuffer != null)
            {
                // Properly release and return the buffer to the pool to avoid excessive memory usage
                ArrayPool<byte>.Shared.Return(_temporaryBuffer, clearArray: true);
                _temporaryBuffer = null;
            }
        }
    }

    public class Http1ConnectionManager : IDisposable
    {
        private HttpClient _httpClient;
        private MemoryStream _httpStream;

        public Http1ConnectionManager()
        {
            // Preventing resource leaks by ensuring HttpClient is properly managed
            _httpClient = new HttpClient();
            _httpStream = new MemoryStream();
        }

        public void MakeHttpRequest(Uri uri)
        {
            // Using HttpClient in an efficient way to avoid leaking connections
            using var response = _httpClient.GetAsync(uri).Result;
            using var contentStream = response.Content.ReadAsStreamAsync().Result;

            // Example: copy response stream into a MemoryStream for local usage
            contentStream.CopyTo(_httpStream);
        }

        public void Dispose()
        {
            // Ensure proper disposal of disposable resources
            _httpClient?.Dispose();
            _httpStream?.Dispose();
        }
    }

    public class StringLogger
    {
        private StringBuilder _logBuilder;
        private const int StringBuilderCapacityLimit = 10000; // Example limit to avoid unbounded growth

        public StringLogger()
        {
            // Avoid long-living strings by managing them explicitly with StringBuilder
            _logBuilder = new StringBuilder();
        }

        public void Log(string message)
        {
            _logBuilder.Append(message);

            // Clear the log periodically to avoid unbounded growth
            if (_logBuilder.Length > StringBuilderCapacityLimit)
            {
                _logBuilder.Clear();
            }
        }

        public string RetrieveLog()
        {
            return _logBuilder.ToString();
        }
    }
}


