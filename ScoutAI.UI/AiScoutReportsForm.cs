using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace ScoutAI.UI
{
    public partial class AiScoutReportsForm : Form
    {
        private Panel pnlSidebar;
        private DataGridView dgvScoutReports;

        public AiScoutReportsForm()
        {
            InitializeComponentCustom();
            LoadScoutData();
        }

        private void AiScoutReportsForm_Load(object sender, EventArgs e)
        {
        }

        private void InitializeComponentCustom()
        {
            this.Text = "Scout AI - AI Scout Raporları";
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(18, 22, 30); // Koyu füme/siyah tema

            // 1. Sol Menü (Portal Standartlarına Tam Uyumlu Sidebar)
            pnlSidebar = new Panel()
            {
                Dock = DockStyle.Left,
                Width = 260,
                BackColor = Color.FromArgb(28, 43, 65)
            };

            Label lblLogoIcon = new Label()
            {
                Text = "⚽",
                Left = 20,
                Top = 20,
                AutoSize = true,
                Font = new Font("Segoe UI Emoji", 16, FontStyle.Regular)
            };

            Label lblTitleHeader = new Label()
            {
                Text = "SCOUT AI PORTAL",
                Left = 60,
                Top = 24,
                AutoSize = true,
                ForeColor = Color.FromArgb(46, 204, 113),
                Font = new Font("Arial", 12f, FontStyle.Bold)
            };

            Label lblMenuTitle = new Label()
            {
                Text = "FUTBOL MENÜSÜ",
                Left = 20,
                Top = 85,
                AutoSize = true,
                ForeColor = Color.FromArgb(170, 190, 210),
                Font = new Font("Arial", 8.5f, FontStyle.Bold)
            };

            string[] menuItems = { "Yönetim Ekranı", "Oyuncu Havuzu", "AI Scout Raporları", "Benim 11'im", "Transfer Borsası" };
            int menuTop = 118;

            foreach (var item in menuItems)
            {
                Button btnMenu = new Button()
                {
                    Text = item,
                    Left = 15,
                    Top = menuTop,
                    Width = 230,
                    Height = 38,
                    FlatStyle = FlatStyle.Flat,
                    TextAlign = ContentAlignment.MiddleLeft,
                    ForeColor = Color.White,
                    BackColor = item == "AI Scout Raporları" ? Color.FromArgb(46, 204, 113) : Color.FromArgb(35, 53, 78),
                    Font = new Font("Arial", 9f, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btnMenu.FlatAppearance.BorderSize = 0;

                string currentItem = item;
                btnMenu.Click += (s, e) => {
                    if (currentItem == "Yönetim Ekranı" || currentItem == "Oyuncu Havuzu") { this.Close(); }
                    else if (currentItem == "Benim 11'im") { MessageBox.Show("Benim 11'im kadro planlama ekranı yakında aktif olacaktır.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information); }
                    else if (currentItem == "Transfer Borsası") { MessageBox.Show("Transfer Borsası modülü yakında aktif olacaktır.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information); }
                };

                pnlSidebar.Controls.Add(btnMenu);
                menuTop += 45;
            }

            pnlSidebar.Controls.Add(lblLogoIcon);
            pnlSidebar.Controls.Add(lblTitleHeader);
            pnlSidebar.Controls.Add(lblMenuTitle);

            // 2. Sağ Taraf - Tablo ve Başlık Alanı
            Label lblTopHeader = new Label()
            {
                Text = "Yapay Zeka Destekli 200 Kişilik Mega Küresel Scout Arşivi",
                Left = 290,
                Top = 25,
                AutoSize = true,
                ForeColor = Color.White,
                Font = new Font("Arial", 14, FontStyle.Bold)
            };

            dgvScoutReports = new DataGridView()
            {
                Left = 290,
                Top = 75,
                Width = 980,
                Height = 600,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.FromArgb(28, 43, 65),
                ForeColor = Color.White,
                GridColor = Color.FromArgb(45, 55, 72)
            };

            dgvScoutReports.EnableHeadersVisualStyles = false;
            dgvScoutReports.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(35, 53, 78);
            dgvScoutReports.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvScoutReports.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            dgvScoutReports.DefaultCellStyle.BackColor = Color.FromArgb(28, 43, 65);
            dgvScoutReports.DefaultCellStyle.ForeColor = Color.LightGray;
            dgvScoutReports.DefaultCellStyle.SelectionBackColor = Color.FromArgb(46, 204, 113);
            dgvScoutReports.DefaultCellStyle.SelectionForeColor = Color.Black;

            this.Controls.Add(pnlSidebar);
            this.Controls.Add(lblTopHeader);
            this.Controls.Add(dgvScoutReports);
        }

        private void LoadScoutData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("OYUNCU", typeof(string));
            dt.Columns.Add("KULÜP", typeof(string));
            dt.Columns.Add("MEVKİ", typeof(string));
            dt.Columns.Add("POTANSİYEL SKORU", typeof(string));
            dt.Columns.Add("AI ÖNERİSİ", typeof(string));

            // =========================================================================
            // MEVCUT 100 KİŞİLİK LİSTE (TÜRK, DÜNYA, SÜPER LİG VE 5 KITA KÜRESEL HAVUZ)
            // =========================================================================
            dt.Rows.Add("Semih Kılıçsoy", "Beşiktaş", "Santrafor", "88 / 100", "Fırsat Transferi (Yüksek Gelişim)");
            dt.Rows.Add("Arda Güler", "Real Madrid", "On Numara", "94 / 100", "Dünya Sınıfı Yetenek");
            dt.Rows.Add("Kenan Yıldız", "Juventus", "Sol Kanat", "91 / 100", "Kilit Oyuncu Potansiyeli");

            dt.Rows.Add("Cristiano Ronaldo", "Al-Nassr", "Santrafor", "92 / 100", "Efsanevi Bitiricilik / Lider");
            dt.Rows.Add("Lionel Messi", "Inter Miami", "Sağ Kanat", "95 / 100", "Tarihin En İyisi / Dahi");
            dt.Rows.Add("Kylian Mbappé", "Real Madrid", "Sol Kanat", "96 / 100", "Asrın Süper Yıldızı");
            dt.Rows.Add("Erling Haaland", "Manchester City", "Santrafor", "95 / 100", "Durdurulamaz Gol Makinesi");
            dt.Rows.Add("Jude Bellingham", "Real Madrid", "Merkez Orta Saha", "94 / 100", "Geleceğin Altın Top Adayı");
            dt.Rows.Add("Lamine Yamal", "Barcelona", "Sağ Kanat", "93 / 100", "Tarihi Wonderkid Potansiyeli");
            dt.Rows.Add("Kevin De Bruyne", "Manchester City", "On Numara", "93 / 100", "Üstün Oyun Görüşü / Maestro");
            dt.Rows.Add("Vinícius Júnior", "Real Madrid", "Sol Kanat", "94 / 100", "Birebirde Durdurulamaz");
            dt.Rows.Add("Harry Kane", "Bayern München", "Santrafor", "91 / 100", "Komple Oyun Kurucu Forvet");
            dt.Rows.Add("Rodri", "Manchester City", "Defansif Orta Saha", "95 / 100", "Orta Sahanın Dinamosu / Kaya");
            dt.Rows.Add("Mohamed Salah", "Liverpool", "Sağ Kanat", "90 / 100", "İstikrar Abidesi / Skor Üretici");
            dt.Rows.Add("Virgil van Dijk", "Liverpool", "Stoper", "91 / 100", "Savunma Hattı Lideri");
            dt.Rows.Add("Florian Wirtz", "Bayer Leverkusen", "On Numara", "92 / 100", "Yaratıcı Oyun Kurucu Deha");
            dt.Rows.Add("William Saliba", "Arsenal", "Stoper", "90 / 100", "Geleceğin En İyi Stoperi");
            dt.Rows.Add("Bukayo Saka", "Arsenal", "Sağ Kanat", "91 / 100", "Kanat Bindirme Uzmanı");
            dt.Rows.Add("Thibaut Courtois", "Real Madrid", "Kaleci", "92 / 100", "Kalede Geçilmez Duvar");
            dt.Rows.Add("Manuel Neuer", "Bayern München", "Kaleci", "89 / 100", "Tecrübeli Ekol / Süpürücü");

            dt.Rows.Add("Mauro Icardi", "Galatasaray", "Santrafor", "90 / 100", "Ceza Sahası Katili / Lider");
            dt.Rows.Add("Victor Osimhen", "Galatasaray", "Santrafor", "93 / 100", "Fiziksel Canavar / Pres Canavarı");
            dt.Rows.Add("Dries Mertens", "Galatasaray", "On Numara", "87 / 100", "Kilit Pas Uzmanı / Maestro");
            dt.Rows.Add("Fernando Muslera", "Galatasaray", "Kaleci", "89 / 100", "Kalede Güven Abidesi / Efsane");
            dt.Rows.Add("Barış Alper Yılmaz", "Galatasaray", "Sağ Kanat", "88 / 100", "Fiziksel Güç / Jokey Kanat");
            dt.Rows.Add("Davinson Sánchez", "Galatasaray", "Stoper", "89 / 100", "Sert Savunmacı / Hava Topu Canavarı");
            dt.Rows.Add("Gabriel Sara", "Galatasaray", "Merkez Orta Saha", "88 / 100", "Duran Top Ustası / Oyun Kurucu");
            dt.Rows.Add("Lucas Torreira", "Galatasaray", "Defansif Orta Saha", "90 / 100", "Orta Sahanın Süpürgesi");
            dt.Rows.Add("Roland Sallai", "Galatasaray", "Sağ Kanat", "86 / 100", "Çalışkan ve Delici Kanat");
            dt.Rows.Add("Victor Nelsson", "Galatasaray", "Stoper", "85 / 100", "Pozisyon Alma Ustası");

            dt.Rows.Add("Edin Džeko", "Fenerbahçe", "Santrafor", "88 / 100", "Kıdemli Bitirici / Tecrübe");
            dt.Rows.Add("Youssef En-Nesyri", "Fenerbahçe", "Santrafor", "89 / 100", "Hava Hakimiyeti / Atletik");
            dt.Rows.Add("Dušan Tadić", "Fenerbahçe", "Sol Kanat", "89 / 100", "Yaratıcı Oyun Görüşü / Asist Kralı");
            dt.Rows.Add("Fred", "Fenerbahçe", "Merkez Orta Saha", "88 / 100", "Dinamik Pres / Pres Kırıcı");
            dt.Rows.Add("Sofyan Amrabat", "Fenerbahçe", "Defansif Orta Saha", "89 / 100", "Sert Mücadele / Kesici");
            dt.Rows.Add("Dominik Livaković", "Fenerbahçe", "Kaleci", "88 / 100", "Çizgi Refleksleri Üst Düzey");
            dt.Rows.Add("İsmail Yüksek", "Fenerbahçe", "Defansif Orta Saha", "86 / 100", "Gelişime Açık Yerli Dinamo");
            dt.Rows.Add("Çağlar Söyüncü", "Fenerbahçe", "Stoper", "87 / 100", "Agresif Stoper / Oyun Kurma");
            dt.Rows.Add("Allan Saint-Maximin", "Fenerbahçe", "Sol Kanat", "89 / 100", "Birebir Çalım Üstadı");
            dt.Rows.Add("Sebastian Szymański", "Fenerbahçe", "On Numara", "88 / 100", "Yorulmak Bilmeyen Pres ve Şut");

            dt.Rows.Add("Ciro Immobile", "Beşiktaş", "Santrafor", "88 / 100", "Gol Yollarında Bitirici Uzman");
            dt.Rows.Add("Rafa Silva", "Beşiktaş", "On Numara", "91 / 100", "Çevik Çalım / Rakip Dengesini Bozan");
            dt.Rows.Add("Gedson Fernandes", "Beşiktaş", "Merkez Orta Saha", "89 / 100", "Yorulmak Bilmeyen Motor");
            dt.Rows.Add("Arthur Masuaku", "Beşiktaş", "Sol Bek", "85 / 100", "Ofansif Bek / Bindirme");
            dt.Rows.Add("Mert Günok", "Beşiktaş", "Kaleci", "86 / 100", "Birebir Kurtarış Başarısı");
            dt.Rows.Add("Ernest Muçi", "Beşiktaş", "On Numara", "87 / 100", "Uzaktan Şut Tehditi / Wonderkid");
            dt.Rows.Add("Felix Uduokhai", "Beşiktaş", "Stoper", "85 / 100", "Güçlü Solak Stoper");
            dt.Rows.Add("Milot Rashica", "Beşiktaş", "Sağ Kanat", "85 / 100", "Takım Oyunu ve Defansif Destek");

            dt.Rows.Add("Simon Banza", "Trabzonspor", "Santrafor", "86 / 100", "Güçlü Pivot Santrafor");
            dt.Rows.Add("Edin Višća", "Trabzonspor", "Sağ Kanat", "86 / 100", "İstikrarlı Orta ve Asist");
            dt.Rows.Add("Uğurcan Çakır", "Trabzonspor", "Kaleci", "88 / 100", "Kalede Refleks ve Liderlik");
            dt.Rows.Add("Ozan Tufan", "Trabzonspor", "Merkez Orta Saha", "84 / 100", "Şut Gücü ve Dinamizm");
            dt.Rows.Add("Anthony Nwakaeme", "Trabzonspor", "Sol Kanat", "83 / 100", "Fiziksel Güç ve Sakinlik");

            dt.Rows.Add("Krzysztof Piatek", "İstanbul Başakşehir", "Santrafor", "87 / 100", "Ceza Sahası Golcüsü");
            dt.Rows.Add("Olivier Kemen", "İstanbul Başakşehir", "Merkez Orta Saha", "83 / 100", "Dinamik Orta Saha");
            dt.Rows.Add("Nuno Da Costa", "Kasımpaşa", "Santrafor", "84 / 100", "Hızlı ve Delici Hücum");
            dt.Rows.Add("Mame Thiam", "Eyüpspor", "Santrafor", "85 / 100", "Etkili Son Vuruş");
            dt.Rows.Add("Jonjo Shelvey", "Eyüpspor", "Merkez Orta Saha", "83 / 100", "Uzun Pas Vizyonu");
            dt.Rows.Add("Aytuğ Batur", "Sipay Bodrum FK", "Stoper", "82 / 100", "Yerli Genç Potansiyel");

            // (İlk 100'ün geri kalan Asya, Amerika, Afrika, MLS ve Avrupa yıldızları)
            dt.Rows.Add("Salem Al-Dawsari", "Al-Hilal", "Sol Kanat", "85 / 100", "Asya'nın En İyi Kanat Delicisi");
            dt.Rows.Add("Neymar Jr", "Al-Hilal", "Sol Kanat", "91 / 100", "Büyüleyici Çalım ve Vizyon");
            dt.Rows.Add("Aleksandar Mitrović", "Al-Hilal", "Santrafor", "89 / 100", "Hava Topu ve Bitiricilik Canavarı");
            dt.Rows.Add("Rúben Neves", "Al-Hilal", "Merkez Orta Saha", "89 / 100", "Uzaktan Şut ve Oyun Kurma");
            dt.Rows.Add("Sadio Mané", "Al-Nassr", "Sol Kanat", "88 / 100", "Yüksek Sürat ve Pres Gücü");
            dt.Rows.Add("Aymeric Laporte", "Al-Nassr", "Stoper", "88 / 100", "Top Çıkarma ve Liderlik");
            dt.Rows.Add("Karim Benzema", "Al-Ittihad", "Santrafor", "90 / 100", "Usta Ayak / Kusursuz Bağlantı");
            dt.Rows.Add("N'Golo Kanté", "Al-Ittihad", "Defansif Orta Saha", "89 / 100", "Asla Durmayan Ciğersiz Pres");
            dt.Rows.Add("Anderson Talisca", "Al-Nassr", "On Numara", "87 / 100", "Sol Ayak Uzaktan Füze Tehditi");
            dt.Rows.Add("Oscar", "Shanghai Port", "On Numara", "86 / 100", "Asya Liglerinin Maestro Oyuncusu");

            dt.Rows.Add("Endrick", "Real Madrid", "Santrafor", "89 / 100", "Patlayıcı Güç / Genç Fenomen");
            dt.Rows.Add("Thiago Almada", "Botafogo", "On Numara", "87 / 100", "Arjantinli Yaratıcı Komutan");
            dt.Rows.Add("Gabigol", "Flamengo", "Santrafor", "85 / 100", "Kritik Maçların Bitiricisi");
            dt.Rows.Add("Giorgian de Arrascaeta", "Flamengo", "On Numara", "88 / 100", "Kilit Pasların Ustası");
            dt.Rows.Add("Estêvão Willian", "Palmeiras", "Sağ Kanat", "90 / 100", "Geleceğin Süperstarı / 'Messinho'");
            dt.Rows.Add("Miguel Borja", "River Plate", "Santrafor", "84 / 100", "Ceza Sahası Bitiricisi");
            dt.Rows.Add("Franco Armani", "River Plate", "Kaleci", "84 / 100", "Güvenilir Arjantinli Eldiven");
            dt.Rows.Add("Enner Valencia", "Internacional", "Santrafor", "85 / 100", "Tecrübeli ve Süratli Hücum");
            dt.Rows.Add("Pedro", "Flamengo", "Santrafor", "86 / 100", "Klasik Brezilyalı Pivot Forvet");
            dt.Rows.Add("Jhon Arias", "Fluminense", "Sağ Kanat", "86 / 100", "Çevik ve Delici Kolombiyalı");

            dt.Rows.Add("Percy Tau", "Al Ahly", "Sağ Kanat", "83 / 100", "Afrika Şampiyonu Kanat Dinamosu");
            dt.Rows.Add("Hussein El Shahat", "Al Ahly", "Sol Kanat", "82 / 100", "Çok Yönlü Hücum Gücü");
            dt.Rows.Add("Ronwen Williams", "Mamelodi Sundowns", "Kaleci", "84 / 100", "Ayakla Oyun Kurma Uzmanı");
            dt.Rows.Add("Themba Zwane", "Mamelodi Sundowns", "On Numara", "83 / 100", "Tecrübeli Oyun Aklı");
            dt.Rows.Add("Pyate", "Pyramids FC", "Merkez Orta Saha", "81 / 100", "Dinamik Orta Saha");
            dt.Rows.Add("Ali Maâloul", "Al Ahly", "Sol Bek", "84 / 100", "Asistci Tecrübeli Sol Bek");
            dt.Rows.Add("Denis Onyango", "Mamelodi Sundowns", "Kaleci", "80 / 100", "Kıdemli Efsane Kaleci");
            dt.Rows.Add("Ben Malango", "Qatar SC", "Santrafor", "81 / 100", "Fiziksel Güçlü Forvet");
            dt.Rows.Add("Zinedine Belaïd", "USM Alger", "Stoper", "82 / 100", "Güvenli Savunma Hattı");
            dt.Rows.Add("Yahya Jabrane", "Wydad Casablanca", "Defansif Orta Saha", "83 / 100", "Sert Mücadeleci Kesici");

            dt.Rows.Add("Luis Suárez", "Inter Miami", "Santrafor", "88 / 100", "Kıdemli Kurt / Bitirici Dahi");
            dt.Rows.Add("Sergio Busquets", "Inter Miami", "Defansif Orta Saha", "89 / 100", "Pozisyon Alma ve Akıl Hocası");
            dt.Rows.Add("Jordi Alba", "Inter Miami", "Sol Bek", "86 / 100", "Hâlâ Durmayan Sol Çizgi");
            dt.Rows.Add("Lorenzo Insigne", "Toronto FC", "Sol Kanat", "86 / 100", "Falsolu Şutlar ve Çalım Yeteneği");
            dt.Rows.Add("Héctor Herrera", "Houston Dynamo", "Merkez Orta Saha", "83 / 100", "Orta Sahada Tecrübe ve Direnç");
            dt.Rows.Add("Riqui Puig", "LA Galaxy", "On Numara", "87 / 100", "MLS'in Pas ve Oyun Kurma Kralı");
            dt.Rows.Add("Denis Bouanga", "LAFC", "Sol Kanat", "88 / 100", "Delici Sürat ve Gol Tehditi");
            dt.Rows.Add("Hany Mukhtar", "Nashville SC", "On Numara", "86 / 100", "Skor Üreten Komutan");
            dt.Rows.Add("Christian Benteke", "DC United", "Santrafor", "85 / 100", "Hava Toplarının Efendisi");
            dt.Rows.Add("Emil Forsberg", "New York Red Bulls", "On Numara", "87 / 100", "İsveçli Klas Oyun Kurucu");

            dt.Rows.Add("Cole Palmer", "Chelsea", "On Numara", "93 / 100", "Soğukkanlı Yıldız / Premier Lig Sensation");
            dt.Rows.Add("Jamal Musiala", "Bayern München", "On Numara", "94 / 100", "Durdurulamaz Çalım Dehası");
            dt.Rows.Add("Phil Foden", "Manchester City", "On Numara", "93 / 100", "Teknik ve Alan Görüşü Üst Düzey");
            dt.Rows.Add("Declan Rice", "Arsenal", "Defansif Orta Saha", "92 / 100", "Orta Sahanın Sigortası");
            dt.Rows.Add("Alexander Isak", "Newcastle United", "Santrafor", "89 / 100", "Zarif ve Bitirici Hücumcu");
            dt.Rows.Add("Martin Ødegaard", "Arsenal", "On Numara", "91 / 100", "Kaptanlık ve Yaratıcı Baskı");
            dt.Rows.Add("Rafael Leão", "AC Milan", "Sol Kanat", "90 / 100", "Fırtına Gibi Patlayıcı Sürat");
            dt.Rows.Add("Mike Maignan", "AC Milan", "Kaleci", "90 / 100", "Elit Refleks ve Uzun Pas Kalitesi");
            dt.Rows.Add("Antoine Griezmann", "Atlético Madrid", "On Numara", "90 / 100", "Taktiksel Zeka ve Komple Oyuncu");
            dt.Rows.Add("Viktor Gyökeres", "Sporting CP", "Santrafor", "91 / 100", "Avrupa'nın En Formda Gol Makinesi");


            // =========================================================================
            // YENİ İSTENEN 100 KİŞİLİK DEV EKLEME (10'ar Gruplar Halinde)
            // =========================================================================

            // --- 1. SUDİ ARABİSTAN (10 Oyuncu) ---
            dt.Rows.Add("Salem Al-Najdi", "Al-Nassr", "Sol Bek", "82 / 100", "Gelecek Vaad eden Asya Sol Beki");
            dt.Rows.Add("Firas Al-Buraikan", "Al-Ahli", "Santrafor", "84 / 100", "Yerli Gol Üreticisi");
            dt.Rows.Add("Mohammed Kanno", "Al-Hilal", "Merkez Orta Saha", "84 / 100", "Fiziksel Güç ve Uzaktan Şut");
            dt.Rows.Add("Abdulelah Al-Amri", "Al-Ittihad", "Stoper", "83 / 100", "Sert Müdahaleler ve Hava Topu");
            dt.Rows.Add("Sultan Al-Ghannam", "Al-Nassr", "Sağ Bek", "85 / 100", "Bindirmeleriyle Etkili Sağ Bek");
            dt.Rows.Add("Hassan Kadesh", "Al-Ittihad", "Stoper", "81 / 100", "Tecrübeli Yerli Stoper");
            dt.Rows.Add("Nasser Al-Dawsari", "Al-Hilal", "Sol Orta Saha", "82 / 100", "Çok Yönlü Joker Oyuncu");
            dt.Rows.Add("Abdulrahman Ghareeb", "Al-Nassr", "Sol Kanat", "84 / 100", "Çevik ve Çalım Atabilen Kanat");
            dt.Rows.Add("Ali Al-Bulaihi", "Al-Hilal", "Stoper", "83 / 100", "Agresif Savunma Karakteri");
            dt.Rows.Add("Abdullah Al-Mayouf", "Al-Ittihad", "Kaleci", "81 / 100", "Kıdemli Çizgi Kalecisi");

            // --- 2. İNGİLTERE (10 Oyuncu) ---
            dt.Rows.Add("Bukayo Saka (ENG)", "Arsenal", "Sağ Kanat", "92 / 100", "Premier Lig Kanat Yıldızı");
            dt.Rows.Add("Ollie Watkins", "Aston Villa", "Santrafor", "89 / 100", "Pres Gücü ve Yüksek Skor Katkısı");
            dt.Rows.Add("Anthony Gordon", "Newcastle United", "Sol Kanat", "87 / 100", "Patlayıcı Hız ve Delicilik");
            dt.Rows.Add("John Stones", "Manchester City", "Stoper", "91 / 100", "Orta Sahaya Evrilen Kusursuz Stoper");
            dt.Rows.Add("Jordan Pickford", "Everton", "Kaleci", "86 / 100", "Refleksler ve Uzun Top Başarısı");
            dt.Rows.Add("Jarrod Bowen", "West Ham United", "Sağ Kanat", "88 / 100", "İstikrarlı Skor Üreticisi");
            dt.Rows.Add("Kobbie Mainoo", "Manchester United", "Merkez Orta Saha", "88 / 100", "Baskı Altında Soğukkanlı Deha");
            dt.Rows.Add("Eberechi Eze", "Crystal Palace", "On Numara", "89 / 100", "Büyüleyici Çalım ve Estetik");
            dt.Rows.Add("Marc Guéhi", "Crystal Palace", "Stoper", "87 / 100", "Güvenilir Genç İngiliz Savunmacı");
            dt.Rows.Add("James Maddison", "Tottenham", "On Numara", "88 / 100", "Kilit Pas ve Duran Top Ustası");

            // --- 3. ALMANYA (10 Oyuncu) ---
            dt.Rows.Add("Florian Wirtz (GER)", "Bayer Leverkusen", "On Numara", "93 / 100", "Alman Futbolunun Akıl Hocası");
            dt.Rows.Add("Leroy Sane", "Bayern München", "Sağ Kanat", "89 / 100", "Fırtına Gibi Delici Sürat");
            dt.Rows.Add("Antonio Rüdiger", "Real Madrid", "Stoper", "91 / 100", "Sert, Korkusuz ve Lider");
            dt.Rows.Add("Kai Havertz", "Arsenal", "Santrafor", "89 / 100", "Sahte 9 ve Akıllı Koşular");
            dt.Rows.Add("Joshua Kimmich", "Bayern München", "Defansif Orta Saha", "90 / 100", "Taktiksel Zeka ve Pres Makinesi");
            dt.Rows.Add("Jonathan Tah", "Bayer Leverkusen", "Stoper", "88 / 100", "Kaya Gibi Fiziksel Stoper");
            dt.Rows.Add("Niclas Füllkrug", "West Ham United", "Santrafor", "85 / 100", "Klasik Ceza Sahası Pivotu");
            dt.Rows.Add("Julian Brandt", "Borussia Dortmund", "On Numara", "87 / 100", "Yaratıcı Hücum Silahı");
            dt.Rows.Add("Karim Adeyemi", "Borussia Dortmund", "Sol Kanat", "86 / 100", "Durdurulamaz Sprint Yeteneği");
            dt.Rows.Add("Maximilian Beier", "Borussia Dortmund", "Santrafor", "85 / 100", "Gelişime Açık Alman Forvet");

            // --- 4. İSPANYA (10 Oyuncu) ---
            dt.Rows.Add("Lamine Yamal (ESP)", "Barcelona", "Sağ Kanat", "94 / 100", "Asrın Çalım ve Oyun Dehası");
            dt.Rows.Add("Dani Olmo", "Barcelona", "On Numara", "90 / 100", "Çok Yönlü Teknik Komutan");
            dt.Rows.Add("Nico Williams", "Athletic Bilbao", "Sol Kanat", "90 / 100", "Fırtına Kanat / Birebir Canavarı");
            dt.Rows.Add("Pedri", "Barcelona", "Merkez Orta Saha", "91 / 100", "Zarif Pas Kontrolü ve Vizyon");
            dt.Rows.Add("Gavi", "Barcelona", "Merkez Orta Saha", "89 / 100", "Agresif Mücadele ve Yüksek Enerji");
            dt.Rows.Add("Alejandro Grimaldo", "Bayer Leverkusen", "Sol Bek", "90 / 100", "Falsolu Orta ve Golcü Bek");
            dt.Rows.Add("Dani Carvajal", "Real Madrid", "Sağ Bek", "89 / 100", "Kazanma Karakteri ve Tecrübe");
            dt.Rows.Add("Unai Simón", "Athletic Bilbao", "Kaleci", "87 / 100", "Güvenilir İspanyol File Bekçisi");
            dt.Rows.Add("Yeremy Pino", "Villarreal", "Sağ Kanat", "85 / 100", "Çevik ve Teknik İspanyol Kanat");
            dt.Rows.Add("Pau Cubarsí", "Barcelona", "Stoper", "88 / 100", "Genç Yaşına Rağmen Muazzam Oyun Kurma");

            // --- 5. İTALYA (10 Oyuncu) ---
            dt.Rows.Add("Nicolò Barella", "Inter", "Merkez Orta Saha", "91 / 100", "Orta Sahanın Dinamosu ve Kalbi");
            dt.Rows.Add("Alessandro Bastoni", "Inter", "Stoper", "90 / 100", "Solak Oyun Kurucu Modern Stoper");
            dt.Rows.Add("Federico Dimarco", "Inter", "Sol Bek", "89 / 100", "Mükemmel Sol Ayak ve Ortalar");
            dt.Rows.Add("Gianluigi Donnarumma", "PSG", "Kaleci", "90 / 100", "Dev Boyu ve Refleksleriyle Duvar");
            dt.Rows.Add("Federico Chiesa", "Liverpool", "Sol Kanat", "88 / 100", "Patlayıcı Güç ve Uzaktan Şut");
            dt.Rows.Add("Riccardo Calafiori", "Arsenal", "Stoper", "89 / 100", "Estetik Çıkışlar Yapan Stoper");
            dt.Rows.Add("Sandro Tonali", "Newcastle United", "Defansif Orta Saha", "88 / 100", "İki Yönlü Agresif Orta Saha");
            dt.Rows.Add("Mateo Retegui", "Atalanta", "Santrafor", "87 / 100", "Fırsatçı ve Formda İtalyan Forvet");
            dt.Rows.Add("Davide Frattesi", "Inter", "Merkez Orta Saha", "86 / 100", "İkinci Yarıda Ceza Sahasına Koşu Atma Ustası");
            dt.Rows.Add("Lorenzo Pellegrini", "Roma", "On Numara", "86 / 100", "Kaptanlık ve Teknik Liderlik");

            // --- 6. FRANSA (10 Oyuncu) ---
            dt.Rows.Add("Kylian Mbappé (FRA)", "Real Madrid", "Sol Kanat", "96 / 100", "Asrın Sürat ve Bitiricilik Abidesi");
            dt.Rows.Add("Antoine Griezmann (FRA)", "Atlético Madrid", "On Numara", "90 / 100", "Taktiksel Deha ve Akıl");
            dt.Rows.Add("William Saliba (FRA)", "Arsenal", "Stoper", "91 / 100", "Hatasız Savunma Duvarı");
            dt.Rows.Add("Ousmane Dembélé", "PSG", "Sağ Kanat", "89 / 100", "Tahmin Edilemez Çalım Yeteneği");
            dt.Rows.Add("Eduardo Camavinga", "Real Madrid", "Merkez Orta Saha", "89 / 100", "Joker ve Top Kazanma Uzmanı");
            dt.Rows.Add("Aurélien Tchouaméni", "Real Madrid", "Defansif Orta Saha", "89 / 100", "Sert Kesici ve Fizik Gücü");
            dt.Rows.Add("Bradley Barcola", "PSG", "Sol Kanat", "88 / 100", "Çıkış Yakalayan Parlayan Yıldız");
            dt.Rows.Add("Michael Olise", "Bayern München", "Sağ Kanat", "89 / 100", "Sol Ayak Estetiği ve Kilit Pas");
            dt.Rows.Add("Dayot Upamecano", "Bayern München", "Stoper", "87 / 100", "Güçlü ve Atletik Fransız Savunmacı");
            dt.Rows.Add("Marcus Thuram", "Inter", "Santrafor", "88 / 100", "Güçlü, Fizikli ve Bağlantı Kurabilen Forvet");

            // --- 7. GÜNEY KORE VE JAPONYA (10 Oyuncu) ---
            dt.Rows.Add("Son Heung-min", "Tottenham", "Sol Kanat", "90 / 100", "Asya'nın Gururu / İki Ayaklı Usta");
            dt.Rows.Add("Kim Min-jae", "Bayern München", "Stoper", "89 / 100", "Canavar Lakaplı Sert Stoper");
            dt.Rows.Add("Lee Kang-in", "PSG", "On Numara", "87 / 100", "Koreli Yaratıcı Çalım ve Pas");
            dt.Rows.Add("Hwang Hee-chan", "Wolverhampton", "Sol Kanat", "85 / 100", "Süratli ve Presci Hücumcu");
            dt.Rows.Add("Kaoru Mitoma", "Brighton", "Sol Kanat", "88 / 100", "Çizgide Dışarıdan Çalım Uzmanı");
            dt.Rows.Add("Takefusa Kubo", "Real Sociedad", "Sağ Kanat", "88 / 100", "Japonya'nın Teknik Komutanı");
            dt.Rows.Add("Wataru Endo", "Liverpool", "Defansif Orta Saha", "86 / 100", "Japonya'nın Savaşçı Kesicisi");
            dt.Rows.Add("Takumi Minamino", "Monaco", "On Numara", "85 / 100", "Akıllı Koşular ve Son Vuruş");
            dt.Rows.Add("Ko Itakura", "Borussia Mönchengladbach", "Stoper", "84 / 100", "Disiplinli ve Temiz Savunma");
            dt.Rows.Add("Daichi Kamada", "Crystal Palace", "On Numara", "85 / 100", "Boşlukları Gören Akıllı Orta Saha");

            // --- 8. RUSYA VE UKRAYNA (10 Oyuncu) ---
            dt.Rows.Add("Aleksandr Golovin", "Monaco", "On Numara", "87 / 100", "Rusya'nın Teknik Lideri ve Falsolu Şutları");
            dt.Rows.Add("Arsen Zakharyan", "Real Sociedad", "On Numara", "84 / 100", "Gelişime Açık Rus Yetenek");
            dt.Rows.Add("Aleksandr Sobolev", "Zenit", "Santrafor", "82 / 100", "Fiziksel Güçlü Rus Pivot");
            dt.Rows.Add("Matvei Safonov", "PSG", "Kaleci", "85 / 100", "Refleksleri Güçlü Rus Eldiven");
            dt.Rows.Add("Ivan Oblyakov", "CSKA Moscow", "Merkez Orta Saha", "82 / 100", "Çalışkan ve Çok Yönlü");
            dt.Rows.Add("Oleksandr Zinchenko", "Arsenal", "Sol Bek", "87 / 100", "İçeri Kat Eden Oyun Kurucu Bek");
            dt.Rows.Add("Mykhailo Mudryk", "Chelsea", "Sol Kanat", "85 / 100", "Fırtına Gibi Patlayıcı Sürat");
            dt.Rows.Add("Artem Dovbyk", "Roma", "Santrafor", "89 / 100", "Ukraynalı Gol Makinesi / Güçlü Fizik");
            dt.Rows.Add("Viktor Tsyhankov", "Girona", "Sağ Kanat", "87 / 100", "Etkili Sol Ayak ve İçeri Kat Etme");
            dt.Rows.Add("Anatoliy Trubin", "Benfica", "Kaleci", "86 / 100", "Ukraynalı Genç ve Dev Kaleci");

            // --- 9. SLAV ÜLKELERİ / İSKANDİNAVYA (Norveç, İsveç, Finlandiya vb.) (10 Oyuncu) ---
            dt.Rows.Add("Erling Haaland (NOR)", "Manchester City", "Santrafor", "95 / 100", "Durdurulamaz İskandinav Gol Canavarı");
            dt.Rows.Add("Martin Ødegaard (NOR)", "Arsenal", "On Numara", "91 / 100", "Norveç'in Sahadaki Beyni");
            dt.Rows.Add("Alexander Isak (SWE)", "Newcastle United", "Santrafor", "89 / 100", "İsveçli Zarif ve Bitirici Forvet");
            dt.Rows.Add("Dejan Kulusevski (SWE)", "Tottenham", "Sağ Kanat", "88 / 100", "Güçlü Fizik ve Çalım");
            dt.Rows.Add("Viktor Gyökeres (SWE)", "Sporting CP", "Santrafor", "91 / 100", "Avrupa'yı Sallayan İsveçli Golcü");
            dt.Rows.Add("Lukas Hradecky", "Bayer Leverkusen", "Kaleci", "85 / 100", "Finlandiyalı Tecrübeli Kaptan");
            dt.Rows.Add("Antonio Nusa", "RB Leipzig", "Sol Kanat", "84 / 100", "Norveçli Müthiş Wonderkid");
            dt.Rows.Add("Julian Ryerson", "Borussia Dortmund", "Sağ Bek", "84 / 100", "Norveçli Savaşçı ve Agresif Bek");
            dt.Rows.Add("Glen Kamara", "Stade Rennais", "Merkez Orta Saha", "83 / 100", "Finlandiyalı Top Saklama Ustası");
            dt.Rows.Add("Jens Cajuste", "Ipswich Town", "Merkez Orta Saha", "82 / 100", "İsveçli Dinamik Güç");

            // --- 10. BELÇİKA VE HOLLANDA (10 Oyuncu) ---
            dt.Rows.Add("Kevin De Bruyne (BEL)", "Manchester City", "On Numara", "93 / 100", "Dünyanın En İyi Pasörü / Maestro");
            dt.Rows.Add("Jeremy Doku (BEL)", "Manchester City", "Sol Kanat", "89 / 100", "Birebirde Rakibi İpe Dizen Sürat");
            dt.Rows.Add("Amadou Onana (BEL)", "Aston Villa", "Defansif Orta Saha", "88 / 100", "Dev Fizik ve Top Kazanma");
            dt.Rows.Add("Loïs Openda (BEL)", "RB Leipzig", "Santrafor", "88 / 100", "Fırtına Gibi Hızlı Belçikalı Forvet");
            dt.Rows.Add("Thibaut Courtois (BEL)", "Real Madrid", "Kaleci", "92 / 100", "Dünyanın En İyi Kalecilerinden");
            dt.Rows.Add("Virgil van Dijk (NED)", "Liverpool", "Stoper", "91 / 100", "Hollanda ve Dünya Futbolunun Lideri");
            dt.Rows.Add("Frenkie de Jong", "Barcelona", "Merkez Orta Saha", "89 / 100", "Topu Savunmadan Zarif Çıkaran Dahi");
            dt.Rows.Add("Xavi Simons", "RB Leipzig", "On Numara", "89 / 100", "Hollandalı Yaratıcı ve Çevik Yıldız");
            dt.Rows.Add("Cody Gakpo", "Liverpool", "Sol Kanat", "88 / 100", "Kavisli Şutlar ve Etkili Kanat");
            dt.Rows.Add("Nathan Aké", "Manchester City", "Stoper", "88 / 100", "Çok Yönlü Güvenilir Savunmacı");

            dgvScoutReports.DataSource = dt;
        }
    }
}