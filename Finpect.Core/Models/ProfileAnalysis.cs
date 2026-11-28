using System.Collections.Generic;

namespace Finpect.Core.Models
{
    // Tek bir yatırımcı profili için puan, yorum ve kriter sonuçlarını birlikte taşır.
    public class ProfileAnalysis
    {
        public string ProfileName { get; }
        public string ProfileDescription { get; }
        public int Score { get; }
        public string Comment { get; }
        public List<CriterionResult> Criteria { get; }

        public ProfileAnalysis(string name, string description, int score,
            string comment, List<CriterionResult> criteria)
        {
            ProfileName = name;
            ProfileDescription = description;
            Score = score;
            Comment = comment;
            Criteria = criteria;
        }
    }
}
