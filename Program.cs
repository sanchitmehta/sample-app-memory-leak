using PerformanceIssues.Serivces;
using PerformanceIssues.Services;
using System.Net.Http;

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

static void DisposeServices(IServiceProvider services)
{
    if (services is IDisposable disposable)
    {
        disposable.Dispose();
    }

    if (services is IServiceScopeFactory scopeFactory)
    {
        using var scope = scopeFactory.CreateScope();
        foreach (var service in scope.ServiceProvider.GetServices<IDisposable>())
        {
            service.Dispose();
        }
    }
}

// Modify the LeakyCache class to properly clear resources
public class LeakyCache : ILeakyCache, IDisposable
{
    private Dictionary<string, byte[]> _cache = new Dictionary<string, byte[]>();

    ~LeakyCache()
    {
        Dispose();
    }

    public void Dispose()
    {
        Clear();
        GC.SuppressFinalize(this);
    }

    public void Clear()
    {
        foreach (var item in _cache.Values)
        {
            Array.Clear(item, 0, item.Length);
        }
        _cache.Clear();
    }

    public void Add(string key, byte[] data)
    {
        if (_cache.ContainsKey(key))
        {
            Array.Clear(_cache[key], 0, _cache[key].Length);
        }
        _cache[key] = data;
    }
}

// Modify DataGenerator where HTTP resources are used
public class DataGenerator : IDisposable
{
    private readonly HttpClient _httpClient;

    public DataGenerator(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    ~DataGenerator()
    {
        Dispose();
    }

    public async Task<string> FetchDataAsync(string url)
    {
        using var response = await _httpClient.GetAsync(url).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}

// Modify CPUTaskManager to ensure proper disposal
public class CPUTaskManager : IDisposable
{
    private List<string> _tasks = new List<string>();
    private bool _disposed;

    public void AddTask(string task)
    {
        _tasks.Add(task);
    }

    public void Dispose()
    {
        ClearAllTasks();
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    ~CPUTaskManager()
    {
        Dispose();
    }

    private void ClearAllTasks()
    {
        if (_disposed) return;

        _tasks.Clear();
    }
}