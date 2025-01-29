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
    else
    {
        if (serviceProvider is IServiceScopeFactory scopeFactory)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                foreach (var service in scope.ServiceProvider.GetServices<IDisposable>())
                {
                    service.Dispose();
                }
            }
        }
    }
}