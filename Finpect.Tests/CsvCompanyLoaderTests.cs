using System.IO;
using Finpect.Core.Data;
using Xunit;

namespace Finpect.Tests;

// xUnit testleri: CSV okuma, eksik/hatalı veri temizleme ve tekrar eden kayıtları doğrular.
public class CsvCompanyLoaderTests
{
    private const string Header =
        "Name,Code,Sector,Price,EarningsPerShare,EarningsGrowthPercent,RevenueGrowthPercent," +
        "DebtToEquity,ReturnOnEquityPercent,NetMarginPercent,CurrentRatio,BookValuePerShare," +
        "PositiveFreeCashFlow,ProfitableYears,DividendYears";

    private static CsvLoadResult Load(params string[] rows)
    {
        string csv = Header + "\n" + string.Join("\n", rows);
        return CsvCompanyLoader.Parse(new StringReader(csv));
    }

    [Fact]
    public void ValidRowsAreConvertedToCompanies()
    {
        var result = Load(
            "Arven Yazılım,ORNEK-ARV,Teknoloji,120,6,22,18,0.30,21,19,2.5,42,true,6,0",
            "Mera Sanayi,ORNEK-MER,Sanayi,75,5,12,8,0.40,16,14,2.2,55,true,9,7");

        Assert.Equal(2, result.Companies.Count);
        Assert.Empty(result.Warnings);

        var arven = result.Companies[0];
        Assert.Equal("ORNEK-ARV", arven.Code);
        Assert.Equal(120, arven.Price);
        Assert.Equal(0.30, arven.DebtToEquity, 3);
        Assert.True(arven.PositiveFreeCashFlow);
        Assert.Equal(20, arven.PriceToEarnings!.Value, 3);
        Assert.Equal(7, result.Companies[1].DividendYears);
    }

    [Fact]
    public void RowWithMissingPriceIsSkipped()
    {
        var result = Load(
            "Arven Yazılım,ORNEK-ARV,Teknoloji,,6,22,18,0.30,21,19,2.5,42,true,6,0",
            "Mera Sanayi,ORNEK-MER,Sanayi,75,5,12,8,0.40,16,14,2.2,55,true,9,7");

        Assert.Single(result.Companies);
        Assert.Equal("ORNEK-MER", result.Companies[0].Code);
        Assert.Single(result.Warnings);
    }

    [Fact]
    public void RowWithInvalidNumberIsSkipped()
    {
        var result = Load("Arven Yazılım,ORNEK-ARV,Teknoloji,120,6,abc,18,0.30,21,19,2.5,42,true,6,0");

        Assert.Empty(result.Companies);
        Assert.Contains("EarningsGrowthPercent", result.Warnings[0]);
    }

    [Fact]
    public void DuplicateCodeIsSkipped()
    {
        var result = Load(
            "Arven Yazılım,ORNEK-ARV,Teknoloji,120,6,22,18,0.30,21,19,2.5,42,true,6,0",
            "Arven Kopya,ORNEK-ARV,Teknoloji,100,5,20,15,0.30,20,18,2.4,40,true,5,0");

        Assert.Single(result.Companies);
        Assert.Contains("tekrar", result.Warnings[0]);
    }

    [Fact]
    public void EmptyOptionalFieldsDefaultToZero()
    {
        var result = Load("Arven Yazılım,ORNEK-ARV,,120,6,,,,,,,,,,");

        var company = Assert.Single(result.Companies);
        Assert.Equal(0, company.DebtToEquity);
        Assert.False(company.PositiveFreeCashFlow);
        Assert.Equal(0, company.ProfitableYears);
    }

    [Fact]
    public void QuotedNameWithCommaIsReadCorrectly()
    {
        var result = Load("\"Arven Yazılım, A.Ş.\",ORNEK-ARV,Teknoloji,120,6,22,18,0.30,21,19,2.5,42,true,6,0");

        Assert.Equal("Arven Yazılım, A.Ş.", Assert.Single(result.Companies).Name);
    }

    [Fact]
    public void MissingRequiredColumnThrows()
    {
        string csv = "Name,Code,Sector\nArven Yazılım,ORNEK-ARV,Teknoloji";

        Assert.Throws<InvalidDataException>(() => CsvCompanyLoader.Parse(new StringReader(csv)));
    }
}
