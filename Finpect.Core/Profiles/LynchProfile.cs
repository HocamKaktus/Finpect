using System;
using System.Collections.Generic;
using Finpect.Core.Models;

namespace Finpect.Core.Profiles
{
    // Lynch'in büyüme ve değerleme yaklaşımından esinlenen 6 eğitim kriteri.
    // Sınırlar proje örneğidir; Lynch'in kesin al/sat kuralları değildir.
    public class LynchProfile : InvestorProfile
    {
        public override string Name => "Peter Lynch";
        public override string Description => "Büyüme ile şirket değerlemesinin karşılaştırılması";

        public override List<CriterionResult> Evaluate(Company c)
        {
            if (c == null) throw new ArgumentNullException(nameof(c));
            List<CriterionResult> results = new List<CriterionResult>();

            results.Add(Rule("Kâr büyümesi %10–30 arası",
                c.EarningsGrowthPercent >= 10 && c.EarningsGrowthPercent <= 30,
                "Kâr büyümesi: %" + c.EarningsGrowthPercent.ToString("0.##")));

            // PEG, F/K oranını kâr büyümesiyle karşılaştırır. Hesaplanamıyorsa kriter boş kalır.
            if (c.PegRatio.HasValue)
                results.Add(Rule("PEG <= 1,5",
                    c.PegRatio.Value <= 1.5,
                    "PEG: " + c.PegRatio.Value.ToString("0.##")));
            else
                results.Add(Unknown("PEG <= 1,5", "Pozitif F/K ve büyüme olmadığı için PEG hesaplanamadı."));

            results.Add(Rule("Ciro büyümesi >= %10",
                c.RevenueGrowthPercent >= 10,
                "Ciro büyümesi: %" + c.RevenueGrowthPercent.ToString("0.##")));

            results.Add(Rule("Borç / özkaynak <= 1,00",
                c.DebtToEquity <= 1.00,
                "Borç / özkaynak: " + c.DebtToEquity.ToString("0.##")));

            results.Add(Rule("Net kâr marjı >= %8",
                c.NetMarginPercent >= 8,
                "Net kâr marjı: %" + c.NetMarginPercent.ToString("0.##")));

            if (c.PriceToEarnings.HasValue)
                results.Add(Rule("F/K <= 30",
                    c.PriceToEarnings.Value <= 30,
                    "F/K: " + c.PriceToEarnings.Value.ToString("0.##")));
            else
                results.Add(Unknown("F/K <= 30", "EPS pozitif olmadığı için F/K hesaplanamadı."));

            return results;
        }
    }
}
