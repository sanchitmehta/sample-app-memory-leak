using PerformanceIssues.Serivces;
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

app.Run();

public class LeakyCache : ILeakyCache, IDisposable
{
    private Dictionary<string, object> _cache = new Dictionary<string, object>();
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
        if (_disposed)
            return;
        
        _cache.Clear();
        _cache = null;

        _disposed = true;
    }
}

public class EventManager : IEventManager, IDisposable
{
    public event EventHandler SomeEvent;
    private bool _disposed = false;

    public void TriggerEvent()
    {
        SomeEvent?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        SomeEvent = null;
        
        _disposed = true;
    }
}

public class DataGenerator : IDisposable
{
    private bool _disposed = false;

    public void Dispose()
    {
        if (_disposed)
            return;
        
        _disposed = true;
    }
}

public class CPUTaskManager : IDisposable
{
    private bool _disposed = false;

    public void Dispose()
    {
        if (_disposed)
            return;
        
        _disposed = true;
    }
}

public interface ILeakyCache
{
    void Add(string key, object value);
    object Get(string key);
}

public interface IEventManager
{
    event EventHandler SomeEvent;
}

public class WebApplication : IDisposable
{
    private bool _disposed = false;

    public static WebApplicationBuilder CreateBuilder(string[] args)
    {
        return new WebApplicationBuilder();
    }
        
    public void UseSwagger() { /* implementation */ }
    public void UseSwaggerUI() { /* implementation */ }
    public void MapControllers() { /* implementation */ }
    public void Run() { /* implementation */ }

    public void Dispose()
    {
        if (_disposed)
            return;
        
        // dispose resources

        _disposed = true;
    }
}

public class WebApplicationBuilder
{
    public WebApplication Build()
    {
        return new WebApplication();
    }
    
    public IServiceCollection Services => new ServiceCollection();
}

public interface IServiceCollection
{
    void AddControllers();
    void AddEndpointsApiExplorer();
    void AddSwaggerGen();
    void AddSingleton<TService, TImplementation>() where TService : class where TImplementation : class, TService;
    void AddSingleton<TService>() where TService : class;
}

public class ServiceCollection : IServiceCollection
{
    public void AddControllers() { /* implementation */ }
    public void AddEndpointsApiExplorer() { /* implementation */ }
    public void AddSwaggerGen() { /* implementation */ }
    public void AddSingleton<TService, TImplementation>() where TService : class where TImplementation : class, TService { /* implementation */ }
    public void AddSingleton<TService>() where TService : class { /* implementation */ }
}