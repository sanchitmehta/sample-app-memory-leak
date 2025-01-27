using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PerformanceIssues.Models;
using PerformanceIssues.Services;

namespace PerformanceIssuesDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MemoryController : ControllerBase, IDisposable
    {
        private readonly ILeakyCache _leakyCache;
        private readonly IEventManager _eventManager;
        private readonly DataGenerator _dataGenerator;
        private bool _disposed;

        public MemoryController(
            ILeakyCache leakyCache,
            IEventManager eventManager,
            DataGenerator dataGenerator)
        {
            _leakyCache = leakyCache ?? throw new ArgumentNullException(nameof(leakyCache));
            _eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            _dataGenerator = dataGenerator ?? throw new ArgumentNullException(nameof(dataGenerator));
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
            Action<string> handler = msg =>
            {
                try
                {
                    Console.WriteLine($"Event received for {id}: {msg}");
                }
                catch
                {
                    // Log or handle exception during event processing
                }
            };

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

            var lines = output.Split('\n')
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Where(l => l.Contains("   "))
                .Take(100);

            return Ok(new { memoryDump = string.Join("\n", lines) });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose unmanaged resources if any are used in this controller in the future.
                }

                // Clear any large collections or resources if necessary here.
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}