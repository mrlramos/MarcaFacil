using MarcaFacilAPI.DataAccess;
using MarcaFacilAPI.DataAccess.Context;
using MarcaFacilAPI.Services.Logs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string? dataBaseConnection = builder.Configuration.GetConnectionString("DataBaseConnection");
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
builder.Services.AddDbContext<PostgreSqlContext>(options =>
options.UseNpgsql(dataBaseConnection, builder =>
{
    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
}));

builder.Services.AddScoped<UserRepository>();
////Adding Logs.txt as log file
builder.Services.AddSingleton<ILoggerProvider, FileLoggerProvider>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
