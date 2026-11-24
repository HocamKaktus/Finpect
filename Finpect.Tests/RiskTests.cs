using Finpect.Core.Risk;
using Xunit;

namespace Finpect.Tests;

public class RiskTests
{
    private readonly PositionCalculator _calc = new();

    [Fact]
    public void Medium_InvestmentAmount_IsTenPercentOfCapital()
    {
        var profile = RiskProfile.For(RiskLevel.Medium);
        Assert.Equal(1000m, _calc.GetInvestmentAmount(10000m, profile));
    }

    [Fact]
    public void ShareCount_RoundsDown()
    {
        Assert.Equal(20, _calc.GetShareCount(1000m, 50m));
        Assert.Equal(19, _calc.GetShareCount(1000m, 52m));
    }

    [Fact]
    public void RiskRewardRatio_IsTwo_ForMedium()
    {
        Assert.Equal(2m, RiskProfile.For(RiskLevel.Medium).RiskRewardRatio);
    }
    
    [Fact]
    public void ZeroPrice_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(    
            () => _calc.GetShareCount(1000m, 0m));
    }
}