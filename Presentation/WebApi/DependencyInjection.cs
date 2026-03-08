using System.Text;
using Domain.Contracts.Identity;
using Domain.Contracts.Infrastructure.Services.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApi.Authorization;
using WebApi.Middlewares;
using WebApi.Swagger;

namespace WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddWebApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddSignalR();
        
        // Configurar Swagger con autenticación JWT
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "ERPManifestoAPI",
                Version = "v1",
                Description = "API de gestión de restaurantes construida con Domain-Driven Design en .NET 10"
            });

            // Configurar el esquema de seguridad JWT
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Ingrese el token JWT en el formato: Bearer {token}"
            });

            // Aplicar el esquema de seguridad globalmente
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

            // Agregar soporte para IFormFile en Swagger
            options.OperationFilter<FileUploadOperationFilter>();
        });

        services.AddScoped<IUserCurrentSession, UserCurrentSession>();
        services.AddScoped<GlobalExceptionHandlingMiddleware>();

        // Configurar autenticación JWT
        var authConfig = configuration.GetSection(TokenConfiguration.SectionName).Get<TokenConfiguration>()!;
        
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authConfig.SecretKey)),
                ValidateIssuer = true,
                ValidIssuer = authConfig.Issuer,
                ValidateAudience = true,
                ValidAudience = authConfig.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            // Permitir que SignalR reciba el token desde query string
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;

                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };
        });

        // Configurar autorización basada en permisos
        services.AddAuthorization();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

        return services;
    }
}
