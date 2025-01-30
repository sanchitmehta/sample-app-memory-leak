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

// Scoped Fixes for Memory Leaks
app.MapControllers();

// Ensure objects that require disposal are properly handled
app.Run(async () =>
{
    // Wrap application in a using block to ensure disposables are disposed.
    // Note: HttpClient usage must be handled carefully. If used in controllers, consider using IHttpClientFactory.
    await using (app) 
    {
        // Place necessary application logic here if any.
    }
});

app.Dispose();

