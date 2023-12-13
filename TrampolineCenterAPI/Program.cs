using Microsoft.EntityFrameworkCore;
using Prometheus;
using TrampolineCenterAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<APIDbContext>(options => options.UseInMemoryDatabase("ClientsDb"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// METRICS
app.UseHttpMetrics();

app.UseAuthorization();

// METRICS
app.MapMetrics();
app.MapControllers();

app.Run();
