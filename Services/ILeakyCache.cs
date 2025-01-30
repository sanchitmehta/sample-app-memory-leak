namespace PerformanceIssues.Services
{
    // Interface definition for ILeakyCache service
    public interface ILeakyCache
    {
        // Adds data to the cache with the specified key and size
        // Ensure proper handling of memory allocation and disposal
        Task<string> AddToCache(string key, int sizeInMb);
        
        // Returns the total size of the cache
        int GetCacheSize();
    }
}