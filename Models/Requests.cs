namespace PerformanceIssues.Models
{
    public class CacheEntryRequest
    {
        public int SizeMB { get; set; }

        // Note: Ensure that objects created related to cache management are disposed properly after use.
        // For example, if using MemoryCache or similar, wrap it in a "using" block or call Dispose().
    }

    public class CPUTaskRequest
    {
        public int Complexity { get; set; }

        // Note: If CPU task management involves temporary memory buffers (like byte arrays), 
        // make sure to allocate these buffers within the scope of a method or "using" pattern to avoid memory leaks.
    }

    public class DataGenerationRequest
    {
        public int RecordCount { get; set; }

        // Note: If data generation involves resources like streams, database connections, or HTTP requests,
        // ensure you properly use "using" blocks or explicitly dispose these resources to prevent memory leaks.
    }
}