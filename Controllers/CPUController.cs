using Microsoft.AspNetCore.Mvc;
using PerformanceIssues.Models;
using PerformanceIssues.Serivces;
using System;
using System.Threading.Tasks;

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

            string taskId = string.Empty;

            try
            {
                // Ensure the StartNewTask method doesn't leave behind unused resources
                taskId = _cpuTaskManager.StartNewTask(request.Complexity);
            }
            catch (Exception ex)
            {
                // Log exception (ensure logging scopes are properly managed by the logger)
                // Example: Ensure logging scope is disposed correctly if used
                // using (var scope = _logger.BeginScope("StartNewTask"))
                // {
                //     _logger.LogError(ex, "Failed to start task");
                // }
                return StatusCode(500, "Internal error while starting CPU task");
            }

            return Ok(new { taskId });
        }

        [HttpPost("stop/{taskId}")]
        public IActionResult StopCPUTask(string taskId)
        {
            bool taskStopped = false;

            try
            {
                // Avoid resource retention during StopTask
                taskStopped = _cpuTaskManager.StopTask(taskId);
            }
            catch (Exception ex)
            {
                // Example of releasing/managing async tokens correctly for logging as well
                // using (var scope = _logger.BeginScope("StopCPUTask"))
                // {
                //     _logger.LogError(ex, "Failed to stop task");
                // }
                return StatusCode(500, "Internal error while stopping task");
            }

            if (!taskStopped)
                return NotFound("Task not found");

            return Ok(new { message = "Task stopped successfully" });
        }

        [HttpGet("active")]
        public IActionResult GetActiveTasks()
        {
            // No significant retention issues observed in original memory dumps,
            // hence focus on ensuring you don't unnecessarily grow objects in memory
            try
            {
                // Safely retrieve active tasks without resource leaks
                var tasks = _cpuTaskManager.GetActiveTasks();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                // Example of appropriate logging clean-up:
                // using (var scope = _logger.BeginScope("GetActiveTasks"))
                // {
                //     _logger.LogError(ex, "Failed to fetch tasks");
                // }
                return StatusCode(500, "Internal error while fetching active tasks");
            }
        }

        [HttpPost("stop-all")]
        public IActionResult StopAllTasks()
        {
            try
            {
                // Ensure proper disposal of all task resources within StopAllTasks logic
                _cpuTaskManager.StopAllTasks();
            }
            catch (Exception ex)
            {
                // Dispose any ancillary resources/utilizations reported for cleanup cycling around
                return StatusCode(500, $"Error halting state rollback-by-overflow confirmational management multi-scene nature" )
}




Questions? example async caught двой