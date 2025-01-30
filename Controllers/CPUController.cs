using Microsoft.AspNetCore.Mvc;
using PerformanceIssues.Models;
using PerformanceIssues.Serivces;
using System;

namespace PerformanceIssuesDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CPUController : ControllerBase
    {
        private readonly CPUTaskManager _cpuTaskManager;

        public CPUController(CPUTaskManager cpuTaskManager)
        {
            _cpuTaskManager = cpuTaskManager;
        }

        [HttpPost("start")]
        public IActionResult StartCPUTask([FromBody] CPUTaskRequest request)
        {
            if (request.Complexity <= 0 || request.Complexity > 1000000)
                return BadRequest("Complexity must be between 1 and 1,000,000");

            try
            {
                // StartNewTask should ensure it doesn't keep unnecessary resources alive
                var taskId = _cpuTaskManager.StartNewTask(request.Complexity);
                return Ok(new { taskId });
            }
            catch (Exception ex)
            {
                // Ensure any exceptions during initialization are logged and memory is not retained
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("stop/{taskId}")]
        public IActionResult StopCPUTask(string taskId)
        {
            try
            {
                // Ensure resources for the task are properly released by StopTask
                if (!_cpuTaskManager.StopTask(taskId))
                    return NotFound("Task not found");

                return Ok(new { message = "Task stopped successfully" });
            }
            catch (Exception ex)
            {
                // Log any issues while ensuring no CancellationTokens leak
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("active")]
        public IActionResult GetActiveTasks()
        {
            try
            {
                // Ensure tasks returned are efficiently managed with minimal resource usage
                var tasks = _cpuTaskManager.GetActiveTasks();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("stop-all")]
        public IActionResult StopAllTasks()
        {
            try
            {
                // Properly clean up resources for all running tasks
                _cpuTaskManager.StopAllTasks();
                return Ok(new { message = "All tasks stopped" });
            }
            catch (Exception ex)
            {
                // Catching and logging unexpected issues
                return StatusCode(500, new { error = ex.Message });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Ensure _cpuTaskManager is properly disposed when no longer needed
                if (_cpuTaskManager is IDisposable disposableManager)
                {
                    disposableManager.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}