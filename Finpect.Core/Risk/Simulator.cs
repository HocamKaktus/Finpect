namespace Finpect.Core.Risk;


public class Simulator
{
    private readonly PositionCalculator _calc = new();

    public IReadOnlyList<ScenarioResult> Run(
        decimal capital,
        decimal buyPrice,
        RiskLevel level,
        IEnumerable<decimal> scenarioPrices)
    {
        var profile = RiskProfile.For(level);
        var amount = _calc.GetInvestmentAmount(capital,profile);
        var shares = _calc.GetShareCount(amount,buyPrice);

        return scenarioPrices
            .Select(price => new ScenarioResult(
                price,
                shares,
                shares * (price - buyPrice),
                (price - buyPrice) / buyPrice))
            .ToList();
    }
}