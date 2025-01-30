namespace PerformanceIssues.Services
{
    // Interface declaration for ILeakyCache
    public interface ILeakyCache
    {
        Task<string> AddToCache(string key, int sizeInMb);
        int GetCacheSize();
    }

    // Implementation of the ILeakyCache interface
    public class LeakyCache : ILeakyCache
    {
        private readonly Dictionary<string, byte[]> _cache = new Dictionary<string, byte[]>();
        private readonly HttpClient _httpClient;
        private readonly ILogger<LeakyCache> _logger;

        // Constructor to initialize dependencies
        public LeakyCache(HttpClient httpClient, ILogger<LeakyCache> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Adds an item to the cache with the given key and size in MB
        public async Task<string> AddToCache(string key, int sizeInMb)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));
            }
            if (sizeInMb <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sizeInMb), "Size in MB should be greater than 0.");
            }

            try
            {
                // Logging scope creation and disposing it to avoid retention
                using (_logger.BeginScope(new Dictionary<string, string> { ["CacheKey"] = key }))
                {
                    _logger.LogInformation($"Adding {key} to cache with size {sizeInMb} MB.");

                    // Allocate large byte array
                    byte[] data = new byte[sizeInMb * 1024 * 1024];

                    // Fetch additional data for processing if needed
                    // Fix: Ensure proper disposal of HttpResponseMessage to prevent connection leaks
                    using (var response = await _httpClient.GetAsync("https://api.example.com/data"))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            _logger.LogError($"Failed to fetch additional data for key {key}. Status code: {response.StatusCode}");
                            return null;
                        }

                        var additionalData = await response.Content.ReadAsByteArrayAsync();

                        // Fix: Clear the allocated byte array before using it to avoid unnecessary memory retention
                        Array.Clear(additionalData, 0, additionalData.Length);
                    }

                    // Add to cache
                    _cache[key] = data;

                    // Fix: Clear the byte array after storing it in cache to avoid memory duplication
                    Array.Clear(data, 0, data.Length);

                    _logger.LogInformation($"Added {key} to cache.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while adding {key} to cache.");
                throw; // Ensure exceptions are propagated correctly
            }

            return key;
        }

        // Returns the current size of the cache
        public int GetCacheSize()
        {
            // Calculate total size in MB for all cached entries
            return _cache.Values.Sum(data => (data?.Length ?? 0) / (1024 * 1024));
        }

        // Properly dispose of resources to release memory and connections
        public void Dispose()
        {
            // Dispose of HttpClient to release underlying resources
            if (_httpClient != null)
            {
                _httpClient.Dispose();
            }

            // Fix: Clear cache and release all byte arrays to reclaim heap space
            foreach (var key in _cache.Keys.ToList())
            {
                byte[] data = _cache[key];
                Array.Clear(data, 0, data.Length);
                _cache.Remove(key);
            }

            // No explicit disposal necessary for logger
        }
    }
}