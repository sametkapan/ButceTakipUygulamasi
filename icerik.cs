using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MySql.Data.MySqlClient;

namespace BTPuygulamasi
{
    public partial class icerik : Form
    {
        string connectionString = "Server=localhost;Database=butce_takip;Uid=root;Pwd=1234;";
        private string username;
        private bool isIncome = false; // Gider varsayılan

        public icerik(string user)
        {
            InitializeComponent();
            this.username = user;
            label1.Text = "Hoşgeldin, " + username;

        }

        private Dictionary<string, Color> gelirRenkler = new()
        {
            { "Maaş", Color.ForestGreen },
            { "Burs", Color.MediumSeaGreen },
            { "Harçlık", Color.MediumAquamarine },
            { "Transfer", Color.DarkSeaGreen },
            { "Avans", Color.MediumSpringGreen },
            { "Gayrimenkul", Color.DarkOliveGreen },
            { "Diğer Gelirler", Color.Gray }
        };

        private Dictionary<string, Color> giderRenkler = new()
        {
            { "Ulaşım", Color.Yellow },
            { "Gıda", Color.Orange },
            { "Faturalar", Color.SkyBlue },
            { "Eğlence", Color.LightSalmon },
            { "Sağlık", Color.Red },
            { "Kira", Color.LightGreen },
            { "Diğer", Color.Gray }
        };

        private void icerik_Load(object sender, EventArgs e)
        {
            guna2cmbCategory.Items.AddRange(new string[] { "Gıda", "Ulaşım", "Faturalar", "Eğlence", "Sağlık", "Kira", "Diğer" });
            guna2cmbCategory2.Items.AddRange(new string[] { "Maaş", "Burs", "Harçlık", "Transfer", "Avans", "Gayrimenkul", "Diğer Gelirler" });
            cmbFilterCategory.Items.Add("Tümü");
            cmbFilterCategory.Items.AddRange(guna2cmbCategory.Items.Cast<string>().ToArray());
            cmbFilterCategory.SelectedIndex = 0;

            cmbTypeFilter.Items.AddRange(new string[] { "Tümü", "Gelir", "Gider" });
            cmbTypeFilter.SelectedIndex = 0;

            dtpStart.Visible = dtpEnd.Visible = label3.Visible = label4.Visible = false;

            InitializeDateSelectors();
            LoadEntriesIntoPanel();
            SetAmountPrefix(isIncome ? "+" : "-");
            UpdateBalance();
            guna2btnToggleIncomeExpense_CheckedChanged(null, null);
            guna2cmbCategory.Visible = true;
            guna2cmbCategory2.Visible = false;
            cmbTypeFilter.SelectedIndexChanged += cmbTypeFilter_SelectedIndexChanged;
            UpdateFilterCategories();
            toggleChartType.Checked = false; // Gider varsayılan
            lblChartType.Text = "Gider";
            LoadPieChart();
            ShowMonthlySuggestions();
            UpdateMonthlySummary();


        }

