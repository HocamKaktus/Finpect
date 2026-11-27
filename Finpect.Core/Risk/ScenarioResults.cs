namespace Finpect.Core.Risk;


public record ScenarioResult(
    decimal ScenarioPrice,
    int Shares,
    decimal ProfitLoss,
    decimal ChangePercent);