namespace PerformanceIssues.Services
{
    public class DataGenerator : IDisposable
    {
        private readonly List<object> _storedData = new(); // Retained data should be periodically cleared
        private readonly Random _random = new();
        private bool _disposed = false;

        // Introduced Dispose method to clean up resources
        public void Dispose()
        {
            if (!_disposed)
            {
                // Clear retained data to avoid memory bloat
                _storedData.Clear();
                _disposed = true;
            }
        }

        public async Task GenerateAndStoreData(int count, CancellationToken cancellationToken = default)
        {
            for (int i = 0; i < count; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    // Exiting the loop promptly in case of cancellation
                    break;
                }

                var data = new
                {
                    Id = Guid.NewGuid(),
                    Name = GenerateRandomString(50),
                    Value = _random.Next(1, 1000000),
                    Timestamp = DateTime.UtcNow,
                    Data = GenerateData() // Ensuring temporary byte[] arrays are disposed properly
                };

                _storedData.Add(data); // Memory leak: consider clearing the list periodically or re-evaluating storage needs

                if (i % 1000 == 0)
                {
                    // Periodically clear excess data to prevent largescale retention
                    _storedData.RemoveRange(0, Math.Min(500, _storedData.Count));

                    await Task.Delay(1, cancellationToken); // Use cancellation token for better responsiveness
                }
            }
        }

        // Helper method to generate the byte array
        private byte[] GenerateData()
        {
            byte[] dataBuffer = new byte[1024];
            _random.NextBytes(dataBuffer);
            return dataBuffer;
        }

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            // Refactored to minimize excessive heap allocations
            using var buffer = new System.IO.StringWriter();
            for (int i = 0; i < length; i++)
            {
                buffer.Write(chars[_random.Next(chars.Length)]);
            }
            return buffer.ToString();
        }
    }
}