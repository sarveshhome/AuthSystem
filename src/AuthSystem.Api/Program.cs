using System.Text;
using AuthSystem.Application.Interfaces;
using AuthSystem.Application.Services;
using AuthSystem.Core.Entities;
using AuthSystem.Core.Interfaces;
using AuthSystem.Infrastructure.Data;
using AuthSystem.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity; // Add this
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add DbContext
builder.Services.AddDbContext<AuthDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(
        connectionString,
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null
            );
        }
    );
});

// Configure JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
var issuer = jwtSettings["Issuer"];
var tokenValidityInMinutes = int.Parse(jwtSettings["TokenValidityInMinutes"]);
var refreshTokenValidityInDays = int.Parse(jwtSettings["RefreshTokenValidityInDays"]);

builder.Services.AddSingleton<IJwtService>(
    new JwtService(secretKey, issuer, tokenValidityInMinutes, refreshTokenValidityInDays)
);

/// Add CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowReactApp",
        policy =>
        {
            // Get CORS origins from configuration
            var corsOrigins =
                builder.Configuration["CORS:Origins"]?.Split(',')
                ?? builder.Configuration.GetSection("CORS:Origins").Get<string[]>()
                ?? Array.Empty<string>();

            policy.WithOrigins(corsOrigins).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
        }
    );
});

// Add Authentication
builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };
    });

// Add Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole(Role.Admin));
});

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();

// Program.cs
builder
    .Services.AddIdentity<User, IdentityRole>(options =>
    {
        // Configure identity options here
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;
    })
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});

builder
    .Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Security Headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; font-src 'self'; connect-src 'self'; frame-ancestors 'none';");
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    await next();
});

app.UseCors("AllowReactApp");

// In Program.cs
app.MapGet(
    "/info",
    () =>
        new
        {
            Version = System
                .Reflection.Assembly.GetExecutingAssembly()
                .GetName()
                .Version?.ToString(),
            Environment = app.Environment.EnvironmentName,
            DateTime = DateTime.UtcNow,
        }
);

// Configure endpoints
app.MapHealthChecks("/health");

// health check for database
app.MapHealthChecks(
    "/health/db",
    new HealthCheckOptions
    {
        Predicate = registration => registration.Tags.Contains("DefaultConnection"),
        ResponseWriter = async (context, report) =>
        {
            var result = report.Status == HealthStatus.Healthy ? "Healthy" : "Unhealthy";
            await context.Response.WriteAsync(result);
        },
    }
);

// health check for kafka
app.MapHealthChecks(
    "/health/kafka",
    new HealthCheckOptions
    {
        Predicate = registration => registration.Tags.Contains("kafka"),
        ResponseWriter = async (context, report) =>
        {
            var result = report.Status == HealthStatus.Healthy ? "Healthy" : "Unhealthy";
            await context.Response.WriteAsync(result);
        },
    }
);

//combine health check db and kafka and api

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
