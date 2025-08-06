using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SQKLocalServe_1.Middleware;
using SQKLocalServe.Business.Services.Role;
using SQKLocalServe.DataAccess;
using Microsoft.EntityFrameworkCore;
using SQKLocalServe.Common.Logging;
using SQKLocalServe.Common.Config;
using Microsoft.Extensions.Options;
using SQKLocalServe.Business.Services.Auth;
using SQKLocalServe.Contract.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using SQKLocalServe.Business.Services;
using SQKLocalServe.Business.Services.Interfaces;
using SQKLocalServe.Contract.DTOs;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Add JWT configuration
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtConfig>>().Value);

// 1️⃣  JWT authentication ----------------------------------------------------
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer   = builder.Configuration["Jwt:Issuer"],       // e.g. "MyCompany"
            ValidAudience = builder.Configuration["Jwt:Audience"],     // e.g. "MyCompanyUsers"
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)) // place your secret in appsettings
        };
    });

builder.Services.AddAuthorization();

// 3️⃣  Swagger with the Bearer scheme ---------------------------------------
builder.Services.AddSwaggerGen();


// ✅ Register your DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Register services
builder.Services.AddScoped<IJwtService, JwtService>();

// Register validators
builder.Services.AddSingleton<ILogManager, NLogManager>();
// Ensure you are using the correct RegisterRequest type that matches your validator
// For example, if your validator is for a custom RegisterRequest, use that type:
builder.Services.AddScoped<IValidator<UserRegistrationDTO>, RegisterUserDtoValidator>();
// Or, if your validator is for a different DTO, register accordingly
builder.Services.AddScoped<IValidator<LoginRequestDTO>, LoginDtoValidator>();

//builder.Services.AddScoped<IPasswordHasher, RsaPasswordHasher>();
// Register services
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add custom middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.UseHttpsRedirection();

// Add authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
