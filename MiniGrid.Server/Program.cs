using MiniGrid.Server.Hubs;
using MiniGrid.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSignalR();

// Create a single instance of GameManager that will be shared across all services
var gameManager = new GameManager();
builder.Services.AddSingleton<GameManager>(gameManager);
builder.Services.AddHostedService<GameLoopService>();

// Configure CORS for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials()
               .WithOrigins("http://localhost:3000")
        .SetPreflightMaxAge(TimeSpan.FromSeconds(3600));

    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.MapHub<GameHub>("/gameHub");
app.MapGet("/", async app =>
{
    await app.Response.WriteAsync("This is the backend");
});


app.Run();