        private void guna2btnSave_Click(object sender, EventArgs e)
        {
            var selectedCategory = isIncome ? guna2cmbCategory2.SelectedItem : guna2cmbCategory.SelectedItem;

            if (selectedCategory == null)
            {
                MessageBox.Show("Lütfen bir kategori seçin.");
                return;
            }

            if (string.IsNullOrWhiteSpace(guna2txtAmount.Text) ||
               (!guna2txtAmount.Text.StartsWith("+") && !guna2txtAmount.Text.StartsWith("-")))
            {
                MessageBox.Show("Lütfen başında + veya - olan geçerli bir tutar girin.");
                return;
            }

            if (!decimal.TryParse(guna2txtAmount.Text, out decimal parsedAmount))
            {
                MessageBox.Show("Tutar geçerli bir sayı değil.");
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO budget_entries (username, category, amount, description, date) VALUES (@username, @category, @amount, @description, @date)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@category", selectedCategory.ToString());
                cmd.Parameters.AddWithValue("@amount", parsedAmount);
                cmd.Parameters.AddWithValue("@description", guna2txtDescription.Text);
                cmd.Parameters.AddWithValue("@date", guna2dtpDate.Value.Date);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Kayıt başarıyla eklendi!");
            LoadEntriesIntoPanel();
            LoadPieChart();
            UpdateBalance();
            ShowMonthlySuggestions();
            UpdateMonthlySummary();


        }


        private void guna2btnToggleIncomeExpense_CheckedChanged(object sender, EventArgs e)
        {
            isIncome = !isIncome;
            guna2btnToggleIncomeExpense.Text = isIncome ? "Gelir" : "Gider";
            if (isIncome)
            {
                lblSwitchStatus.Text = "Gelir";
                lblSwitchStatus.ForeColor = Color.ForestGreen;
                guna2txtAmount.ForeColor = Color.ForestGreen;
                guna2cmbCategory.Visible = false;
                guna2cmbCategory2.Visible = true;
            }
            else
            {
                lblSwitchStatus.Text = "Gider";
                lblSwitchStatus.ForeColor = Color.Crimson;
                guna2txtAmount.ForeColor = Color.Crimson;
                guna2cmbCategory.Visible = true;
                guna2cmbCategory2.Visible = false;
            }


            SetAmountPrefix(isIncome ? "+" : "-");
        }

        private void SetAmountPrefix(string prefix)
        {
            string current = guna2txtAmount.Text;
            if (current.StartsWith("+") || current.StartsWith("-"))
                current = current.Substring(1);

            guna2txtAmount.Text = prefix + current;
            guna2txtAmount.SelectionStart = guna2txtAmount.Text.Length;
        }

        private void guna2txtAmount_KeyDown(object sender, KeyEventArgs e)
        {
            if ((guna2txtAmount.SelectionStart <= 1) &&
                (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete))
            {
                e.SuppressKeyPress = true;
            }
        }

        private void DeleteEntry(int id)
        {
            if (MessageBox.Show("Bu kaydı silmek istiyor musunuz?", "Silme Onayı", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("DELETE FROM budget_entries WHERE id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
                LoadEntriesIntoPanel();
                LoadPieChart();
                UpdateBalance();

            }
        }

        private void LoadEntriesIntoPanel(DateTime? start = null, DateTime? end = null, string category = "Tümü", string type = "Tümü")
        {
            panelEntries.Controls.Clear();

            using MySqlConnection conn = new(connectionString);
            conn.Open();

            var queryBuilder = new StringBuilder("SELECT id, date, category, amount, description FROM budget_entries WHERE username = @username");

            if (start != null && end != null)
                queryBuilder.Append(" AND date >= @start AND date < @end");

            if (category != "Tümü")
                queryBuilder.Append(" AND category = @category");

            if (type == "Gelir") queryBuilder.Append(" AND amount > 0");
            else if (type == "Gider") queryBuilder.Append(" AND amount < 0");

            queryBuilder.Append(" ORDER BY date DESC");

            var cmd = new MySqlCommand(queryBuilder.ToString(), conn);
            cmd.Parameters.AddWithValue("@username", username);

            if (start != null && end != null)
            {
                cmd.Parameters.AddWithValue("@start", start.Value);
                cmd.Parameters.AddWithValue("@end", end.Value);
            }

            if (category != "Tümü")
                cmd.Parameters.AddWithValue("@category", category);

            using var reader = cmd.ExecuteReader();
            int yOffset = 10;

            while (reader.Read())
            {
                int entryId = reader.GetInt32("id");
                string date = reader.GetDateTime("date").ToString("dd.MM.yyyy");
                string cat = reader.GetString("category");
                decimal amount = reader.GetDecimal("amount");
                string desc = reader.GetString("description");

                var gb = new Guna.UI2.WinForms.Guna2GroupBox
                {
                    Size = new Size(820, 40),
                    Location = new Point(10, yOffset),
                    BackColor = Color.White,
                    FillColor = amount > 0 ? Color.LightGreen : Color.FromArgb(255, 128, 128),
                    BorderRadius = 8,
                    BorderThickness = 0,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    CustomBorderColor = Color.Transparent,
                    CustomBorderThickness = new Padding(0)
                };

                gb.Controls.Add(CreateReadOnlyBox("Kategori", cat, new Point(10, 5)));
                gb.Controls.Add(CreateReadOnlyBox("Tutar", amount.ToString("C"), new Point(190, 5)));
                gb.Controls.Add(CreateReadOnlyBox("Açıklama", desc, new Point(370, 5)));
                gb.Controls.Add(CreateReadOnlyBox("Tarih", date, new Point(550, 5)));

                var btnDelete = new Guna.UI2.WinForms.Guna2Button
                {
                    Text = "Sil",
                    Size = new Size(80, 30),
                    Location = new Point(730, 5),
                    FillColor = Color.IndianRed,
                    ForeColor = Color.White
                };
                btnDelete.Click += (s, e) => DeleteEntry(entryId);
                gb.Controls.Add(btnDelete);

                panelEntries.Controls.Add(gb);
                yOffset += gb.Height;
            }
        }

        private Guna.UI2.WinForms.Guna2TextBox CreateReadOnlyBox(string placeholder, string text, Point location)
        {
            return new Guna.UI2.WinForms.Guna2TextBox
            {
                PlaceholderText = placeholder,
                Text = text,
                Location = location,
                Size = new Size(170, 30),
                ReadOnly = true,
                FillColor = Color.WhiteSmoke,
                BorderRadius = 0,
                Font = new Font("Segoe UI", 9)
            };
        }

        private void InitializeDateSelectors()
        {
            cmbMonth.Items.AddRange(CultureInfo.GetCultureInfo("tr-TR").DateTimeFormat.MonthNames.Where(m => !string.IsNullOrEmpty(m)).ToArray());
            cmbMonth.SelectedIndex = DateTime.Now.Month - 1;

            for (int y = DateTime.Now.Year - 5; y <= DateTime.Now.Year + 1; y++)
                cmbYear.Items.Add(y.ToString());

            cmbYear.SelectedItem = DateTime.Now.Year.ToString();
        }

        private void LoadPieChart()
        {
            chartSummary.Series.Clear();
            chartSummary.Titles.Clear();
            chartSummary.ChartAreas.Clear();
            chartSummary.Legends.Clear();

            string chartTypeTitle = toggleChartType.Checked ? "Gelir Dağılımı" : "Gider Dağılımı";
            chartSummary.Titles.Add("\n" + chartTypeTitle);
            chartSummary.Titles[0].Font = new Font("Segoe UI", 14, FontStyle.Bold);
            chartSummary.ChartAreas.Add(new ChartArea("SummaryArea"));
            chartSummary.Legends.Add(new Legend("Legend") { Docking = Docking.Bottom, Alignment = StringAlignment.Center });

            Series series = new(toggleChartType.Checked ? "Gelirler" : "Giderler")
            {
                ChartType = SeriesChartType.Pie,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ["PieLabelStyle"] = "Inside"
            };
            chartSummary.Series.Add(series);

            string selectedMonth = cmbMonth.SelectedItem.ToString();
            int month = DateTime.ParseExact(selectedMonth, "MMMM", new CultureInfo("tr-TR")).Month;
            int year = int.Parse(cmbYear.SelectedItem.ToString());

            Dictionary<string, decimal> kategoriler = toggleChartType.Checked
                ? new(gelirRenkler.Keys.ToDictionary(k => k, k => 0m))
                : new(giderRenkler.Keys.ToDictionary(k => k, k => 0m));

            using (MySqlConnection conn = new(connectionString))
            {
                conn.Open();
                string query = "SELECT category, amount FROM budget_entries WHERE username = @username AND MONTH(date) = @month AND YEAR(date) = @year";
                MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@month", month);
                cmd.Parameters.AddWithValue("@year", year);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string category = reader.GetString("category");
                    decimal amount = reader.GetDecimal("amount");

                    if (toggleChartType.Checked && amount > 0)
                    {
                        if (!kategoriler.ContainsKey(category)) category = "Diğer Gelirler";
                        kategoriler[category] += amount;
                    }
                    else if (!toggleChartType.Checked && amount < 0)
                    {
                        if (!kategoriler.ContainsKey(category)) category = "Diğer";
                        kategoriler[category] += Math.Abs(amount);
                    }
                }
            }

            decimal toplam = kategoriler.Values.Sum();
            if (toplam == 0) return;

            var renkler = toggleChartType.Checked ? gelirRenkler : giderRenkler;

            foreach (var item in kategoriler.Where(i => i.Value > 0))
            {
                double percent = (double)item.Value / (double)toplam * 100;
                int pointIndex = series.Points.AddY(item.Value);
                var point = series.Points[pointIndex];

                point.LegendText = item.Key;
                point.Color = renkler[item.Key];
                point.ToolTip = $"{item.Key}: {item.Value:C} ({percent:F1}%)";
                point.Label = percent >= 5 ? $"{item.Key}\n{percent:F1}%" : "";
            }

            chartSummary.Invalidate();
        }

        private void toggleChartType_CheckedChanged(object sender, EventArgs e)
        {
            lblChartType.Text = toggleChartType.Checked ? "Gelir" : "Gider";
            LoadPieChart();
        }

        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            string selectedCategory = cmbFilterCategory.SelectedItem?.ToString() ?? "Tümü";
            string selectedType = cmbTypeFilter.SelectedItem?.ToString() ?? "Tümü";

            DateTime? start = null, end = null;
            if (chkDateFilter.Checked)
            {
                start = dtpStart.Value.Date;
                end = dtpEnd.Value.Date.AddDays(1);
            }

            LoadEntriesIntoPanel(start, end, selectedCategory, selectedType);
        }

        private void UpdateFilterCategories()
        {
            cmbFilterCategory.Items.Clear();
            cmbFilterCategory.Items.Add("Tümü");

            if (cmbTypeFilter.SelectedItem?.ToString() == "Gelir")
            {
                cmbFilterCategory.Items.AddRange(guna2cmbCategory2.Items.Cast<string>().ToArray());
            }
            else if (cmbTypeFilter.SelectedItem?.ToString() == "Gider")
            {
                cmbFilterCategory.Items.AddRange(guna2cmbCategory.Items.Cast<string>().ToArray());
            }
            else // Tümü
            {
                var allCategories = guna2cmbCategory.Items.Cast<string>()
                    .Concat(guna2cmbCategory2.Items.Cast<string>())
                    .Distinct()
                    .ToArray();

                cmbFilterCategory.Items.AddRange(allCategories);
            }

            cmbFilterCategory.SelectedIndex = 0;
        }

        private void cmbTypeFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFilterCategories();
        }



