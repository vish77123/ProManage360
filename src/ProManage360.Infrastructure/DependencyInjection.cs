namespace ProManage360.Infrastructure;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ProManage360.Application.Common.Interfaces;
using ProManage360.Application.Common.Interfaces.Service;
using ProManage360.Infrastructure.Identity;
using ProManage360.Persistence;
using ProManage360.Infrastructure.Services;
using System.Text;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database Context
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            // Use PostgreSQL (recommended)
            //options.UseNpgsql(connectionString, npgsqlOptions =>
            //{
            //    npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            //    npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
            //});

            // OR use SQL Server
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                sqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
            });
        });

        // Register IApplicationDbContext
        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());


        // Identity Services
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IEmailService, EmailService>();
        // Other Services
        services.AddSingleton<IDateTime, DateTimeService>();

        // HTTP Context Accessor (required for CurrentUserService)
        services.AddHttpContextAccessor();

        // JWT Authentication
        var jwtSecret = configuration["JwtSettings:Secret"]
            ?? throw new InvalidOperationException("JWT Secret not configured in appsettings.json");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["JwtSettings:Issuer"],
                ValidAudience = configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                ClockSkew = TimeSpan.Zero // No tolerance for expired tokens
            };
        });

        return services;
    }
}