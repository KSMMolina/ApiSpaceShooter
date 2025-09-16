namespace ApiSpaceShooter.Models.Requests;

/// <summary>
/// Request para crear un nuevo puntaje en el juego Space Shooter
/// </summary>
public record CreateScoreRequest(
    string Alias,          // 3-30 caracteres, obligatorio
    int Points,            // >= 0, obligatorio  
    int? MaxCombo,         // >= 0, opcional
    int? DurationSec,      // >= 0, opcional
    string? Metadata       // <= 400 caracteres, opcional
);