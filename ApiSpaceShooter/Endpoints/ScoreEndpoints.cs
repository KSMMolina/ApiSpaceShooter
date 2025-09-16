using ApiSpaceShooter.Application.UseCases;
using ApiSpaceShooter.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace ApiSpaceShooter.Endpoints;

public static class ScoreEndpoints
{
    public static IEndpointRouteBuilder MapScoreEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/scores")
            .WithTags("Scores");

        // POST /api/v1/scores - Crear puntaje
        group.MapPost("/", CreateScoreAsync)
            .WithName("CreateScore")
            .WithSummary("Crear nuevo puntaje")
            .WithDescription("Crea un nuevo puntaje para el juego Space Shooter")
            .Accepts<CreateScoreRequest>("application/json")
            .Produces<object>(201)
            .ProducesValidationProblem(400);

        // GET /api/v1/scores/top?limit=10 - Top N puntajes
        group.MapGet("/top", GetTopScoresAsync)
            .WithName("GetTopScores")
            .WithSummary("Obtener top puntajes")
            .WithDescription("Obtiene los mejores puntajes ordenados por puntos (DESC), duración (ASC), fecha (ASC)")
            .Produces<IReadOnlyList<ApiSpaceShooter.Domain.Entities.Score>>(200);

        // GET /api/v1/scores/alias/{alias} - Historial por alias
        group.MapGet("/alias/{alias}", GetScoresByAliasAsync)
            .WithName("GetScoresByAlias")
            .WithSummary("Obtener historial por alias")
            .WithDescription("Obtiene el historial de puntajes de un jugador específico")
            .Produces<IReadOnlyList<ApiSpaceShooter.Domain.Entities.Score>>(200)
            .ProducesValidationProblem(400);

        return endpoints;
    }

    private static async Task<IResult> CreateScoreAsync(
        CreateScoreRequest request, 
        CreateScore createScore, 
        CancellationToken ct)
    {
        try
        {
            var id = await createScore.Handle(
                request.Alias,
                request.Points,
                request.MaxCombo,
                request.DurationSec,
                request.Metadata,
                ct);

            return Results.Created($"/api/v1/scores/{id}", new { id });
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { error = ex.Message, type = "validation_error" });
        }
    }

    private static async Task<IResult> GetTopScoresAsync(
        [FromQuery] int limit = 10,
        GetTopScores getTopScores = null!,
        CancellationToken ct = default)
    {
        try
        {
            var scores = await getTopScores.Handle(limit, ct);
            return Results.Ok(scores);
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error al obtener los top puntajes: {ex.Message}");
        }
    }

    private static async Task<IResult> GetScoresByAliasAsync(
        string alias,
        GetScoresByAlias getScoresByAlias,
        CancellationToken ct)
    {
        try
        {
            var scores = await getScoresByAlias.Handle(alias, ct);
            return Results.Ok(scores);
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { error = ex.Message, type = "validation_error" });
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error al obtener puntajes del alias '{alias}': {ex.Message}");
        }
    }
}