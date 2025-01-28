namespace PerformanceIssues.Models
{
    public class CacheEntryRequest
    {
        public int SizeMB { get; set; }
        
        // Assuming there are resources that need disposing
        // Implement IDisposable if necessary and use proper disposal patterns
    }

    public class CPUTaskRequest
    {
        public int Complexity { get; set; }
        
        // Assuming there are resources that need disposing
        // Implement IDisposable if necessary and use proper disposal patterns
    }

    public class DataGenerationRequest
    {
        public int RecordCount { get; set; }
        
        // Assuming there are resources that need disposing
        // Implement IDisposable if necessary and use proper disposal patterns
    }
}