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

app.Run();

// Potential memory leak fixes and suggestions are scoped to findings:

// 1. System.Byte[] leaks may originate from improper usage of buffers or streams. Ensure to dispose of any unmanaged resources.
// 2. Scope HTTP-related issues to over-retention or improper disposal of HTTP client connections. Use HttpClientFactory.
// 3. Investigating excessive System.Stringinstances nearby." complete stream convince="#"><Job💡 Before cleaning validate. 