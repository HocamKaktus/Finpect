using System.Collections.Generic;
using Finpect.Core.Models;

namespace Finpect.Core.Data
{
    public static class DemoCompanies
    {
        // TAMAMI KURGUSALDIR. Gerçek şirket, sembol veya piyasa verisi değildir.
        // Alp'in veri modülü hazır olunca bu liste gerçek veri aktarımıyla değiştirilir.
        public static List<Company> GetAll()
        {
            return new List<Company>
            {
                new Company { Name = "Arven Yazılım", Code = "ORNEK-ARV", Sector = "Teknoloji",
                    Price = 120, EarningsPerShare = 6, EarningsGrowthPercent = 22,
                    RevenueGrowthPercent = 18, DebtToEquity = 0.30,
                    ReturnOnEquityPercent = 21, NetMarginPercent = 19,
                    CurrentRatio = 2.5, BookValuePerShare = 42,
                    PositiveFreeCashFlow = true, ProfitableYears = 6, DividendYears = 0 },
                new Company { Name = "Mera Sanayi", Code = "ORNEK-MER", Sector = "Sanayi",
                    Price = 75, EarningsPerShare = 5, EarningsGrowthPercent = 12,
                    RevenueGrowthPercent = 8, DebtToEquity = 0.40,
                    ReturnOnEquityPercent = 16, NetMarginPercent = 14,
                    CurrentRatio = 2.2, BookValuePerShare = 55,
                    PositiveFreeCashFlow = true, ProfitableYears = 9, DividendYears = 7 },
                new Company { Name = "Kuzey Enerji", Code = "ORNEK-KZE", Sector = "Enerji",
                    Price = 90, EarningsPerShare = 3, EarningsGrowthPercent = 8,
                    RevenueGrowthPercent = 13, DebtToEquity = 1.30,
                    ReturnOnEquityPercent = 12, NetMarginPercent = 6,
                    CurrentRatio = 1.1, BookValuePerShare = 35,
                    PositiveFreeCashFlow = false, ProfitableYears = 3, DividendYears = 0 },
                new Company { Name = "Duru Tüketim", Code = "ORNEK-DUR", Sector = "Tüketim",
                    Price = 60, EarningsPerShare = 4, EarningsGrowthPercent = 10,
                    RevenueGrowthPercent = 11, DebtToEquity = 0.45,
                    ReturnOnEquityPercent = 18, NetMarginPercent = 12,
                    CurrentRatio = 2.1, BookValuePerShare = 40,
                    PositiveFreeCashFlow = true, ProfitableYears = 7, DividendYears = 6 },
                new Company { Name = "Nova Robotik", Code = "ORNEK-NVR", Sector = "Teknoloji",
                    Price = 230, EarningsPerShare = 4, EarningsGrowthPercent = 38,
                    RevenueGrowthPercent = 45, DebtToEquity = 0.65,
                    ReturnOnEquityPercent = 17, NetMarginPercent = 10,
                    CurrentRatio = 1.8, BookValuePerShare = 50,
                    PositiveFreeCashFlow = false, ProfitableYears = 2, DividendYears = 0 },
                new Company { Name = "Liman Lojistik", Code = "ORNEK-LML", Sector = "Lojistik",
                    Price = 40, EarningsPerShare = 4, EarningsGrowthPercent = 4,
                    RevenueGrowthPercent = 3, DebtToEquity = 0.20,
                    ReturnOnEquityPercent = 13, NetMarginPercent = 11,
                    CurrentRatio = 2.4, BookValuePerShare = 35,
                    PositiveFreeCashFlow = true, ProfitableYears = 10, DividendYears = 8 },
                new Company { Name = "Temel Biyotek", Code = "ORNEK-TMB", Sector = "Sağlık",
                    Price = 55, EarningsPerShare = -2, EarningsGrowthPercent = 0,
                    RevenueGrowthPercent = 22, DebtToEquity = 0.80,
                    ReturnOnEquityPercent = -9, NetMarginPercent = -8,
                    CurrentRatio = 1.9, BookValuePerShare = 18,
                    PositiveFreeCashFlow = false, ProfitableYears = 0, DividendYears = 0 }
            };
        }
    }
}
