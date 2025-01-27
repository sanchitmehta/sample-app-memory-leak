using PerformanceIssues.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ILeakyCache, LeakyCache>();
builder.Services.AddSingleton<IEventManager, EventManager>();
builder.Services.AddSingleton<DataGenerator>();
builder.Services.AddSingleton<CPUTaskManager>();
builder.Services.AddSingleton<ILoggerFactory>(provider =>
{
    var factory = LoggerFactory.Create(loggingBuilder =>
    {
        loggingBuilder.SetMinimumLevel(LogLevel.Debug);
        loggingBuilder.AddConsole();
        loggingBuilder.AddDebug();
    });

    return factory;
});

var app = builder.Build();
// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Lifetime.ApplicationStopping.Register(() =>
{
    var leakyCache = app.Services.GetRequiredService<ILeakyCache>() as IDisposable;
    var eventManager = app.Services.GetRequiredService<IEventManager>() as IDisposable;
    var dataGenerator = app.Services.GetRequiredService<DataGenerator>() as IDisposable;
    var cpuTaskManager = app.Services.GetRequiredService<CPUTaskManager>() as IDisposable;

    leakyCache?.Dispose();
    eventManager?.Dispose();
    dataGenerator?.Dispose();
    cpuTaskManager?.Dispose();
    
    var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
    loggerFactory.Dispose();
});

app.Run();