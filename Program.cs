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

// Properly dispose HTTP request pipeline:
app.Use(async (context, next) =>
{
    await next();
    context.Response.OnCompleted(() =>
    {
        // Dispose or clean up resources here if necessary
        return Task.CompletedTask;
    });
});

using (app)
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapControllers();

    app.Run();
}

// Ensure proper disposal of services by implementing IDisposable pattern
public partial class LeakyCache : ILeakyCache, IDisposable
{
    // Implement IDisposable interface and dispose method
    public void Dispose()
    {
        // Dispose resources
    }
}

public partial class EventManager : IEventManager, IDisposable
{
    // Implement IDisposable interface and dispose method
    public void Dispose()
    {
        // Dispose resources
    }
}

public partial class DataGenerator : IDisposable
{
    // Implement IDisposable interface and dispose method
    public void Dispose()
    {
        // Dispose resources
    }
}

public partial class CPUTaskManager : IDisposable
{
    // Implement IDisposable interface and dispose method
    public void Dispose()
    {
        // Dispose resources
    }
}

public interface ILeakyCache { }
public interface IEventManager { }
public class LeakyCache : ILeakyCache
{
    // Example method which might need dispose pattern implementation
    public void ClearCache()
    {
        // Clear the cache
    }
}

public class EventManager : IEventManager
{
    // Example method which might need dispose pattern implementation
    public void ManageEvents()
    {
        // Manage events
    }
}

public class DataGenerator
{
    // Example method which might need dispose pattern implementation
    public void GenerateData()
    {
        // Generate data
    }
}

public class CPUTaskManager
{
    // Example method which might need dispose pattern implementation
    public void RunTasks()
    {
        // Run tasks
    }
}