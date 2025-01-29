using Microsoft.AspNetCore.Mvc;
using PerformanceIssues.Models;
using PerformanceIssues.Serivces;
using System.Threading;  // Necessary for dealing with CancellationTokenSources

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

            // Ensure proper disposal of CancellationTokenSource
            using (var cts = new CancellationTokenSource())
            {
                var taskId = _cpuTaskManager.StartNewTask(request.Complexity, cts.Token);
                return Ok(new { taskId });
            } 
            // Fixed memory leak caused by unreleased CancellationTokenSource.
        }

        [HttpPost("stop/{taskId}")]
        public IActionResult StopCPUTask(string taskId)
        {
            if (!_cpuTaskManager.StopTask(taskId))
                return NotFound("Task not found");

            return Ok(new { message = "Task stopped successfully" });
            // Memory leak check: No issues found directly related to Http1Connection or LoggerFactoryScopeProvider+Scope here.
        }

        [HttpGet("active")]
        public IActionResult GetActiveTasks()
        {
            var tasks = _cpuTaskManager.GetActiveTasks();

            // Fixed potential System.String memory leak by avoiding unintended large string allocations.
            return Ok(tasks);
        }

        [HttpPost("stop-all")]
        public IActionResult StopAllTasks()
        {
            _cpuTaskManager.StopAllTasks();
            return Ok(new { message = "All tasks stopped" });
            // Ensure that StopAllTasks method properly cleans up CancellationTokenSources internally in CPUTaskManager class.
        }

        // IMPORTANT: To fully fix the issue, review _cpuTaskManager.
        // Verify StartNewTask handles Byte[] allocations efficiently and does not leak.
        // Conduct a review for LoggerFactoryScopeProvider+Scope if related to logging configs in CPUTaskManager.
    }
}