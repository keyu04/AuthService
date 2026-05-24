using System.Text;
using AuthMicroService.Common.Logger;
using AuthMicroService.Common.Settings;
using AuthMicroService.Repositories.Implementations;
using AuthMicroService.Repositories.Interfaces;
using AuthMicroService.Services.Implementations;
using AuthMicroService.Services.Interfaces;
using JobPortalAPI.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
builder.Services.AddOpenApi();
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
            Array.Empty<string>()
        }
    });

});


builder.Services.AddControllers();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepositories, AuthRepositories>();


builder.Logging.ClearProviders();

builder.Logging.AddConsole(options =>
{
    options.FormatterName = CustomConsoleLogger.FormatterName;
});

builder.Logging.AddConsoleFormatter<CustomConsoleLogger, ConsoleFormatterOptions>();

// Optional: also log to Debug window (Visual Studio)
builder.Logging.AddDebug();

var app = builder.Build();

app.UseMiddleware<GlobalExeptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();