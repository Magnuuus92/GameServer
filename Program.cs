using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using GameServer.Services;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);
//SERVICES
builder.Services.AddSingleton<mongoService>();

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
        .AllowedAnyHeader()
        .AllowAnyMethod();
    });
});

//JWT AUTH
var jwtSecret = builder.Configuration["Jwt:Secret"]
?? throw new InvalidOperationException("JWT secret not configured.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationsParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ValidateIssuer= false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero, 
    };
});