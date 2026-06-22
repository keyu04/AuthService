using System.Text;
using Asp.Versioning;
using AuthMicroService.Common.Logger;
using AuthMicroService.Common.Settings;
using AuthMicroService.Repositories.Implementations;
using AuthMicroService.Repositories.Interfaces;
using AuthMicroService.Services.Implementations;
using AuthMicroService.Services.Interfaces;
using JobPortalAPI.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


Settings.JwtSecreteKey = builder.Configuration.GetValue<string>("Jwt:Key");
Settings.JwtIssuer = builder.Configuration.GetValue<string>("Jwt:Issuer");
Settings.JwtAudience = builder.Configuration.GetValue<string>("Jwt:Audience");
GoogleSetting.ClientId = builder.Configuration.GetValue<string>("Google:ClientId");

builder.Services.AddDbContext<AppDbContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Settings.JwtIssuer,
        ValidAudience = Settings.JwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Settings.JwtSecreteKey!))
    };
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AuthService",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });

});


builder.Services.AddControllers();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepositories, AuthRepositories>();

// ── API Versioning ────────────────────────────────────────────────
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddMvc()
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddRateLimiter(options =>
{
    // ── Very strict — login/register are the highest-risk endpoints in your entire system ──
    options.AddFixedWindowLimiter("auth-strict", config =>
    {
        config.Window = TimeSpan.FromMinutes(1);
        config.PermitLimit = 5;     // ← only 5 login attempts per minute per IP
        config.QueueLimit = 0;
    });

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        context.HttpContext.Response.ContentType = "application/json";

        await context.HttpContext.Response.WriteAsync("""
        {
            "status": false,
            "Message": "Too many attempts. Please try again in a minute.",
            "Code": "AUTH_SERVICE_429"
        }
        """, cancellationToken);
    };
});

builder.Logging.ClearProviders();

builder.Logging.AddConsole(options =>
{
    options.FormatterName = CustomConsoleLogger.FormatterName;
});

builder.Logging.AddConsoleFormatter<CustomConsoleLogger, ConsoleFormatterOptions>();

// Optional: also log to Debug window (Visual Studio)
builder.Logging.AddDebug();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthService API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseRateLimiter();
app.MapControllers();

app.Run();