namespace RockPaperScissorsLizardSpockGame.Application.Interfaces;

public interface IRandomNumberService
{
    Task<int> GetRandomNumber(CancellationToken ct);
}
