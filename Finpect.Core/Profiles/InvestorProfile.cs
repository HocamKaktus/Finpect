using System.Collections.Generic;
using Finpect.Core.Models;

namespace Finpect.Core.Profiles
{
    // Ortak şablon: bütün yatırımcı profilleri bu sınıftan türer.
    public abstract class InvestorProfile
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract List<CriterionResult> Evaluate(Company company);

        // Kod tekrarını azaltmak için ortak yardımcı metot (protected).
        protected CriterionResult Rule(string title, bool passed, string detail)
        {
            return new CriterionResult(title, passed, detail);
        }

        protected CriterionResult Unknown(string title, string detail)
        {
            return new CriterionResult(title, null, detail);
        }
    }
}
