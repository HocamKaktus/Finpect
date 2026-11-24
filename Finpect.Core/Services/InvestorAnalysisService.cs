using System;
using System.Collections.Generic;
using System.Linq;
using Finpect.Core.Models;
using Finpect.Core.Profiles;

namespace Finpect.Core.Services
{
    // Bu servis, her yatırımcının kriterlerini aynı formülle puanlar.
    public class InvestorAnalysisService
    {
        public List<InvestorProfile> Profiles { get; }

        public InvestorAnalysisService()
        {
            Profiles = new List<InvestorProfile>
            {
                new BuffettProfile(), new LynchProfile(), new GrahamProfile()
            };
        }

        public ProfileAnalysis Analyze(Company company, InvestorProfile profile)
        {
            if (company == null) throw new ArgumentNullException(nameof(company));
            if (profile == null) throw new ArgumentNullException(nameof(profile));

            List<CriterionResult> criteria = profile.Evaluate(company);
            int usableCount = criteria.Count(x => x.Passed.HasValue);
            int passedCount = criteria.Count(x => x.Passed == true);

            // Eksik/hesaplanamayan kriterler paydaya eklenmez.
            int score = usableCount == 0
                ? 0
                : (int)Math.Round(passedCount * 100.0 / usableCount,
                    MidpointRounding.AwayFromZero);

            string comment;
            if (usableCount == 0)
                comment = "Değerlendirme için yeterli veri yok.";
            else if (score >= 80)
                comment = "Bu örnek kriterlerin çoğunu karşılıyor.";
            else if (score >= 50)
                comment = "Bazı kriterleri karşılıyor; eksikler incelenmeli.";
            else
                comment = "Bu profilin örnek kriterleriyle uyum düşük.";

            return new ProfileAnalysis(profile.Name, profile.Description,
                score, comment, criteria);
        }

        // Method overloading: Aynı işi, profil adı ile de çağırabiliriz.
        public ProfileAnalysis Analyze(Company company, string profileName)
        {
            if (string.IsNullOrWhiteSpace(profileName))
                throw new ArgumentException("Profil adı boş olamaz.", nameof(profileName));

            InvestorProfile? profile = Profiles.FirstOrDefault(p => p.Name == profileName);
            if (profile == null)
                throw new ArgumentException("Bilinmeyen profil: " + profileName);

            return Analyze(company, profile);
        }

        public List<ProfileAnalysis> Compare(Company company)
        {
            if (company == null) throw new ArgumentNullException(nameof(company));
            return Profiles.Select(p => Analyze(company, p))
                .OrderByDescending(a => a.Score)
                .ToList();
        }
    }
}
