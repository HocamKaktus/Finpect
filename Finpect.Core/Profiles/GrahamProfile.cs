using System;
using System.Collections.Generic;
using Finpect.Core.Models;

namespace Finpect.Core.Profiles
{
    // Graham'ın değerleme ve güvenlik marjı düşüncesinden esinlenen
    // 6 eğitim kriteri. Bunlar eksiksiz Graham analizi değildir.
    public class GrahamProfile : InvestorProfile
    {
        public override string Name => "Benjamin Graham";
        public override string Description => "Makul değerleme ve bilanço güvenliği";

        public override List<CriterionResult> Evaluate(Company c)
        {
            if (c == null) throw new ArgumentNullException(nameof(c));
            List<CriterionResult> results = new List<CriterionResult>();

            // Graham örneğinde önce değerleme oranlarını, sonra bilanço güvenliğini kontrol ediyoruz.
            if (c.PriceToEarnings.HasValue)
                results.Add(Rule("F/K <= 15",
                    c.PriceToEarnings.Value <= 15,
                    "F/K: " + c.PriceToEarnings.Value.ToString("0.##")));
            else
                results.Add(Unknown("F/K <= 15", "EPS pozitif olmadığı için F/K hesaplanamadı."));

            if (c.PriceToBook.HasValue)
                results.Add(Rule("PD/DD <= 1,5",
                    c.PriceToBook.Value <= 1.5,
                    "PD/DD: " + c.PriceToBook.Value.ToString("0.##")));
            else
                results.Add(Unknown("PD/DD <= 1,5", "Defter değeri pozitif değil; PD/DD hesaplanamadı."));

            results.Add(Rule("Cari oran >= 2,00",
                c.CurrentRatio >= 2.00,
                "Cari oran: " + c.CurrentRatio.ToString("0.##")));

            results.Add(Rule("Borç / özkaynak <= 0,50",
                c.DebtToEquity <= 0.50,
                "Borç / özkaynak: " + c.DebtToEquity.ToString("0.##")));

            results.Add(Rule("En az 5 yıl kârlılık",
                c.ProfitableYears >= 5,
                "Kârlı yıl: " + c.ProfitableYears));

            results.Add(Rule("En az 5 yıl temettü geçmişi",
                c.DividendYears >= 5,
                "Temettü yılı: " + c.DividendYears));

            return results;
        }
    }
}
