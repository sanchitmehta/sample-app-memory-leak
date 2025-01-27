using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PerformanceIssues.Models;
using PerformanceIssues.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PerformanceIssuesDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MemoryController : ControllerBase, IDisposable
    {
        private readonly ILeakyCache _leakyCache;
        private readonly IEventManager _eventManager;
        private readonly DataGenerator _dataGenerator;
        private bool _disposed = false;

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

            try
            {
                var key = await _leakyCache.AddToCache(Guid.NewGuid().ToString(), request.SizeMB);
                return Ok(new { key, size = request.SizeMB });
            }
            finally
            {
                // Ensuring any temporary usage is cleaned up if necessary
            }
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
            Action<string> handler = null;

            try
            {
                handler = msg => Console.WriteLine($"Event received for {id}: {msg}");
                _eventManager.Subscribe(handler);
                await Task.Run(() => _eventManager.RaiseEvent($"Test event for {id}"));
                return Ok(new { subscriberId = id });
            }
            finally
            {
                if (handler != null)
                {
                    _eventManager.Unsubscribe(handler);
                }
            }
        }

        [HttpPost("generate-data")]
        public async Task<IActionResult> GenerateData([FromBody] DataGenerationRequest request)
        {
            if (request.RecordCount <= 0 || request.RecordCount > 1000000)
                return BadRequest("Record count must be between 1 and 1,000,000");

            try
            {
                await _dataGenerator.GenerateAndStoreData(request.RecordCount);
                return Ok(new { recordsGenerated = request.RecordCount });
            }
            catch (Exception ex)
            {
                // Log exception if necessary
                return StatusCode(500, new { error = ex.Message });
            }
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

            try
            {
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
            finally
            {
                process?.Dispose();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    if (_leakyCache is IDisposable disposableCache)
                    {
                        disposableCache.Dispose();
                    }

                    if (_eventManager is IDisposable disposableEventManager)
                    {
                        disposableEventManager.Dispose();
                    }

                    if (_dataGenerator is IDisposable disposableDataGenerator)
                    {
                        disposableDataGenerator.Dispose();
                    }
                }

                // Free unmanaged resources if any
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}