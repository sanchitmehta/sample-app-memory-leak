using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
            ILeakyCache leakyCache,
            IEventManager eventManager,
            DataGenerator dataGenerator)
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

            // Ensure unused byte arrays in the cache are cleared promptly (handled in ILeakyCache implementation).
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

            // Use weak references or unsubscribe mechanisms to prevent handler retention.
            Action<string> handler = msg =>
            {
                Console.WriteLine($"Event received for {id}: {msg}");
            };

            _eventManager.Subscribe(handler);
            await Task.Run(() =>
            {
                _eventManager.RaiseEvent($"Test event for {id}");

                // Unsubscribe the handler to avoid leaks.
                _eventManager.Unsubscribe(handler);
            });

            return Ok(new { subscriberId = id });
        }

        [HttpPost("generate-data")]
        public async Task<IActionResult> GenerateData([FromBody] DataGenerationRequest request)
        {
            if (request.RecordCount <= 0 || request.RecordCount > 1000000)
                return BadRequest("Record count must be between 1 and 1,000,000");

            // Dispose large disposable resources within DataGenerator appropriately.
            await _dataGenerator.GenerateAndStoreData(request.RecordCount);
            return Ok(new { recordsGenerated = request.RecordCount });
        }

        [HttpPost("generate-data-background")]
        public IActionResult GenerateDataBackground([FromBody] DataGenerationRequest request)
        {
            if (request.RecordCount <= 0 || request.RecordCount > 1000000)
                return BadRequest("Record count must be between 1 and 1,000,000");

            // Fire-and-forget using Task.Run for background processing.
            _ = Task.Run(async () =>
            {
                try
                {
                    // Proper async token handling within the service level.
                    await _dataGenerator.GenerateAndStoreData(request.RecordCount);
                }
                catch (Exception ex)
                {
                    // Log or handle exceptions to prevent silent failures.
                    Console.WriteLine($"Background generation error: {ex.Message}");
                }
            });

            return Ok(new { recordsRequested = request.RecordCount });
        }

        [HttpGet("dump")]
        public IActionResult MemoryDump(int processId = 1)
        {
            // Always use proper resource cleanup for processes.
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = "collect-dump.sh",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            using var process = Process.Start(processStartInfo);
            if (process is null)
            {
                return BadRequest("Failed to start memory dump process");
            }

            // Ensure streams are read and disposed properly.
            string output;
            string error;

            using (var standardOutput = process.StandardOutput)
            using (var standardError = process.StandardError)
            {
                output = standardOutput.ReadToEnd();
                error = standardError.ReadToEnd();
            }

            process.WaitForExit();

            if (!string.IsNullOrEmpty(error))
            {
                return BadRequest($"Error during memory dump collection: {error}");
            }

            // Parse and limit to top 100 objects safely.
            var lines = output.Split('\n')
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Where(l => l.Contains("   ")) // Filter memory dump lines
                .Take(100);

            return Ok(string.Join("\n", lines));
        }
    }
}