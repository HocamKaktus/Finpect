namespace Finpect.Core.Risk;


public record RiskProfile(
    RiskLevel Level,
    decimal PositionLimit,
    decimal StopLoss,
    decimal TakeProfit)
{
    public decimal RiskRewardRatio => TakeProfit / StopLoss;

    public static RiskProfile For(RiskLevel level) => level switch
    {
        RiskLevel.Low    => new RiskProfile(level, 0.05m, 0.05m, 0.10m),
        RiskLevel.Medium => new RiskProfile(level, 0.10m, 0.10m, 0.20m),
        RiskLevel.High   => new RiskProfile(level, 0.20m, 0.20m, 0.40m),
        _ => throw new ArgumentOutOfRangeException(nameof(level))
    };
}