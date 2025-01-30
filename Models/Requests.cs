namespace PerformanceIssues.Models
{
    public class CacheEntryRequest
    {
        public int SizeMB { get; set; }

        // If this class ever holds references to large objects or unmanaged resources in the future, 
        // consider implementing IDisposable to manage cleanup.
    }

    public class CPUTaskRequest
    {
        public int Complexity { get; set; }

        // If this class starts using resources like external handlers or data streams,
        // ensure proper disposal (using patterns or implementing IDisposable).
    }

    public class DataGenerationRequest
    {
        public int RecordCount { get; set; }

        // If this class develops fields requiring cleanup (e.g., DataTables, file streams), 
        // implement IDisposable.
    }
}