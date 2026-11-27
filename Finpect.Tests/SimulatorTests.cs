using Finpect.Core.Risk;
using Xunit;

namespace Finpect.Tests;


public class SimulatorTests
{
    [Fact]
    public void Medium_PriceUp_And_Down_AreCorrect()
    {
        var sim = new Simulator();

        var results = sim.Run(10000m, 50m, RiskLevel.Medium, new[] {60m, 40m});

        Assert.Equal(2, results.Count);
        Assert.Equal(20,results[0].Shares);
        Assert.Equal(200m, results[0].ProfitLoss);
        Assert.Equal(0.2m, results[0].ChangePercent);
        Assert.Equal(-200m, results[1].ProfitLoss);
        Assert.Equal(-0.2m, results[1].ChangePercent);
    }
}