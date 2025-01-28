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

using (app)
{
    app.Run();
}

public partial class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
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

            await using var appLifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
            var cts = new CancellationTokenSource();
            appLifetime.ApplicationStopping.Register(() => 
            {
                cts.Cancel();
            });

            await app.RunAsync(cts.Token);

            CleanupResources(app.Services);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unhandled exception: {ex}");
            throw;
        }
        finally
        {
            await DisposeAsync();
        }
    }

    private static void CleanupResources(IServiceProvider services)
    {
        if (services.GetService<IEventManager>() is IDisposable eventManager)
        {
            eventManager.Dispose();
        }

        if (services.GetService<ILeakyCache>() is IDisposable leakyCache)
        {
            leakyCache.Dispose();
        }

        if (services.GetService<DataGenerator>() is IDisposable dataGenerator)
        {
            dataGenerator.Dispose();
        }

        if (services.GetService<CPUTaskManager>() is IDisposable cpuTaskManager)
        {
            cpuTaskManager.Dispose();
        }
    }

    public static async ValueTask DisposeAsync()
    {
        await Task.CompletedTask;
    }
}