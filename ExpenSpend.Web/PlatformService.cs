﻿using System;
using System.Text;
using System.Text.Json.Serialization;
using ExpenSpend.Domain.DTOs.Emails;
using ExpenSpend.Data.Context;
using ExpenSpend.Domain.Models.Users;
using ExpenSpend.Repository.Contracts;
using ExpenSpend.Repository.Implementations;
using ExpenSpend.Service;
using ExpenSpend.Service.Contracts;
using ExpenSpend.Service.Emails;
using ExpenSpend.Service.Emails.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using ExpenSpend.Domain.DTOs.Accounts;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ExpenSpend.Web;

public static class PlatformService
{
    public static void AddDbContextConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("AppDbContext") ?? throw new InvalidOperationException("Connection string 'AppDbContext' not found."));
        });
    }

    public static void AddControllerConfig(this IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
        });
    }
    public static void AddIdentityConfig(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options => options.SignIn.RequireConfirmedEmail = true)
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
    }

    public static void AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                builder
                .WithOrigins(configuration["Cors:AllowedOrigins"]!.Split(","))
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
            });
        });
    }

    public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JWT").Get<JwtSettingsDto>()!;

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
      .AddJwtBearer("Bearer", options =>
      {
          options.SaveToken = true;
          options.RequireHttpsMetadata = false;
          options.TokenValidationParameters = new TokenValidationParameters
          {
              ValidateIssuerSigningKey = true,
              ValidateIssuer = true,
              ValidateAudience = true,
              ValidAudience = jwtSettings.Audience,
              ValidIssuer = jwtSettings.Issuer,
              IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
          };
      }).AddJwtBearer("Auth0", options =>
      {
          options.Authority = $"https://{configuration["Auth0:Domain"]}/";
          options.Audience = configuration["Auth0:Audience"];
          options.TokenValidationParameters = new TokenValidationParameters
          {
              NameClaimType = ClaimTypes.NameIdentifier
          };
      }).AddMicrosoftIdentityWebApi(configuration.GetSection("AzureAd"), jwtBearerScheme: "AzureAd");
    }

    public static void AddJwtAuthorizationPolicy(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser().AddAuthenticationSchemes("Bearer", "Auth0").Build();
        });
    }

    public static void AddEmailService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration.GetSection("EmailConfig").Get<EmailConfigurationDto>()!);
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(EntityFrameworkRepository<>));
        services.AddScoped<IAuthAppService, AuthAppService>();
        services.AddScoped<IUserAppService, UserAppService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IFriendAppService, FriendAppService>();
        services.AddScoped<IGroupAppService, GroupAppService>();
        services.AddScoped<IGroupMemberAppService, GroupMemberAppService>();
        services.AddScoped<IAuth0Service, Auth0Service>();
        services.AddHttpContextAccessor();
    }

    public static void AddSwaggerConfig(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "ExpenSpend API", Version = "v1" });
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter your JWT token into the textbox below",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });
        });
    }
    public static void ApplyMigrations(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var serviceProvider = scope.ServiceProvider;
            try
            {
                var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying migrations: {ex.Message}");
            }
            // Seed Database
            ExpenSpendDbInitializer.Seed(app);
        }
    }
}