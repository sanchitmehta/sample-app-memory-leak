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

// Wrap the app run in a try-finally block to ensure cleanup
try
{
    app.Run();
}
finally
{
    // Ensure services implementing IDisposable are disposed properly.
    if (app is IAsyncDisposable asyncDisposableApp)
    {
        // Properly dispose of the app.
        asyncDisposableApp.DisposeAsync().AsTask().GetAwaiter().GetResult();
    }
    else if (app is IDisposable disposableApp)
    {
        disposableApp.Dispose();
    }
}

// Notes:
// - Ensure that `LeakyCache`, `EventManager`, `DataGenerator`, or `CPUTaskManager` are properly implementing `IDisposable` or `IAsyncDisposable` if they utilize unmanaged resources or require explicit cleanup.
// - Certain services (e.g., HTTP clients) that are disposed in their respective code should be enclosed in `using` statements where they're used if necessary.