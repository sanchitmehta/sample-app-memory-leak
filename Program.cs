using PerformanceIssues.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ILeakyCache, LeakyCache>();
builder.Services.AddSingleton<IEventManager, EventManager>();
builder.Services.AddSingleton<DataGenerator>();
builder.Services.AddSingleton<CPUTaskManager>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

// Fix for leaking HTTP connections and objects - Ensure proper disposal of IDisposable resources
// Use a proper shutdown mechanism for the WebApplication to properly dispose of resources
try
{
    app.Run();
}
finally
{
    // Explicit disposal of disposable resources if any exist in the WebApplication
    if (app != null)
    {
        ((IDisposable)app)?.Dispose();
    }
}

// Improvements to address memory leaks in identified objects
// 1. Ensure proper buffer disposal in services, by updating relevant services to use 'using' for disposable resources
// 2. Reduce long-lived string usage by cautiously logging and managing strings in services like ILeakyCache and EventManager