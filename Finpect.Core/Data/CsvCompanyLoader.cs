using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using Finpect.Core.Models;

namespace Finpect.Core.Data
{
    // CSV'den okunan şirketleri ve atlanan satırların nedenlerini birlikte tutar.
    public class CsvLoadResult
    {
        public List<Company> Companies { get; } = new List<Company>();
        public List<string> Warnings { get; } = new List<string>();
    }

    // Şirket verilerini CSV dosyasından okuyup Company nesnelerine dönüştürür.
    // Beklenen biçim: ilk satır başlık, ayraç virgül, ondalık ayracı nokta (örn. 2.5).
    // Sütunlar başlık adına göre bulunur; sıraları önemli değildir.
    public static class CsvCompanyLoader
    {
        // Bu sütunlar olmadan şirket anlamlı değerlendirilemez.
        private static readonly string[] RequiredColumns =
        {
            "Name", "Code", "Price", "EarningsPerShare"
        };

        public static CsvLoadResult LoadFromFile(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("CSV dosyası bulunamadı.", path);

            using var reader = new StreamReader(path, Encoding.UTF8);
            return Parse(reader);
        }

        public static CsvLoadResult Parse(TextReader reader)
        {
            var result = new CsvLoadResult();

            string? headerLine = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(headerLine))
            {
                result.Warnings.Add("CSV dosyası boş.");
                return result;
            }

            // Başlık adı -> sütun sırası (büyük/küçük harf duyarsız).
            var columns = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            string[] headers = SplitLine(headerLine);
            for (int i = 0; i < headers.Length; i++)
            {
                string header = headers[i].Trim().TrimStart('﻿');
                if (header.Length > 0 && !columns.ContainsKey(header))
                    columns[header] = i;
            }

            foreach (string required in RequiredColumns)
            {
                if (!columns.ContainsKey(required))
                    throw new InvalidDataException("CSV'de zorunlu sütun eksik: " + required);
            }

            var seenCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            int lineNumber = 1;
            string? line;

            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] fields = SplitLine(line);

                if (!TryCreateCompany(fields, columns, out Company? company, out string error))
                {
                    result.Warnings.Add("Satır " + lineNumber + " atlandı: " + error);
                    continue;
                }

                if (!seenCodes.Add(company.Code))
                {
                    result.Warnings.Add("Satır " + lineNumber + " atlandı: " + company.Code + " kodu tekrar ediyor.");
                    continue;
                }

                result.Companies.Add(company);
            }

            return result;
        }

        // Bir satırı Company nesnesine çevirir; veri hatalıysa nedenini döndürür.
        private static bool TryCreateCompany(string[] fields, Dictionary<string, int> columns,
            [NotNullWhen(true)] out Company? company, out string error)
        {
            company = null;
            error = "";

            string Get(string column)
            {
                if (!columns.TryGetValue(column, out int index) || index >= fields.Length) return "";
                return fields[index].Trim();
            }

            string name = Get("Name");
            string code = Get("Code");
            if (name.Length == 0) { error = "Name boş."; return false; }
            if (code.Length == 0) { error = "Code boş."; return false; }

            if (!TryParseRequiredDouble(Get("Price"), out double price) || price <= 0)
            {
                error = "Price eksik, sayı değil veya sıfırdan küçük.";
                return false;
            }
            if (!TryParseRequiredDouble(Get("EarningsPerShare"), out double eps))
            {
                error = "EarningsPerShare eksik veya sayı değil.";
                return false;
            }

            var c = new Company
            {
                Name = name,
                Code = code,
                Sector = Get("Sector"),
                Price = price,
                EarningsPerShare = eps
            };

            // İsteğe bağlı sütunlar: boşsa 0/false kabul edilir, doluysa geçerli olmalıdır.
            if (!TryOptionalDouble(Get("EarningsGrowthPercent"), v => c.EarningsGrowthPercent = v, "EarningsGrowthPercent", ref error)) return false;
            if (!TryOptionalDouble(Get("RevenueGrowthPercent"), v => c.RevenueGrowthPercent = v, "RevenueGrowthPercent", ref error)) return false;
            if (!TryOptionalDouble(Get("DebtToEquity"), v => c.DebtToEquity = v, "DebtToEquity", ref error)) return false;
            if (!TryOptionalDouble(Get("ReturnOnEquityPercent"), v => c.ReturnOnEquityPercent = v, "ReturnOnEquityPercent", ref error)) return false;
            if (!TryOptionalDouble(Get("NetMarginPercent"), v => c.NetMarginPercent = v, "NetMarginPercent", ref error)) return false;
            if (!TryOptionalDouble(Get("CurrentRatio"), v => c.CurrentRatio = v, "CurrentRatio", ref error)) return false;
            if (!TryOptionalDouble(Get("BookValuePerShare"), v => c.BookValuePerShare = v, "BookValuePerShare", ref error)) return false;

            if (!TryParseBool(Get("PositiveFreeCashFlow"), out bool fcf))
            {
                error = "PositiveFreeCashFlow true/false olmalı.";
                return false;
            }
            c.PositiveFreeCashFlow = fcf;

            if (!TryParseOptionalInt(Get("ProfitableYears"), out int profitableYears))
            {
                error = "ProfitableYears tam sayı olmalı.";
                return false;
            }
            c.ProfitableYears = profitableYears;

            if (!TryParseOptionalInt(Get("DividendYears"), out int dividendYears))
            {
                error = "DividendYears tam sayı olmalı.";
                return false;
            }
            c.DividendYears = dividendYears;

            company = c;
            return true;
        }

        private static bool TryParseRequiredDouble(string text, out double value)
        {
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        private static bool TryOptionalDouble(string text, Action<double> assign, string column, ref string error)
        {
            if (text.Length == 0) { assign(0); return true; }
            if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            {
                assign(value);
                return true;
            }
            error = column + " sayı değil: " + text;
            return false;
        }

        private static bool TryParseOptionalInt(string text, out int value)
        {
            value = 0;
            if (text.Length == 0) return true;
            return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value) && value >= 0;
        }

        private static bool TryParseBool(string text, out bool value)
        {
            switch (text.ToLowerInvariant())
            {
                case "":
                case "false":
                case "0":
                case "no":
                case "hayır":
                    value = false;
                    return true;
                case "true":
                case "1":
                case "yes":
                case "evet":
                    value = true;
                    return true;
                default:
                    value = false;
                    return false;
            }
        }

        // Virgülle ayrılmış satırı böler; tırnak içindeki virgülleri ("Arven, A.Ş.") korur.
        private static string[] SplitLine(string line)
        {
            var fields = new List<string>();
            var current = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char ch = line[i];
                if (inQuotes)
                {
                    if (ch == '"')
                    {
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            current.Append('"');
                            i++;
                        }
                        else
                        {
                            inQuotes = false;
                        }
                    }
                    else
                    {
                        current.Append(ch);
                    }
                }
                else if (ch == '"')
                {
                    inQuotes = true;
                }
                else if (ch == ',')
                {
                    fields.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(ch);
                }
            }

            fields.Add(current.ToString());
            return fields.ToArray();
        }
    }
}
