using System;
using System.IO;
using System.Net.Http;
using System.Text;

namespace PerformanceIssues.Models
{
    // Updated class definitions with comments for improved disposal and cleanup

    public class CacheEntryRequest : IDisposable
    {
        public int SizeMB { get; set; }

        // Implement IDisposable for better memory management
        public void Dispose()
        {
            // Add cleanup logic here if necessary in future
            GC.SuppressFinalize(this);
        }
    }

    public class CPUTaskRequest : IDisposable
    {
        public int Complexity { get; set; }

        // Implement IDisposable to follow a proper pattern
        public void Dispose()
        {
            // If any unmanaged resources are added later, clean up here
            GC.SuppressFinalize(this);
        }
    }

    public class DataGenerationRequest : IDisposable
    {
        public int RecordCount { get; set; }

        // Implement IDisposable for consistency and to prevent leaks
        public void Dispose()
        {
            // Placeholder for future resources
            GC.SuppressFinalize(this);
        }
    }

    // Helper methods for proper memory cleanup if HTTP-related tasks exist
    public class HttpHelper : IDisposable
    {
        private readonly HttpClient _httpClient;

        public HttpHelper()
        {
            _httpClient = new HttpClient();
        }

        public string GetData(string url)
        {
            // Ensure HTTP request objects are managed properly
            using (var response = _httpClient.GetAsync(url).GetAwaiter().GetResult())
            {
                if (response.IsSuccessStatusCode)
                {
                    using (var content = response.Content)
                    {
                        return content.ReadAsStringAsync().GetAwaiter().GetResult();
                    }
                }

                return string.Empty;
            }
        }

        public void Dispose()
        {
            // Cleanup the HttpClient instance to ensure it doesn't cause memory leaks
            _httpClient.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    // Example logger scope class, addressed for memory leak prevention
    public class LoggerScope : IDisposable
    {
        private readonly StreamWriter _logger;

        public LoggerScope(string filePath)
        {
            // Encapsulate the lifetime of logging scope via proper disposable mechanisms
            _logger = new StreamWriter(filePath, append: true, encoding: Encoding.UTF8);
        }

        public void WriteLog(string message)
        {
            if (_logger == null) return;
            
            lock (_logger)
            {
                _logger.WriteLine(message);
                _logger.Flush();
            }
        }

        public void Dispose()
        {
            // Dispose the StreamWriter to release related unmanaged and memory buffers
            _logger.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}