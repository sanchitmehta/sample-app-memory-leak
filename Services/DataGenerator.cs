namespace PerformanceIssues.Services
{
    public class DataGenerator : IDisposable
    {
        private readonly List<object> _storedData = new();
        private readonly Random _random = new();
        private bool _disposed = false;

        // generates about 1MB of data per minute
        public async Task GenerateAndStoreData(int count)
        {
            for (int i = 0; i < count; i++)
            {
                // Ensure proper disposal of the generated data if possible (adjust logic elsewhere if retained)
                var data = new
                {
                    Id = Guid.NewGuid(),
                    Name = GenerateRandomString(50),
                    Value = _random.Next(1, 1000000),
                    Timestamp = DateTime.UtcNow,
                    Data = new byte[1024]  // ~1 KB
                };
            
                // Avoid correlation of excessive buffer lifetimes by clearing up references later, if needed.
                _random.NextBytes(data.Data);
                _storedData.Add(data);

                // ~1 ms delay => 1024 ms for 1024 records => ~1 MB/s
                await Task.Delay(1);
            }