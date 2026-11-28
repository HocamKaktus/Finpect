using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Finpect.Core.Models;

namespace Finpect.Core.Data
{
    // CSV dosyasındaki şirket verilerini okuyup Company listesine çevirir
    public static class CsvCompanyLoader
    {
        public static List<Company> Load(string path)
        {
            List<Company> companies = new List<Company>();
            string[] lines = File.ReadAllLines(path);

            // ilk satır başlık olduğu için 1'den başlıyor
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];

                // boş satırları atla
                if (line.Trim() == "")
                    continue;

                string[] parts = line.Split(',');

                // eksik sütunlu satırları atla
                if (parts.Length < 15)
                    continue;

                Company company = new Company();
                company.Name = parts[0];
                company.Code = parts[1];
                company.Sector = parts[2];
                company.Price = ToDouble(parts[3]);
                company.EarningsPerShare = ToDouble(parts[4]);
                company.EarningsGrowthPercent = ToDouble(parts[5]);
                company.RevenueGrowthPercent = ToDouble(parts[6]);
                company.DebtToEquity = ToDouble(parts[7]);
                company.ReturnOnEquityPercent = ToDouble(parts[8]);
                company.NetMarginPercent = ToDouble(parts[9]);
                company.CurrentRatio = ToDouble(parts[10]);
                company.BookValuePerShare = ToDouble(parts[11]);
                company.PositiveFreeCashFlow = parts[12] == "true";
                company.ProfitableYears = int.Parse(parts[13]);
                company.DividendYears = int.Parse(parts[14]);

                companies.Add(company);
            }

            return companies;
        }

        // 0.30 gibi noktalı sayıları okumak için
        private static double ToDouble(string text)
        {
            return double.Parse(text, CultureInfo.InvariantCulture);
        }
    }
}
