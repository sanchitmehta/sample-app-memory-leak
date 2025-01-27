using PerformanceIssues.Services;
using System;
using Microsoft.Extensions.Hosting;

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

app.Lifetime.ApplicationStopping.Register(OnShutdown);

app.Run();

void OnShutdown()
{
    var leakyCache = app.Services.GetService<ILeakyCache>() as IDisposable;
    if (leakyCache != null)
    {
        leakyCache.Dispose();
    }

    var eventManager = app.Services.GetService<IEventManager>() as IDisposable;
    if (eventManager != null)
    {
        eventManager.Dispose();
    }

    var dataGenerator = app.Services.GetService<DataGenerator>() as IDisposable;
    if (dataGenerator != null)
    {
        dataGenerator.Dispose();
    }

    var cpuTaskManager = app.Services.GetService<CPUTaskManager>() as IDisposable;
    if (cpuTaskManager != null)
    {
        cpuTaskManager.Dispose();
    }
}

public class LeakyCache : ILeakyCache, IDisposable
{
    private Dictionary<string, object> _cache = new();
    private bool _disposed = false;

    public void Add(string key, object value)
    {
        _cache[key] = value;
    }

    public object Get(string key)
    {
        _cache.TryGetValue(key, out var value);
        return value;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _cache.Clear();
            _cache = null;
            _disposed = true;
        }
    }
}

public class EventManager : IEventManager, IDisposable
{
    private event EventHandler SomeEvent;
    private bool _disposed = false;

    public void OnSomeEvent()
    {
        SomeEvent?.Invoke(this, EventArgs.Empty);
    }

    public void AddEventHandler(EventHandler handler)
    {
        SomeEvent += handler;
    }

    public void RemoveEventHandler(EventHandler handler)
    {
        SomeEvent -= handler;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            foreach (var d in SomeEvent.GetInvocationList())
            {
                SomeEvent -= (EventHandler)d;
            }
            _disposed = true;
        }
    }
}

public class DataGenerator : IDisposable
{
    private bool _disposed = false;

    public void GenerateData()
    {
        // Generate some data
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
        }
    }
}

public class CPUTaskManager : IDisposable
{
    private bool _disposed = false;

    public void RunTasks()
    {
        // Run some CPU intensive tasks
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
        }
    }
}