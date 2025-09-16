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
            ?? "Server=localhost,1433;Database=SpaceShooter;User Id=sa;Password=Your_password123;TrustServerCertificate=True";

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
        // Repository Pattern
        services.AddScoped<IScoreRepository, ScoreRepository>();
        
        // Unit of Work Pattern
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // Use Cases
        services.AddScoped<CreateScore>();
        services.AddScoped<GetTopScores>();
        services.AddScoped<GetScoresByAlias>();

        return services;
    }

    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("http://localhost:4200")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
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
                Description = "API para gestión de puntajes del juego Space Shooter con arquitectura hexagonal"
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