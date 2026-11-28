using System;
using System.Collections.Generic;
using Finpect.Core.Models;

namespace Finpect.Core.Profiles
{
    // Buffett'ın kalite ve finansal sağlamlık yaklaşımından esinlenen
    // 6 BASİT EĞİTİM KRİTERİ. Eşikler Buffett'ın resmi kuralları DEĞİLDİR.
    public class BuffettProfile : InvestorProfile
    {
        public override string Name => "Warren Buffett";
        public override string Description => "Kârlılık, nakit üretimi ve finansal sağlamlık";

        public override List<CriterionResult> Evaluate(Company c)
        {
            if (c == null) throw new ArgumentNullException(nameof(c));
            List<CriterionResult> results = new List<CriterionResult>();

            results.Add(Rule("Özkaynak kârlılığı (ROE) >= %15",
                c.ReturnOnEquityPercent >= 15,
                "ROE: %" + c.ReturnOnEquityPercent.ToString("0.##")));

            results.Add(Rule("Borç / özkaynak <= 0,70",
                c.DebtToEquity <= 0.70,
                "Borç / özkaynak: " + c.DebtToEquity.ToString("0.##")));

            results.Add(Rule("Net kâr marjı >= %12",
                c.NetMarginPercent >= 12,
                "Net kâr marjı: %" + c.NetMarginPercent.ToString("0.##")));

            results.Add(Rule("Pozitif serbest nakit akışı",
                c.PositiveFreeCashFlow,
                "Serbest nakit akışı: " + (c.PositiveFreeCashFlow ? "Pozitif" : "Pozitif değil")));

            // EPS pozitif değilse F/K hesaplanamaz; bu koşul "veri yok" olur.
            if (c.PriceToEarnings.HasValue)
                results.Add(Rule("F/K <= 25",
                    c.PriceToEarnings.Value <= 25,
                    "F/K: " + c.PriceToEarnings.Value.ToString("0.##")));
            else
                results.Add(Unknown("F/K <= 25", "EPS pozitif olmadığı için F/K hesaplanamadı."));

            results.Add(Rule("En az 5 yıl kârlılık",
                c.ProfitableYears >= 5,
                "Kârlı yıl: " + c.ProfitableYears));

            return results;
        }
    }
}
