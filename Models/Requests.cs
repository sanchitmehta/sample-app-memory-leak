using System;
using System.Net.Http;

namespace PerformanceIssues.Models
{
    public class CacheEntryRequest
    {
        public int SizeMB { get; set; }

        // Ensure to clean up any associated byte arrays or resources explicitly in the calling code.
        // For example, if data is loaded into a byte array for cache operations, null the reference
        // or clear the buffer where applicable to help GC reclaim memory sooner.
    }

    public class CPUTaskRequest
    {
        public int Complexity { get; set; }

        // If performing operations involving temporary resources, ensure proper disposal.
        // For example, dispose of IDisposable objects correctly if used during complex tasks.
    }

    public class DataGenerationRequest : IDisposable
    {
        public int RecordCount { get; set; }
        private HttpClient _httpClient; // Assuming HTTP client might be used here for generating data.
        
        public DataGenerationRequest()
        {
            // Initialize the HttpClient only when required, or pass it via DI if reusable.
            _httpClient = new HttpClient();
        }

        // Example of ensuring proper cleanup with IDisposable implementation.
        public void Dispose()
        {
            // Dispose the HttpClient to release underlying resources.
            if (_httpClient != null)
            {
                _httpClient.Dispose();
                _httpClient = null; // Clear reference to help GC.
            }
        }

        // Ensures HTTP connections are disposed properly within this class.
        public void FetchDataFromApi(string apiUrl)
        {
            if (string.IsNullOrEmpty(apiUrl))
                throw new ArgumentException("API URL cannot be null or empty.", nameof(apiUrl));

            // Using statement for HttpRequestMessage ensures proper disposal.
            using (var request = new HttpRequestMessage(HttpMethod.Get, apiUrl))
            {
                var response = _httpClient.SendAsync(request).Result; // Synchronous call for simplicity; consider async alternative.

                if (response.Content != null)
                {
                    // Dispose content explicitly after processing to avoid retention.
                    using (var content = response.Content)
                    {
                        var payload = content.ReadAsByteArrayAsync().Result;

                        // Ensure the byte array reference is cleared explicitly after use.
                        payload = null;
                    }
                }
            }
        }
    }

    // General Note:
    // - For logging frameworks, ensure to dispose of IDisposable logging scopes created via BeginScope.
    // - Use async tokens carefully; avoid long retention by ensuring cancellation tokens are scoped properly.
    // - Always prefer "using" statements or "using declarations" where supported to manage IDisposable resources automatically.
}