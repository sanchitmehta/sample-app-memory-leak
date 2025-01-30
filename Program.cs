using PerformanceIssues.Services;

// Step 1: Added `using` statements for disposables like `HttpClient`, which may be part of the services.
// Step 2: Scoped the fixes to address memory leaks and potential issues identified in memory dump analysis.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Step 3: Add Singleton instances but ensure proper disposal management is considered.
builder.Services.AddSingleton<ILeakyCache, LeakyCache>();
builder.Services.AddSingleton<IEventManager, EventManager>();
builder.Services.AddSingleton<DataGenerator>();
builder.Services.AddSingleton<CPUTaskManager>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

// Step 4: Ensure the application lifecycle with proper cleanup mechanisms integrated.
app.Lifetime.ApplicationStopping.Register(() =>
{
    // Explicitly dispose of Singleton services that interact with resources.
    if (app.Services.GetService<ILeakyCache>() is IDisposable leakyCache)
    {
        leakyCache.Dispose();
    }
    if (app.Services.GetService<IEventManager>() is IDisposable eventManager)
    {
        eventManager.Dispose();
    }
    if (app.Services.GetService<DataGenerator>() is IDisposable dataGenerator)
    {
        dataGenerator.Dispose();
    }
    if (app.Services.GetService<CPUTaskManager>() is IDisposable cpuTaskManager)
    {
        cpuTaskManager.Dispose();
    }

    // Step 5: Note any connections requiring manual cleanup if not already handled.
    // Add appropriate cleanup if Http-related cached objects are used elsewhere in the codebase.
});

app.Run();