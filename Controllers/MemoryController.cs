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

            // Check proper disposal in `ILeakyCache` implementation to prevent memory leaks internally
            var key = await _leakyCache.AddToCache(Guid.NewGuid().ToString(), request.SizeMB);
            return Ok(new { key, size = request.SizeMB });
        }

        [HttpGet("cache/size")]
        public IActionResult GetCacheSize()
        {
            // Ensure _leakyCache implementation does not retain excessive unused resources
            return Ok(new { size = _leakyCache.GetCacheSize() });
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe()
        {
            var id = Guid.NewGuid().ToString();
            Action<string> handler = msg =>
            {
                Console.WriteLine($"Event received for {id}: {msg}");
            };

            _eventManager.Subscribe(handler);

            // Fix for potential memory leak:
            // Unsubscribe to free up subscriptions once event handling for this method is complete
            try
            {
                await Task.Run(() => _eventManager.RaiseEvent($"Test event for {id}"));
            }
            finally
            {
                _eventManager.Unsubscribe(handler); // Ensure unsubscription after event is handled
            }

            return Ok(new { subscriberId = id });
        }

        [HttpPost("generate-data")]
        public async Task<IActionResult> GenerateData([FromBody] DataGenerationRequest request)
        {
            if (request.RecordCount <= 0 || request.RecordCount > 1000000)
                return BadRequest("Record count must be between 1 and 1,000,000");

            // Ensure DataGenerator does not create excessive memory allocation without cleanup
            await _dataGenerator.GenerateAndStoreData(request.RecordCount);
            return Ok(new { recordsGenerated = request.RecordCount });
        }

        [HttpGet("dump")]
        public IActionResult MemoryDump(int processId = 1)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = "collect-dump.sh",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            // Properly dispose `Process` to release OS resources
            using var process = Process.Start(processStartInfo);
            if (process is null)
            {
                return BadRequest("Failed to start memory dump process");
            }

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                return BadRequest(new { error });
            }

            // Parse and limit to top 100 objects
            var lines = output.Split('\n')
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Where(l => l.Contains("   ")) // Filter memory dump lines
                .Take(100);

            return Ok(new { memoryDump = string.Join("\n", lines) });
        }
    }
}