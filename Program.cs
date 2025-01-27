using PerformanceIssues.Services;
using System;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ILeakyCache, LeakyCache>();
builder.Services.AddSingleton<IEventManager, EventManager>();
builder.Services.AddSingleton<DataGenerator>();
builder.Services.AddSingleton<CPUTaskManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Lifetime.ApplicationStopping.Register(() =>
{
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
});

app.Run();

public class LeakyCache : ILeakyCache, IDisposable
{
    private List<byte[]> _cache = new();
    public void AddToCache(byte[] data) => _cache.Add(data);
    public void Dispose() => _cache.Clear();
}

public class EventManager : IEventManager, IDisposable
{
    private List<string> _events = new();
    public void RegisterEvent(string eventInfo) => _events.Add(eventInfo);
    public void Dispose() => _events.Clear();
}

public class DataGenerator : IDisposable
{
    private bool _disposed = false;

    // Dispose pattern implementation
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing) 
        {
            // Release managed resources
        }
        _disposed = true;
    }

    ~DataGenerator()
    {
        Dispose(false);
    }
}

public class CPUTaskManager : IDisposable
{
    private CancellationTokenSource _cts = new();

    public void Dispose() => _cts.Dispose();
}

public interface ILeakyCache
{
    void AddToCache(byte[] data);
}

public interface IEventManager
{
    void RegisterEvent(string eventInfo);
}