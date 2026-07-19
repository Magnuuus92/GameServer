
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using GameServer.Services;
using System.Security.Cryptography;
DotNetEnv.Env.Load(); //loads environment (.env)
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
?? throw new InvalidOperationException("JWT_SECRET not found in env.");
var mongoConn = Environment.GetEnvironmentVariable("MONGODB_CONNECTION")
?? throw new InvalidOperationException("MONGODB_CONNECTION not found in env.");

var builder = WebApplication.CreateBuilder(args);
builder.Configuration["Jwt:Secret"] = jwtSecret;
builder.Configuration["MongoDB:ConnectionString"] = mongoConn;
//SERVICES
builder.Services.AddSingleton<MongoService>();

builder.Services.AddSingleton<JwtService>();

builder.Services.AddControllers();
//CORS requests from frontend
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
?? ["http://localhost:5500"];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

//JWT AUTH



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero, 
    };
});
builder.Services.AddAuthorization();

//APP
var app = builder.Build();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

//Hlth check
app.MapGet("/", () => "GameProject API is running.");

app.Run();