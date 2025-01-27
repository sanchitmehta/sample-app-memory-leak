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
    DisposeServices(app.Services);
});

app.Run();

static void DisposeServices(IServiceProvider serviceProvider)
{
    if (serviceProvider is IDisposable disposable)
    {
        disposable.Dispose();
    }
}

// EventManager.cs
public class EventManager : IEventManager, IDisposable
{
    private readonly List<EventHandler> _eventHandlers = new();

    public void RegisterEventHandler(EventHandler handler)
    {
        _eventHandlers.Add(handler);
    }
    
    public void ClearEventHandlers()
    {
        _eventHandlers.Clear();
    }
    
    public void Dispose()
    {
        ClearEventHandlers();
    }
}

// LeakyCache.cs
public class LeakyCache : ILeakyCache, IDisposable
{
    private readonly Dictionary<string, byte[]> _cache = new();

    public void Add(string key, byte[] data)
    {
        _cache[key] = data;
    }
    
    public void ClearCache()
    {
        _cache.Clear();
    }

    public void Dispose()
    {
        ClearCache();
    }
}

// DataGenerator.cs
public class DataGenerator : IDisposable
{
    private System.Timers.Timer _timer;

    public DataGenerator()
    {
        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += (sender, e) => GenerateData();
        _timer.Start();
    }

    private void GenerateData()
    {
        // Generate data logic
    }

    public void Dispose()
    {
        _timer.Stop();
        _timer.Dispose();
    }
}

// CPUTaskManager.cs
public class CPUTaskManager : IDisposable
{
    private List<Task> _tasks = new();
    private CancellationTokenSource _cancellationTokenSource;

    public CPUTaskManager()
    {
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public void StartTask()
    {
        var token = _cancellationTokenSource.Token;
        _tasks.Add(Task.Run(() => PerformHeavyCPUTask(token), token));
    }

    private void PerformHeavyCPUTask(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            // Perform CPU-intensive work
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        Task.WhenAll(_tasks).Wait();
        _cancellationTokenSource.Dispose();
        _tasks.Clear();
    }
}