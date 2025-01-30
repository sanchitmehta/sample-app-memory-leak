using System.Collections.Concurrent;
using System.Net.Http;
using Microsoft.Extensions.Logging;

﻿namespace PerformanceIssues.Services
{
    public class DataGenerator
    {
        private readonly ConcurrentBag<object> _storedData = new(); // Changed List to ConcurrentBag for thread safety
        private readonly Random _random = new();
        private readonly ILogger<DataGenerator> _logger; // Assuming an ILogger is injected
        private readonly HttpClient _httpClient; // Assuming an HttpClient is used elsewhere in the implementation
    
        // Constructor to initialize HttpClient and Logger
        public DataGenerator(ILogger<DataGenerator> logger, HttpClient httpClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }
    
        // generates about 1MB of data per minute
        public async Task GenerateAndStoreData(int count)
        {
              