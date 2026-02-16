using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;
using SalesSystem.Infrastructure;
using SalesSystem.Infrastructure.Data;
using SalesSystem.Api.Middleware;
using Serilog;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "SalesSystem")
    .WriteTo.Console(new RenderedCompactJsonFormatter())
    .WriteTo.File(
        new CompactJsonFormatter(),
        "logs/sales-system-.json",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddInfrastructure(builder.Configuration);

AsyncRetryPolicy<HttpResponseMessage> retryPolicy = Policy<HttpResponseMessage>
    .Handle<HttpRequestException>()
    .OrResult(msg => msg.IsSuccessStatusCode == false)
    .WaitAndRetryAsync(3, retryAttempt => 
        TimeSpan.FromMilliseconds(100 * Math.Pow(2, retryAttempt)),
        onRetry: (outcome, timespan, retryAttempt, context) =>
        {
            Log.Warning("Retry {RetryAttempt} after {Timespan}ms due to: {Error}",
                retryAttempt, timespan.TotalMilliseconds, outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString());
        });

builder.Services.AddHttpClient("resilientClient")
    .AddPolicyHandler(retryPolicy);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("SalesSystem-Secret-Key-2024-32-Chars-Long!")),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "API Sistema de Ventas",
        Description = "API REST para gestión de productos y ventas",
        Version = "v1",
        Contact = new()
        {
            Name = "Soporte",
            Email = "soporte@salesystem.com"
        }
    });
    
    c.AddSecurityDefinition("Bearer", new()
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Sistema de Ventas v1");
    });
}

// app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

Log.Information("Iniciando aplicacion SalesSystem");

app.Run();
