using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProManage360.API.Middleware;
using ProManage360.Application;
using ProManage360.Application.Common.Interfaces;
using ProManage360.Application.Common.Interfaces.Repository;
using ProManage360.Application.Common.Interfaces.Service;
using ProManage360.Application.Services;
using ProManage360.Application.Settings;
using ProManage360.Infrastructure;
using ProManage360.Infrastructure.Identity;
using ProManage360.Infrastructure.Services;
using ProManage360.Persistence;
using System;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
//builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<IRoleRepository, RoleRepository>();
//builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
//builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddScoped<IJwtTokenService, JwtService>();
//builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddScoped<DatabaseSeeder>();

builder.Services.AddInfrastructure(builder.Configuration);

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
//var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = jwtSettings.Issuer,
//        ValidAudience = jwtSettings.Audience,
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
//    };
//});

builder.Services.AddAuthorization();

builder.Services.AddAuthorization();

builder.Services.AddApplication();
builder.Services.AddScoped<IApplicationDbContext>(provider =>
    provider.GetRequiredService<ApplicationDbContext>());

var app = builder.Build();

// Seed database on application startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    //try
    //{
    //    var seeder = services.GetRequiredService<DatabaseSeeder>();
    //    await seeder.SeedAsync();
    //    logger.LogInformation("Database seeding completed successfully");
    //}
    //catch (Exception ex)
    //{
    //    logger.LogError(ex, "An error occurred while seeding the database");
    //    throw;  // Fail fast if seeding fails
    //}
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseGlobalExceptionHandler();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

