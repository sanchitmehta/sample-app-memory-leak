using PerformanceIssues.Serivces;
using PerformanceIssues.Services;
using Microsoft.Extensions.Logging; // Ensure logging namespace is imported
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ILeakyCache, LeakyCache>(); // Ensure disposal of cache resources.
builder.Services.AddSingleton<IEventManager, EventManager>(); // Ensure proper management of event subscriptions.
builder.Services.AddSingleton<DataGenerator>(); // Ensure generated data is correctly disposed when not needed.
builder.Services.AddSingleton<CPUTaskManager>(); // Manage CancellationTokens in tasks properly.

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

try
{
    // Run the application inside try to catch unhandled exceptions that might result in resource leaks.
    app.Run();
}
finally
{
    // Release application-wide resources if any.
    
    // Dispose of services and release unmanaged resources if needed.
    if (app is IDisposable disposableApp)
    {
        disposableApp.Dispose();
    }
}

/* Suggested Fixes and Notes:
1. System.Byte[]: Review LeakyCache and DataGenerator for improper allocation or retention of Byte[] objects.
   - Ensure Byte[] instances are scoped correctly and not held in memory beyond their usage. 
   - For large arrays, consider using ArrayPool<byte>.Shared for memory pooling and efficient reuse.

2. System.String: Avoid retaining large strings unnecessarily, especially in logs or caches. 
   - Analyze string usage in logging or persistent caches (e.g., LeakyCache) and ensure proper lifecycle handling.

3. Http1Connection: Ensure connections are properly disposed after each request cycle.
   - If EventManager or controllers create HttpClient or similar objects, ensure they use HttpClientFactory or are disposed via 'using' blocks.

4. LoggerFactoryScopeProvider+Scope: Ensure no unintended logging scope retention.
   - Avoid long-lived logging scope instances; ensure ILoggerFactory or scoped ILogger instances are properly released.

5. CancellationTokenSource: Avoid leaking CancellationTokenSource when tasks are canceled.
   - Review tasks created in DataGenerator and CPUTaskManager, ensure CancellationTokenSource is disposed once the task completes or is canceled.

6. General Improvements:
   - Use 'using' blocks for disposable resources.
   - Validate service lifetimes match resource requirements (e.g., transient for short-lived resources).

7. Verify logging configuration and ensure unnecessary logs are minimized, especially if heavy string formatting is used.

*/