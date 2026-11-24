namespace Finpect.Core.Risk;

public class PositionCalculator
{
    public decimal GetInvestmentAmount(decimal capital, RiskProfile profile)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capital);
        return capital * profile.PositionLimit;
    }
    
    public int GetShareCount(decimal amount, decimal price)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);
        return (int)Math.Floor(amount/price);
    }

    public decimal GetStopLossPrice(decimal buyPrice, RiskProfile profile)
        => buyPrice * (1 - profile.StopLoss);

    public decimal GetTakeProfitPrice(decimal buyPrice, RiskProfile profile)
        => buyPrice * (1 + profile.TakeProfit);
}