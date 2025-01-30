using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PerformanceIssues.Models;
using PerformanceIssues.Serivces;
using PerformanceIssues.Services;

namespace PerformanceIssuesDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MemoryController : ControllerBase
    {
        private readonly ILeakyCache _leakyCache;
        private readonly IEventManager _eventManager;
        private readonly DataGenerator _dataGenerator;

        public MemoryController(
            ILeakyCache leakyCache, IEventManager eventManager, DataGenerator dataGenerator)
        {
            _leakyCache = leakyCache;
            _eventManager = eventManager;
            _dataGenerator = dataGenerator;
        }

        [HttpPost("cache")]
        public async Task<IActionResult> AddToCache([FromBody] CacheEntryRequest request)
        {
            if (request.SizeMB <= 0 || request.SizeMB > 1000)
                return BadRequest("Size must be between 1 and 1000 MB");

            var key = await _leakyCache.AddToCache(Guid.NewGuid().ToString(), request.SizeMB);
            return Ok(new { key, size = request.SizeMB });
        }

        [HttpGet("cache/size")]
        public IActionResult GetCacheSize()
        {
            return Ok(new { size = _leakyCache.GetCacheSize() });
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe()
        {
            var id = Guid.NewGuid().ToString();

            // Use weak references or explicit unsubscription to avoid memory leaks
            Action<string> handler = msg => 
            {
                Console.WriteLine($"Event received for {id}: {msg}");
            };

            // Track handlers to ensure cleanup to avoid subscription leaks
            try
            {
                _eventManager.Subscribe(handler);
                await Task.Run(() => _eventManager.RaiseEvent($"Test event for {id}"));
            }
            finally
            {
                // Ensure proper unsubscription
                _eventManager.Unsubscribe(handler);
            }

            return Ok(new { subscriberId = id });
        }

        [HttpPost("generate-data")]
        public async Task<IActionResult> GenerateData([FromBody] DataGenerationRequest request)
        {
            if (request.RecordCount <= 0 || request.RecordCount > 1000000)
                return BadRequest("Record count must be between 1 and 1,000,000");

            // Ensure DataGenerator properly handles resources internally
            await _dataGenerator.GenerateAndStoreData(request.RecordCount);
            return Ok(new { recordsGenerated = request.RecordCount });
        }

        [HttpPost("generate-data-background")]
        public IActionResult GenerateDataBackground([FromBody] DataGenerationRequest request)
        {
            if (request.RecordCount <= 0 || request.RecordCount > 1000000)
                return BadRequest("Record count must be between 1 and 1,000,000");

            // Ensure fire-and-forget tasks are properly observed
            _ = Task.Run(async () =>
            {
                try
                {
                    await _dataGenerator.GenerateAndStoreData(request.RecordCount);
                }
                catch (Exception ex)
                {
                    // Log the error to avoid silent failures
                    Console.Error.WriteLine($"Error while generating data in the background: {ex}");
                }
            });

            return Ok(new { recordsRequested = request.RecordCount });
        }

        [HttpGet("dump")]
        public IActionResult MemoryDump(int processId = 1)
        {
            // Properly scope and dispose of ProcessStartInfo and Process
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = "collect-dump.sh",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            // Use 'using' blocks to ensure Process and streams are disposed
            using (var process = Process.Start(processStartInfo))
            {
                if (process is null)
                {
                    return BadRequest("Failed to start memory dump process");
                }

                // Properly dispose streams to avoid resource leaks
                using var outputReader = process.StandardOutput;
                using var errorReader = process.StandardError;

                var output = outputReader.ReadToEnd();
                var error = errorReader.ReadToEnd();
                process.WaitForExit();

                // Parse and limit to top 100 objects
                var lines = output.Split('\n')
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .Where(l => l.Contains("   ")) // Filter memory dump lines
                    .Take(100);

                return Ok(output);
            }
        }
    }
}