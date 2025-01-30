namespace PerformanceIssues.Services
{
    public class DataGenerator : IDisposable
    {
        private readonly List<object> _storedData = new();
        private readonly Random _random = new();
        private bool _disposed = false;

        // Ensure proper cleanup of memory
        public async Task GenerateAndStoreData(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var data = new
                {
                    Id = Guid.NewGuid(),
                    Name = GenerateRandomString(50),
                    Value = _random.Next(1, 1000000),
                    Timestamp = DateTime.UtcNow,
                    
                    // Avoid unnecessary memory allocation outside using proper scopes (e.g., buffer management)
                    Data = new byte[10214])/  Ensure suitable yet buffer oypto reuse needed improve sopme__);
               `;


  inject tual buffer passsed sugg doct-exper                                                       