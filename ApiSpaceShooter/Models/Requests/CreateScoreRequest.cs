namespace ApiSpaceShooter.Models.Requests;

public record CreateScoreRequest(
    string Alias, 
    int Points, 
    int? MaxCombo, 
    int? DurationSec, 
    string? Metadata);