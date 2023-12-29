using Microsoft.EntityFrameworkCore;
using Prometheus;
using TrampolineCenterAPI.Data;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Grafana.Loki;

// Add cfg
var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add logging 
builder.Logging.ClearProviders();
var logger = new LoggerConfiguration().ReadFrom.Configuration(config).CreateLogger();
builder.Logging.AddSerilog(logger);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("ClientsDb"));

// Добавление поддержки CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder => 
        builder
            .SetIsOriginAllowed(_ => true) // Разрешить запросы с любого источника
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }


app.UseSwagger();
app.UseSwaggerUI();

// app.UseHttpsRedirection();

app.UseRouting();
// METRICS
app.UseHttpMetrics();

// Добавление middleware для обработки CORS перед middleware для авторизации
app.UseCors();

app.UseAuthorization();

app.MapControllers();
// METRICS
app.MapMetrics();

app.Run();
