using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace BTPuygulamasi
{

    public partial class Form1 : Form
    {

        string connectionString = "Server=localhost;Database=butce_takip;Uid=root;Pwd=1234;";


        public Form1()
        {
            InitializeComponent();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            string username = guna2TextBox1.Text;
            string password = guna2TextBox2.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Lütfen kullanıcı adı ve şifre giriniz.");
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO users (username, password) VALUES (@username, @password)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Kayıt başarılı!");
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1062)
                        MessageBox.Show("Bu kullanıcı adı zaten kayıtlı.");
                    else
                        MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }
        ////////

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string username = guna2TextBox1.Text;
            string password = guna2TextBox2.Text;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM users WHERE username = @username AND password = @password";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                int count = Convert.ToInt32(cmd.ExecuteScalar());

                if (count > 0)
                {
                    MessageBox.Show("Giriş başarılı!");
                    
                    this.Hide();
                    icerik Icerik = new icerik(username);
                    Icerik.Show();
                }
                else
                {
                    MessageBox.Show("Kullanıcı adı veya şifre hatalı.");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult onay = MessageBox.Show("Çıkmak istediğine emin misin?", "Çıkış İşlemi", MessageBoxButtons.YesNo);
            if (onay == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
