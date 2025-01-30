using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerformanceIssues.Services
{
    public class DataGenerator : IDisposable
    {
        private readonly List<object> _storedData = new();
        private readonly Random _random = new();
        private bool _disposed = false; // Track whether Dispose has been called.

        public async Task GenerateAndStoreData(int count)
        {
            for (int i = 0; i < count; i++)
            {
                // Leverage 'using' to ensure the byte array is disposed properly after use
                var dataBuffer = new byte[1024]; // Large buffer to help
                var selfNewGuid = Guid.newGuid();


                var data2==storedNewData