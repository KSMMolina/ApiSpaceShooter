using ApiSpaceShooter.Application.UseCases;
using ApiSpaceShooter.Application.Ports;
using ApiSpaceShooter.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiSpaceShooter.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SqlServer")
            ?? Environment.GetEnvironmentVariable("SQLSERVER_CONN")
            ?? "Server=DESKTOP-OK96HBJ\\SLQSERVER;Database=ApiPruebaIa;User Id=sa;Password=ofima;TrustServerCertificate=True;";

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
            options.EnableSensitiveDataLogging(false);
            options.EnableDetailedErrors(false);
        });

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IScoreRepository, ScoreRepository>();
        services.AddScoped<CreateScore>();
        services.AddScoped<GetTopScores>();
        services.AddScoped<GetScoresByAlias>();

        return services;
    }

    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            // Política específica para desarrollo
            options.AddPolicy("DevelopmentPolicy", policy =>
            {
                policy.WithOrigins(
                    "http://localhost:4200",    // Angular default
                    "https://localhost:4200",   // Angular HTTPS
                    "http://localhost:3000",    // React default
                    "https://localhost:3000",   // React HTTPS
                    "http://127.0.0.1:4200",   // IP local Angular
                    "http://127.0.0.1:3000"    // IP local React
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .SetPreflightMaxAge(TimeSpan.FromMinutes(5));
            });

            // Política específica para producción
            options.AddPolicy("ProductionPolicy", policy =>
            {
                var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins")
                    .Get<string[]>() ?? ["https://yourdomain.com"];
                
                policy.WithOrigins(allowedOrigins)
                    .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                    .WithHeaders("Content-Type", "Authorization", "X-Requested-With")
                    .AllowCredentials()
                    .SetPreflightMaxAge(TimeSpan.FromHours(1));
            });

            // Política por defecto más permisiva para desarrollo
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        return services;
    }

    public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new() { 
                Title = "Space Shooter API", 
                Version = "v1",
                Description = "API para gestión de puntajes del juego Space Shooter - Clean Architecture"
            });
            
            // Agregar configuración para CORS en Swagger
            c.AddServer(new() { Url = "https://localhost:7000", Description = "Desarrollo HTTPS" });
            c.AddServer(new() { Url = "http://localhost:5000", Description = "Desarrollo HTTP" });
        });

        return services;
    }

    public static IServiceCollection AddErrorHandling(this IServiceCollection services)
    {
        services.AddProblemDetails();
        return services;
    }
}