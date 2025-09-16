using ApiSpaceShooter.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiSpaceShooter.Configuration;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        // Configuración para desarrollo
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Space Shooter API v1");
                c.RoutePrefix = string.Empty;
            });
        }

        app.UseCors(); // Usar política por defecto

        app.UseRouting(); // Después de CORS

        app.UseExceptionHandler(); // Manejo de errores

        // Solo HTTPS en producción
        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }

        return app;
    }

    public static async Task<WebApplication> InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        
        try
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            
            // Crear base de datos si no existe
            await context.Database.EnsureCreatedAsync();
            logger.LogInformation("Base de datos verificada exitosamente");

            if (app.Environment.IsDevelopment())
            {
                await SeedTestDataAsync(context, logger);
            }
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Error al inicializar la base de datos");
            throw;
        }

        return app;
    }

    private static async Task SeedTestDataAsync(AppDbContext context, ILogger logger)
    {
        if (await context.Scores.AnyAsync())
        {
            logger.LogInformation("La base de datos ya contiene datos");
            return;
        }

        var testScores = new[]
        {
            new ApiSpaceShooter.Domain.Entities.Score
            {
                Alias = "player1",
                Points = 1500,
                MaxCombo = 25,
                DurationSec = 180,
                Metadata = "Primera partida de prueba",
                CreatedAt = DateTime.UtcNow.AddMinutes(-30)
            },
            new ApiSpaceShooter.Domain.Entities.Score
            {
                Alias = "gamer2",
                Points = 2000,
                MaxCombo = 40,
                DurationSec = 150,
                Metadata = "Partida increíble",
                CreatedAt = DateTime.UtcNow.AddMinutes(-15)
            },
            new ApiSpaceShooter.Domain.Entities.Score
            {
                Alias = "player1",
                Points = 1800,
                MaxCombo = 30,
                DurationSec = 160,
                Metadata = "Segunda partida",
                CreatedAt = DateTime.UtcNow.AddMinutes(-10)
            }
        };

        context.Scores.AddRange(testScores);
        await context.SaveChangesAsync();
        
        logger.LogInformation("Datos de prueba insertados: {Count} registros", testScores.Length);
    }
}