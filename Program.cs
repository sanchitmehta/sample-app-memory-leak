using PerformanceIssues.Serivces;
using PerformanceIssues.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ILeakyCache, LeakyCache>();
builder.Services.AddSingleton<IEventManager, EventManager>();
builder.Services.AddSingleton<DataGenerator>();
builder.Services.AddSingleton<CPUTaskManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

using (app) 
{
    app.Run();
}

// Ensure singleton services implement IDisposable and dispose them properly
public class LeakyCache : ILeakyCache, IDisposable
{
    private readonly Dictionary<string, byte[]> _cache = new();
    private bool _disposed;

    public void Add(string key, byte[] value)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(LeakyCache));
        _cache[key] = value;
    }

    public byte[]? Get(string key)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(LeakyCache));
        return _cache.TryGetValue(key, out var value) ? value : null;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _cache.Clear();
        _disposed = true;
    }
}

public class EventManager : IEventManager, IDisposable
{
    private readonly List<string> _events = new();
    private bool _disposed;

    public void AddEvent(string eventData)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(EventManager));
        _events.Add(eventData);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _events.Clear();
        _disposed = true;
    }
}

public class DataGenerator : IDisposable
{
    private System.Timers.Timer? _timer;
    private bool _disposed;

    public void StartGenerating()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(DataGenerator));
        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += (sender, e) => { /* generate data */ };
        _timer.Start();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _timer?.Dispose();
        _disposed = true;
    }
}

public class CPUTaskManager : IDisposable
{
    private readonly List<Task> _tasks = new();
    private bool _disposed;

    public void CreateTask(Func<Task> task)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(CPUTaskManager));
        _tasks.Add(task());
    }

    public async Task WaitAllTasksAsync()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(CPUTaskManager));
        await Task.WhenAll(_tasks);
        _tasks.Clear();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _tasks.Clear();
        _disposed = true;
    }
}