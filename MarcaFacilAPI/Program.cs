using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using MarcaFacilAPI.DataAccess;
using MarcaFacilAPI.DataAccess.Context;
using MarcaFacilAPI.Services.Logs;
using MarcaFacilAPI.Services.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Token
var key = Encoding.ASCII.GetBytes(builder.Configuration["TokenKey"]);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
builder.Services.AddSingleton<TokenService>();

//Database
string? dataBaseConnection = builder.Configuration.GetConnectionString("DataBaseConnection");
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
builder.Services.AddDbContext<PostgreSqlContext>(options =>
options.UseNpgsql(dataBaseConnection, builder =>
{
    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
}));
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<PlaceRepository>();
builder.Services.AddScoped<ItemRepository>();

//Logs
builder.Services.AddSingleton<ILoggerProvider, FileLoggerProvider>();

//Amazon and BucketS3
builder.Services.AddAWSService<IAmazonS3>(new AWSOptions
{
    Region = RegionEndpoint.USEast1,
    Credentials = new BasicAWSCredentials(
        builder.Configuration.GetSection("Amazon")["AccessKey"],
        builder.Configuration.GetSection("Amazon")["SecretKey"])
});


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// App
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); //This call needs come before UseAuthorization()
app.UseAuthorization();

app.MapControllers();

app.Run();
