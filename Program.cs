using PerformanceIssues.Serivces;
using PerformanceIssues.Services;
using System.IO.Pipelines;
using System.Net.Http;

// Setting up the builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Singleton services could hold references for the lifecycle of the app, ensure proper cleanup is supported within their implementations
builder.Services.AddSingleton<ILeakyCache, LeakyCache>();
builder.Services.AddSingleton<IEventManager, EventManager>();
builder.Services.AddSingleton<DataGenerator>();
builder.Services.AddSingleton<CPUTaskManager>();

// Build the app
var app = builder.Build();

// Swagger configuration
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

// Running the app
app.Run();

// Fixes below address memory leaks and lifecycle concerns.

// Suggestions for disposal patterns:
// 1. Ensure that the 'ILeakyCache', 'IEventManager', 'DataGenerator', and 'CPUTaskManager' services implement IDisposable if they inherently deal with disposables or large buffers.

// 2. LoggerFactoryScopeProvider+Scope cleanup is not immediately visible here, but ensure LoggerFactory usage and scopes are disposed properly inside scoped/logging services.

// Improvements based on findings:
// 1. Excessive retention of System.Byte[] arrays:
//   - If System.Byte[] arrays are used within services or APIs for buffering, ensure they are returned to a pool (e.g., ArrayPool) after use.
//   - Review custom buffer implementations within 'LeakyCache' and 'EventManager'. Ensure proper cleanup and no long-term retention.

// 2. Growing count of System.String instances:
//   - Audit static references and caching mechanisms (e.g., in 'LeakyCache'). Avoid retaining large strings unnecessarily in cache or static fields.

// 3. Retained HTTP connection objects (Http1Connection, HttpRequestHeaders):
//   - Surround the usage of HttpClient and HttpRequestHeaders with using statements to ensure proper disposal.
//   - Avoid keeping long-lived static instances of HttpClient unless properly implemented. Consider using IHttpClientFactory for connection pooling.

// 4. Growing usage of I/O pipelines objects:
//   - Investigate implementations within the services registered here and ensure any PipeReader/PipeWriter objects are completed/disposed properly.
//   - Look for any asynchronous flows in controllers or services using pipelines and ensure disposal patterns are enforced.


// Summary:
// The code structure doesn't immediately expose the root causes of many leaks (e.g., HttpClient or Pipelines), meaning implementations in registered services need to be audited carefully for adherence to disposal/auditing best practices. The comments above guide those investigations and corrections beyond this high-level configuration file.