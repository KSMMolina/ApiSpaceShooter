namespace ApiSpaceShooter.Application.DTOs;

public record ScoreStatistics(
    string Alias,
    int TotalGames,
    int BestScore,
    double AverageScore,
    int BestCombo,
    int ShortestDuration,
    DateTime FirstGameDate,
    DateTime LastGameDate);