namespace RockPaperScissorsLizardSpockGame.Domain.Models;

public class ScoreEntry
{
    public int Id { get; set; }
    public string PlayerEmail { get; set; } = string.Empty;
    public GameMove PlayerMove { get; set; }
    public GameMove ComputerMove { get; set; }
    public string Result { get; set; } = string.Empty;
    public DateTime PlayedAt { get; set; }
}