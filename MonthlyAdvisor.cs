using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using static Guna.UI2.Native.WinApi;

namespace BTPuygulamasi
{
    internal class MonthlyAdvisor
    {
        
        
            private readonly string username;
            private readonly string connectionString;

            public MonthlyAdvisor(string username, string connectionString)
            {
                this.username = username;
                this.connectionString = connectionString;
            }

            public List<string> GenerateSuggestions()
            {
                List<string> suggestions = new();
                decimal totalIncome = 0, totalExpense = 0;
                Dictionary<string, decimal> expenseByCategory = new();

                using MySqlConnection conn = new(connectionString);
                conn.Open();

                string query = @"SELECT category, amount FROM budget_entries 
                         WHERE username = @username 
                         AND MONTH(date) = @month 
                         AND YEAR(date) = @year";

                MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@month", DateTime.Now.Month);
                cmd.Parameters.AddWithValue("@year", DateTime.Now.Year);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    decimal amount = reader.GetDecimal("amount");
                    string category = reader.GetString("category");

                    if (amount > 0) totalIncome += amount;
                    else
                    {
                        totalExpense += Math.Abs(amount);
                        if (!expenseByCategory.ContainsKey(category))
                            expenseByCategory[category] = 0;
                        expenseByCategory[category] += Math.Abs(amount);
                    }
                }

                // Uyarı: Giderler geliri geçmiş
                if (totalExpense > totalIncome)
                    suggestions.Add("Uyarı: Bu ay harcamalarınız gelirlerinizi aştı. Giderlerinizi gözden geçirin.");

                // Uyarı: Gelirlerin %30'undan fazlası harcanmış
                if (totalIncome > 0 && totalExpense / totalIncome > 0.7m)
                    suggestions.Add("Tavsiyemiz: Gelirlerinizin %70'inden fazlasını harcadınız. Tasarrufa yönelin.");

                // En çok harcanan kategori
                if (expenseByCategory.Count > 0)
                {
                    var max = expenseByCategory.OrderByDescending(e => e.Value).First();
                    suggestions.Add($"Bilgilendirme: Bu ay en çok harcama yaptığınız kategori: {max.Key} ({max.Value:C}).");
                }

                // Gelir yoksa
                if (totalIncome == 0)
                    suggestions.Add("Uyarı: Bu ay herhangi bir gelir kaydedilmemiş. Gelir kaynaklarınızı kaydetmeyi unutmayın.");

                // Hiç harcama yoksa
                if (totalExpense == 0)
                    suggestions.Add("Bu ay hiç harcama yapılmamış, unutuyor musun yoksa gerçekten harcamıyor musun?");

                //Hiç tasarruf yapılmadıysa
                decimal savings = totalIncome - totalExpense;
                if (savings < 0)
                suggestions.Add("Bu ay gelirinizden fazlasını harcadınız. Tasarruf yapmayı düşünmelisiniz.");
                else if (savings < totalIncome * 0.1m)
                suggestions.Add("Bu ay çok az tasarruf ettiniz. Her ay gelirinizin en az %10'unu kenara ayırmayı deneyin.");


            return suggestions;
            }
        }

    
}
