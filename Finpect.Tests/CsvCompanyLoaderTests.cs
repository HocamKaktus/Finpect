using System.IO;
using Finpect.Core.Data;
using Xunit;

namespace Finpect.Tests;

// CSV okuyucunun testleri
public class CsvCompanyLoaderTests
{
    private const string Header =
        "Name,Code,Sector,Price,EarningsPerShare,EarningsGrowthPercent,RevenueGrowthPercent," +
        "DebtToEquity,ReturnOnEquityPercent,NetMarginPercent,CurrentRatio,BookValuePerShare," +
        "PositiveFreeCashFlow,ProfitableYears,DividendYears";

    // test için geçici bir csv dosyası oluşturur
    private string CreateCsv(string rows)
    {
        string path = Path.GetTempFileName();
        File.WriteAllText(path, Header + "\n" + rows);
        return path;
    }

    [Fact]
    public void CompaniesAreReadFromCsv()
    {
        string path = CreateCsv(
            "Arven Yazılım,ORNEK-ARV,Teknoloji,120,6,22,18,0.30,21,19,2.5,42,true,6,0\n" +
            "Mera Sanayi,ORNEK-MER,Sanayi,75,5,12,8,0.40,16,14,2.2,55,true,9,7");

        var companies = CsvCompanyLoader.Load(path);

        Assert.Equal(2, companies.Count);
        Assert.Equal("Arven Yazılım", companies[0].Name);
        Assert.Equal(120, companies[0].Price);
        Assert.Equal(7, companies[1].DividendYears);
    }

    [Fact]
    public void DecimalNumbersAreReadCorrectly()
    {
        string path = CreateCsv("Arven Yazılım,ORNEK-ARV,Teknoloji,120,6,22,18,0.30,21,19,2.5,42,true,6,0");

        var companies = CsvCompanyLoader.Load(path);

        Assert.Equal(0.30, companies[0].DebtToEquity);
        Assert.Equal(2.5, companies[0].CurrentRatio);
    }

    [Fact]
    public void EmptyAndIncompleteLinesAreSkipped()
    {
        string path = CreateCsv(
            "Arven Yazılım,ORNEK-ARV,Teknoloji,120,6,22,18,0.30,21,19,2.5,42,true,6,0\n" +
            "\n" +
            "Eksik Satır,ORNEK-EKS,Sanayi");

        var companies = CsvCompanyLoader.Load(path);

        Assert.Single(companies);
    }
}
