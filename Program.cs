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

app.Lifetime.ApplicationStopping.Register(() =>
{
    // Ensure all disposable services are properly disposed on application shutdown
    var disposableServices = app.Services.GetServices<IDisposable>();
    foreach (var disposable in disposableServices)
    {
        disposable.Dispose();
    }
});

app.Run();

public class DataGenerator : IDisposable
{
    private readonly Timer _timer;
    private bool _disposed;

    public DataGenerator()
    {
        _timer = new Timer(GenerateData, null, 0, 1000);
    }

    private void GenerateData(object? state)
    {
        // Simulating data generation
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _timer.Dispose();
            _disposed = true;
        }
    }
}

public class EventManager : IEventManager, IDisposable
{
    private readonly List<EventHandler> _eventHandlers = new();
    private bool _disposed;

    public void RegisterHandler(EventHandler handler)
    {
        _eventHandlers.Add(handler);
    }

    public void TriggerEvents()
    {
        foreach (var handler in _eventHandlers)
        {
            handler?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _eventHandlers.Clear();
            _disposed = true;
        }
    }
}

public class LeakyCache : ILeakyCache, IDisposable
{
    private readonly Dictionary<string, byte[]> _cache = new();
    private bool _disposed;

    public void AddToCache(string key, byte[] value)
    {
        _cache[key] = value;
    }

    public byte[]? GetFromCache(string key)
    {
        return _cache.TryGetValue(key, out var value) ? value : null;
    }

    public void ClearCache()
    {
        _cache.Clear();
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _cache.Clear();
            _disposed = true;
        }
    }
}

public class CPUTaskManager : IDisposable
{
    private CancellationTokenSource? _cts;
    private Task? _runningTask;
    private bool _disposed;

    public void StartTask()
    {
        _cts = new CancellationTokenSource();
        _runningTask = Task.Run(() => PerformCPUIntensiveTask(_cts.Token), _cts.Token);
    }

    private void PerformCPUIntensiveTask(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            // Simulating CPU-intensive task
        }
    }

    public void StopTask()
    {
        _cts?.Cancel();
        _runningTask?.Wait();
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            StopTask();
            _cts?.Dispose();
            _disposed = true;
        }
    }
}