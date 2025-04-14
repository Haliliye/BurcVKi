using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Globalization;

namespace BurcVKIProjesi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string GetBurc(int gun, int ay)
        {
            if ((ay == 1 && gun >= 20) || (ay == 2 && gun <= 18)) return "Kova";
            if ((ay == 2 && gun >= 19) || (ay == 3 && gun <= 20)) return "Balık";
            if ((ay == 3 && gun >= 21) || (ay == 4 && gun <= 19)) return "Koç";
            if ((ay == 4 && gun >= 20) || (ay == 5 && gun <= 20)) return "Boğa";
            if ((ay == 5 && gun >= 21) || (ay == 6 && gun <= 21)) return "İkizler";
            if ((ay == 6 && gun >= 22) || (ay == 7 && gun <= 22)) return "Yengeç";
            if ((ay == 7 && gun >= 23) || (ay == 8 && gun <= 22)) return "Aslan";
            if ((ay == 8 && gun >= 23) || (ay == 9 && gun <= 22)) return "Başak";
            if ((ay == 9 && gun >= 23) || (ay == 10 && gun <= 22)) return "Terazi";
            if ((ay == 10 && gun >= 23) || (ay == 11 && gun <= 21)) return "Akrep";
            if ((ay == 11 && gun >= 22) || (ay == 12 && gun <= 21)) return "Yay";
            if ((ay == 12 && gun >= 22) || (ay == 1 && gun <= 19)) return "Oğlak";
            return "Bilinmiyor";
        }

        string GetBurcYorum(string burc)
        {
            switch (burc)
            {
                case "Koç": return "Cesur, lider ruhludur.";
                case "Boğa": return "Sabırlı ve güvenilirdir.";
                case "İkizler": return "Zeki ve iletişimde kuvvetlidir.";
                case "Yengeç": return "Duygusal, sadık ve sezgiseldir.";
                case "Aslan": return "Kendine güvenen ve cömerttir.";
                case "Başak": return "Titiz ve detaycıdır.";
                case "Terazi": return "Adil ve dengelidir.";
                case "Akrep": return "Tutkulu ve gizemlidir.";
                case "Yay": return "Macera sever ve enerjiktir.";
                case "Oğlak": return "Disiplinli ve kararlıdır.";
                case "Kova": return "Bağımsız ve yenilikçidir.";
                case "Balık": return "Hayalperest ve sezgiseldir.";
                default: return "Burç yorumu bulunamadı.";
            }
        }

        float HesaplaVKI(float boy, float kilo)
        {
            return kilo / (boy * boy);
        }

        string VKIYorum(float vki)
        {
            if (vki < 18.5) return "Zayıf";
            else if (vki < 25) return "Normal";
            else if (vki < 30) return "Fazla kilolu";
            else return "Obez";
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            string ad = txtAd.Text;
            string soyad = txtSoyad.Text;
            DateTime dogum = dtpDogumTarihi.Value;
            float boy = float.Parse(txtBoy.Text, CultureInfo.InvariantCulture);
            float kilo = float.Parse(txtKilo.Text, CultureInfo.InvariantCulture);

            int gun = dogum.Day;
            string ayIsim = dogum.ToString("MMMM", new CultureInfo("tr-TR"));
            int aySayi = dogum.Month;
            int yil = dogum.Year;

            string burc = GetBurc(gun, aySayi);
            string yorum = GetBurcYorum(burc);

            float vki = HesaplaVKI(boy, kilo);
            string vkiYorum = VKIYorum(vki);

            string burcResmi = $"burclar\\{burc.ToLower().Replace("ç", "c").Replace("ğ", "g").Replace("ı", "i").Replace("ö", "o").Replace("ş", "s").Replace("ü", "u")}.jpg";

            if (System.IO.File.Exists(burcResmi))
                pictureBox1.Image = Image.FromFile(burcResmi);
            else
                pictureBox1.Image = null;

            lblSonuc.Text = $"Burç: {burc}\nYorum: {yorum}\nVKİ: {vki:F1} - {vkiYorum}";

            string connectionString = "Server=localhost;Database=BurcVKI;Trusted_Connection=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO Kisiler (Ad, Soyad, Gun, Ay, Yil, Burc, Yorum, BurcResmi, VKI, VKIYorum) " +
                             "VALUES (@ad, @soyad, @gun, @ay, @yil, @burc, @yorum, @resim, @vki, @vkiyorum)";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ad", ad);
                    cmd.Parameters.AddWithValue("@soyad", soyad);
                    cmd.Parameters.AddWithValue("@gun", gun);
                    cmd.Parameters.AddWithValue("@ay", ayIsim);
                    cmd.Parameters.AddWithValue("@yil", yil);
                    cmd.Parameters.AddWithValue("@burc", burc);
                    cmd.Parameters.AddWithValue("@yorum", yorum);
                    cmd.Parameters.AddWithValue("@resim", burcResmi);
                    cmd.Parameters.AddWithValue("@vki", vki);
                    cmd.Parameters.AddWithValue("@vkiyorum", vkiYorum);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Bilgiler başarıyla kaydedildi!");
                }
            }
        }
    }
}
