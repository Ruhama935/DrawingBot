using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Middleware;
using server.Repositories;
using server.Services;


var builder = WebApplication.CreateBuilder(args);

var apiKey = builder.Configuration["OpenRouter:ApiKey"];
var env = builder.Environment;

if (string.IsNullOrWhiteSpace(apiKey))
{
    if (env.IsDevelopment())
    {
        throw new Exception(
            "Missing API key. Please configure it in appsettings.Development.json"
        );
    }

    throw new Exception("Missing API key.");
}

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = false; 
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddHttpClient<IAiService, OpenRouterAiService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IDrawingRepository, DrawingRepository>();
builder.Services.AddScoped<IDrawingService, DrawingService>();
builder.Services.AddDbContext<DrawingDbContext>(options =>
    options.UseSqlite("Data Source=drawingbot.db"));

var app = builder.Build();

// Ensure database and tables are created (development/demo purpose)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DrawingDbContext>();
    db.Database.EnsureCreated();
}

Console.WriteLine(
    $"DB Path: {Path.GetFullPath("drawingbot.db")}"
);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowReact");

app.UseExceptionHandler();

app.UseAuthorization();

app.MapControllers();

app.UseAuthorization();

app.MapControllers();

app.Run();
