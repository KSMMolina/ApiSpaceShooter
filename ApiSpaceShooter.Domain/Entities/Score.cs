namespace ApiSpaceShooter.Domain.Entities
{
    public class Score
    {
        public int Id { get; set; }
        public string Alias { get; set; } = string.Empty;
        public int Points { get; set; }
        public int? MaxCombo { get; set; }
        public int? DurationSec { get; set; }
        public string? Metadata { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
