using Microsoft.AspNetCore.Mvc;
using PerformanceIssues.Models;
using PerformanceIssues.Serivces;
using System;

namespace PerformanceIssuesDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CPUController : ControllerBase, IDisposable
    {
        private readonly CPUTaskManager _cpuTaskManager;
        private bool _disposed = false;

        public CPUController(CPUTaskManager cpuTaskManager)
        {
            _cpuTaskManager = cpuTaskManager ?? throw new ArgumentNullException(nameof(cpuTaskManager));
        }

        [HttpPost("start")]
        public IActionResult StartCPUTask([FromBody] CPUTaskRequest request)
        {
            if (request.Complexity <= 0 || request.Complexity > 1000000)
                return BadRequest("Complexity must be between 1 and 1,000,000");

            var taskId = _cpuTaskManager.StartNewTask(request.Complexity);
            return Ok(new { taskId });
        }

        [HttpPost("stop/{taskId}")]
        public IActionResult StopCPUTask(string taskId)
        {
            if (!_cpuTaskManager.StopTask(taskId))
                return NotFound("Task not found");

            return Ok(new { message = "Task stopped successfully" });
        }

        [HttpGet("active")]
        public IActionResult GetActiveTasks()
        {
            var tasks = _cpuTaskManager.GetActiveTasks();
            return Ok(tasks);
        }

        [HttpPost("stop-all")]
        public IActionResult StopAllTasks()
        {
            _cpuTaskManager.StopAllTasks();
            return Ok(new { message = "All tasks stopped" });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Clean up the task manager
                    _cpuTaskManager.Dispose();
                }
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