using backend.Models;
using backend.Service;
using backend.XML;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ------------------ Add Services ------------------
builder.Services.AddScoped<FileService>();
builder.Services.AddSingleton<XMLService>();
builder.Services.AddSingleton<UrlStore>();
builder.Services.AddSingleton<NgrokService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<DeviceService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactApp",
        policy => policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod());
});
builder.Services.Configure<FilePaths>(builder.Configuration.GetSection("Paths"));

// ------------------ Configure SeriLog ------------------

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error()
    .WriteTo.File(
        path: "Logs/errors-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 14)
    .CreateLogger();

builder.Host.UseSerilog();

// ------------------ Build App ------------------
var app = builder.Build();

// ------------------ Global Exception Handling ------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var exception = context.Features
                .Get<IExceptionHandlerFeature>()?
                .Error;

            if (exception != null)
            {
                Log.Error(exception, "Unhandled exception");
            }

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("An unexpected error occurred.");
        });
    });
}

// ------------------ Swagger ------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ------------------ Middleware Pipeline ------------------

//app.UseHttpsRedirection();
app.UseCors("ReactApp");
app.UseAuthorization();
app.MapControllers();

// ------------------ Run App and Ngrok ------------------

// Start the server asynchronously
var serverTask = app.RunAsync("http://localhost:5000");

// Start ngrok in background **after server is listening**
var scope = app.Services.CreateScope();
var ngrok = scope.ServiceProvider.GetRequiredService<NgrokService>();
_ = Task.Run(async () =>
{
    await Task.Delay(2000); // wait 2 seconds for server to bind port
    await ngrok.StartAsync(localPort: 5000);
    Console.WriteLine($"Ngrok public URL: {ngrok.PublicUrl}");
});

// Keep app running
await serverTask;
