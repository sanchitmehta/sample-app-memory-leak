using System.Collections.Concurrent;
using System.Text;

namespace PerformanceIssues.Services
{
    public class DataGenerator : IDisposable
    {
        private readonly ConcurrentBag<object> _storedData = new();
        private readonly Random _random = new();
        private bool _disposed = false;

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
                    Data = GenerateRandomData()
                };

                _storedData.Add(data);

                if (i % 1000 == 0)
                {
                    await Task.Delay(1);
                }
            }
        }

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringBuilder = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                int index = _random.Next(chars.Length);
                stringBuilder.Append(chars[index]);
            }
            return stringBuilder.ToString();
        }

        private byte[] GenerateRandomData()
        {
            var data = new byte[1024];
            _random.NextBytes(data);
            return data;
        }

        public void ClearStoredData()
        {
            if (_storedData.Count > 0)
            {
                // Ensure all objects in the collection are no longer referenced
                while (!_storedData.IsEmpty)
                {
                    _storedData.TryTake(out _);
                }
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                ClearStoredData();

                GC.SuppressFinalize(this);
                _disposed = true;
            }
        }

        ~DataGenerator()
        {
            Dispose();
        }
    }
}