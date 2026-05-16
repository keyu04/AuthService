using System.Text;
using AuthMicroService.Common.Settings;
using AuthMicroService.Repositories.Implementations;
using AuthMicroService.Repositories.Interfaces;
using AuthMicroService.Services.Implementations;
using AuthMicroService.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token.\nExample: Bearer abc123"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddControllers();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepositories, AuthRepositories>();

var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
// }

// app.UseHttpsRedirection();
app.MapControllers();

app.Run();