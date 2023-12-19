using Microsoft.EntityFrameworkCore;
using Prometheus;
using TrampolineCenterAPI.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("ClientsDb"));

var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// METRICS
app.UseHttpMetrics();

app.UseAuthorization();

// METRICS
app.MapMetrics();
app.MapControllers();

app.Run();