        private void chkDateFilter_CheckedChanged(object sender, EventArgs e)
        {
            bool visible = chkDateFilter.Checked;
            dtpStart.Visible = dtpEnd.Visible = label3.Visible = label4.Visible = visible;
        }

        private void btnShowChart_Click(object sender, EventArgs e)
        {
            LoadPieChart();
            UpdateMonthlySummary();

        }


        private void UpdateBalance()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT SUM(amount) FROM budget_entries WHERE username = @username";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);

                object result = cmd.ExecuteScalar();
                decimal balance = 0;

                if (result != DBNull.Value)
                    balance = Convert.ToDecimal(result);

                lblBalance.Text = "Bakiye: " + balance.ToString("C2");
                lblBalance.ForeColor = balance >= 0 ? Color.DarkGreen : Color.Red;
            }
        }

        private void UpdateMonthlySummary()
        {
            if (cmbMonth.SelectedItem == null || cmbYear.SelectedItem == null)
                return;

            string selectedMonth = cmbMonth.SelectedItem.ToString();
            int month = DateTime.ParseExact(selectedMonth, "MMMM", new CultureInfo("tr-TR")).Month;
            int year = int.Parse(cmbYear.SelectedItem.ToString());

            decimal totalIncome = 0, totalExpense = 0;

            using (MySqlConnection conn = new(connectionString))
            {
                conn.Open();
                string query = @"SELECT amount FROM budget_entries 
                         WHERE username = @username AND MONTH(date) = @month AND YEAR(date) = @year";
                MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@month", month);
                cmd.Parameters.AddWithValue("@year", year);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    decimal amount = reader.GetDecimal("amount");
                    if (amount > 0)
                        totalIncome += amount;
                    else
                        totalExpense += Math.Abs(amount);
                }
            }

            decimal net = totalIncome - totalExpense;

            var lblTotalIncome = this.Controls.Find("lblTotalIncome", true).FirstOrDefault();
            var lblTotalExpense = this.Controls.Find("lblTotalExpense", true).FirstOrDefault();
            var lblNetProfit = this.Controls.Find("lblNetProfit", true).FirstOrDefault();

            if (lblTotalIncome != null) lblTotalIncome.Text = $"Toplam Gelir: {totalIncome:C2}";
            if (lblTotalExpense != null) lblTotalExpense.Text = $"Toplam Gider: {totalExpense:C2}";
            if (lblNetProfit != null)
            {
                lblNetProfit.Text = $"Net Kar: {net:C2}";
                lblNetProfit.ForeColor = net >= 0 ? Color.DarkGreen : Color.Crimson;
            }
        }



        //Bütce danısmanı

        private void ShowMonthlySuggestions()
        {
            flowLayoutPanelSuggestions.Controls.Clear();
            var advisor = new MonthlyAdvisor(username, connectionString);
            var suggestions = advisor.GenerateSuggestions();

            foreach (var suggestion in suggestions)
            {
                Label lbl = new()
                {
                    Text = "• " + suggestion,
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10, FontStyle.Regular),
                    ForeColor = suggestion.StartsWith("Uyarı") ? Color.Red :
                                suggestion.StartsWith("Tavsiyemiz") ? Color.DarkOrange :
                                Color.DarkGreen,
                    Padding = new Padding(5)
                };
                flowLayoutPanelSuggestions.Controls.Add(lbl);
            }
        }





    }
}
