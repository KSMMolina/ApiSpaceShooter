using ApiSpaceShooter.Configuration;
using ApiSpaceShooter.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Configurar servicios
builder.Services
    .AddDatabase(builder.Configuration)
    .AddApplicationServices()
    .AddCorsConfiguration()
    .AddApiDocumentation()
    .AddErrorHandling();

var app = builder.Build();

// Configurar pipeline
app.ConfigurePipeline();

// Mapear endpoints
app.MapScoreEndpoints();

// Inicializar base de datos
await app.InitializeDatabaseAsync();

// Log de inicio
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Space Shooter API iniciada correctamente en {Environment}", app.Environment.EnvironmentName);

app.Run();