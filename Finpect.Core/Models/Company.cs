namespace Finpect.Core.Models
{
    // Finansal veriler bu sınıfta tutulur.
    // İleride Alp'in CSV veya SQLite verileri bu sınıfa dönüştürülebilir.
    public class Company
    {
        public string Name { get; set; } = "";
        public string Code { get; set; } = "";
        public string Sector { get; set; } = "";
        public double Price { get; set; }
        public double EarningsPerShare { get; set; }
        public double EarningsGrowthPercent { get; set; }
        public double RevenueGrowthPercent { get; set; }
        public double DebtToEquity { get; set; }
        public double ReturnOnEquityPercent { get; set; }
        public double NetMarginPercent { get; set; }
        public double CurrentRatio { get; set; }
        public double BookValuePerShare { get; set; }
        public bool PositiveFreeCashFlow { get; set; }
        public int ProfitableYears { get; set; }
        public int DividendYears { get; set; }

        public string DisplayName => Name + " (" + Code + ")";

        // EPS sıfır veya negatifse F/K anlamlı değildir.
        public double? PriceToEarnings
        {
            get
            {
                if (EarningsPerShare <= 0) return null;
                return Price / EarningsPerShare;
            }
        }

        // Kitaplarda geçen PEG için örnek: F/K / yıllık kâr büyümesi (%).
        public double? PegRatio
        {
            get
            {
                if (PriceToEarnings == null || EarningsGrowthPercent <= 0) return null;
                return PriceToEarnings.Value / EarningsGrowthPercent;
            }
        }

        public double? PriceToBook
        {
            get
            {
                if (BookValuePerShare <= 0) return null;
                return Price / BookValuePerShare;
            }
        }
    }
}
