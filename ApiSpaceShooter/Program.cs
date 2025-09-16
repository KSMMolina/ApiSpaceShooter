using ApiSpaceShooter.Configuration;
using ApiSpaceShooter.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Configurar servicios
builder.Services
    .AddDatabase(builder.Configuration)
    .AddApplicationServices()
    .AddCorsConfiguration(builder.Configuration) // Pasar configuration
    .AddApiDocumentation()
    .AddErrorHandling();

var app = builder.Build();

// Configurar pipeline (orden correcto de middlewares)
app.ConfigurePipeline();

// Mapear endpoints
app.MapScoreEndpoints();

// Inicializar base de datos
await app.InitializeDatabaseAsync();

// Log de inicio
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Space Shooter API iniciada correctamente en {Environment}", app.Environment.EnvironmentName);
logger.LogInformation("CORS configurado para desarrollo con política permisiva");

app.Run();