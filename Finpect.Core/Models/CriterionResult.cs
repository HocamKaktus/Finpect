namespace Finpect.Core.Models
{
    // bool? = true: geçti, false: geçmedi, null: veri hesaplanamadı.
    public class CriterionResult
    {
        public string Title { get; }
        public bool? Passed { get; }
        public string Explanation { get; }

        public CriterionResult(string title, bool? passed, string explanation)
        {
            Title = title;
            Passed = passed;
            Explanation = explanation;
        }

        public string Status
        {
            get
            {
                if (Passed == true) return "Uygun";
                if (Passed == false) return "Uygun değil";
                return "Veri yok";
            }
        }
    }
}
