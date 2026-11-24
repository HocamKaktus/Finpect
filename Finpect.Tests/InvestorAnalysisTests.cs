using System;
using System.Linq;
using Finpect.Core.Data;
using Finpect.Core.Models;
using Finpect.Core.Services;
using Xunit;

namespace Finpect.Tests;

public class InvestorAnalysisTests
{
    private readonly InvestorAnalysisService _analyzer = new();

    [Fact]
    public void ThreeProfilesHaveSixCriteriaForEachDemoCompany()
    {
        foreach (Company company in DemoCompanies.GetAll())
        {
            var analyses = _analyzer.Compare(company);
            Assert.Equal(3, analyses.Count);
            Assert.All(analyses, a => Assert.Equal(6, a.Criteria.Count));
            Assert.All(analyses, a => Assert.InRange(a.Score, 0, 100));
        }
    }

    [Fact]
    public void MissingFinancialMultiplesAreNotTreatedAsPassing()
    {
        Company company = DemoCompanies.GetAll().Single(c => c.Code == "ORNEK-TMB");
        Assert.Null(company.PriceToEarnings);
        Assert.Null(company.PegRatio);
        Assert.Contains(_analyzer.Compare(company),
            a => a.Criteria.Any(c => !c.Passed.HasValue));
    }

    [Fact]
    public void ProfileNameAndInstanceProduceSameScore()
    {
        Company company = DemoCompanies.GetAll().First();
        var profile = _analyzer.Profiles[0];
        Assert.Equal(_analyzer.Analyze(company, profile).Score,
                     _analyzer.Analyze(company, profile.Name).Score);
    }

    [Fact]
    public void GrahamAllCriteriaPassScoreIs100()
    {
        Company company = new()
        {
            Name = "Test", Code = "TEST", Price = 10,
            EarningsPerShare = 1, BookValuePerShare = 10,
            CurrentRatio = 3, DebtToEquity = 0.1,
            ProfitableYears = 6, DividendYears = 6
        };
        Assert.Equal(100, _analyzer.Analyze(company, "Benjamin Graham").Score);
    }

    [Fact]
    public void ComparisonIsOrderedByScoreDescending()
    {
        var analyses = _analyzer.Compare(DemoCompanies.GetAll().First());
        Assert.Equal(analyses.Select(a => a.Score).OrderByDescending(s => s),
                     analyses.Select(a => a.Score));
    }
}
