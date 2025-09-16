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
            ?? "Server=DESKTOP-OK96HBJ\\SLQSERVER;Database=VideJuegoApi;User Id=sa;Password=ofima;TrustServerCertificate=True;";

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
            options.EnableSensitiveDataLogging(false);
            options.EnableDetailedErrors(true); // Habilitar para debug
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
            // Política permisiva para desarrollo
            options.AddPolicy("DevelopmentPolicy", policy =>
            {
                policy.WithOrigins(
                    "http://localhost:4200",
                    "https://localhost:4200",
                    "http://127.0.0.1:4200"
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
            });

            // Política por defecto
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
                Description = "API para gestión de puntajes del juego Space Shooter"
            });
        });

        return services;
    }

    public static IServiceCollection AddErrorHandling(this IServiceCollection services)
    {
        services.AddProblemDetails();
        return services;
    }
}