using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PerformanceIssues.Models;
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
            Action<string> handler = msg => Console.WriteLine($"Event received for {id}: {msg}");

            // Ensure the handler is unsubscribed after its usage to prevent a memory leak.
            _eventManager.Subscribe(handler);
            try
            {
                await Task.Run(() => _eventManager.RaiseEvent($"Test event for {id}"));
            }
            finally
            {
                _eventManager.Unsubscribe(handler);
            }
            
            return Ok(new { subscriberId = id });
        }

        [HttpPost("generate-data")]
        public async Task<IActionResult> GenerateData([FromBody] DataGenerationRequest request)
        {
            if (request.RecordCount <= 0 || request.RecordCount > 1000000)
                return BadRequest("Record count must be between 1 and 1,000,000");

            await _dataGenerator.GenerateAndStoreData(request.RecordCount);
            return Ok(new { recordsGenerated = request.RecordCount });
        }

        [HttpPost("generate-data-background")]
        public IActionResult GenerateDataBackground([FromBody] DataGenerationRequest request)
        {
            if (request.RecordCount <= 0 || request.RecordCount > 1000000)
                return BadRequest("Record count must be between 1 and 1,000,000");

            // Start generating data on a background thread (fire-and-forget).
            _ = Task.Run(async () => 
            {
                try
                {
                    await _dataGenerator.GenerateAndStoreData(request.RecordCount);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in background task: {ex.Message}");
                }
            });

            // Immediately return a response, so the caller isn't blocked.
            return Ok(new { recordsRequested = request.RecordCount });
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

            // Ensure the process is properly disposed of after its usage
            using var process = Process.Start(processStartInfo);
            if (process is null)
            {
                return BadRequest("Failed to start memory dump process");
            }

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
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