using PerformanceIssues.Serivces;
using PerformanceIssues.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Singleton Services
builder.Services.AddSingleton<ILeakyCache, LeakyCache>();
builder.Services.AddSingleton<IEventManager, EventManager>();
builder.Services.AddSingleton<DataGenerator>();
builder.Services.AddSingleton<CPUTaskManager>();

var app = builder.Build();

// Configure the HTTP request pipeline
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

// Ensure the application disposes services properly upon shutdown
app.Lifetime.ApplicationStopping.Register(() =>
{
    // Dispose singleton services manually to prevent memory leaks
    if (app.Services.GetService<ILeakyCache>() is ILeakyCache leakyCache)
    {
        leakyCache.Dispose(); // Ensure IDisposable is implemented in LeakyCache
    }

    if (app.Services.GetService<IEventManager>() is IEventManager eventManager)
    {
        eventManager.Dispose(); // Ensure IDisposable is implemented in EventManager
    }

    if (app.Services.GetService<DataGenerator>() is DataGenerator dataGenerator)
    {
        dataGenerator.Dispose(); // Ensure IDisposable is implemented in DataGenerator
    }

    if (app.Services.GetService<CPUTaskManager>() is CPUTaskManager cpuTaskManager)
    {
        cpuTaskManager.Dispose(); // Ensure IDisposable is implemented in CPUTaskManager
    }
});

app.Run();