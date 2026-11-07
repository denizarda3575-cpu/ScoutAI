using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ScoutAI.DAL;
using ScoutAI.Entities;

namespace ScoutAI.UI
{
    public partial class PlayerAddForm : Form
    {
        private readonly PlayerRepository _playerRepository = new PlayerRepository();
        private readonly TeamRepository _teamRepository = new TeamRepository();

        private TextBox txtFullName;
        private ComboBox cmbTeams;
        private ComboBox cmbPosition;
        private ComboBox cmbAge;
        private TextBox txtMarketValue;
        private Button btnSavePlayer;

        public PlayerAddForm()
        {
            InitializeComponentCustom();
            LoadDropdownData();
        }

        private void InitializeComponentCustom()
        {
            this.Text = "Scout AI - Yeni Oyuncu Ekle";
            this.Size = new Size(420, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(28, 43, 65);

            Label lblTitle = new Label()
            {
                Text = "YENİ OYUNCU EKLE",
                Left = 0,
                Top = 20,
                Width = 400,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(46, 204, 113),
                Font = new Font("Arial", 16, FontStyle.Bold)
            };

            int lblX = 40;
            int txtX = 40;
            int startY = 70;

            // Ad Soyad
            Label lbl1 = new Label() { Text = "Ad Soyad", Left = lblX, Top = startY, Width = 320, ForeColor = Color.LightGray, Font = new Font("Arial", 9, FontStyle.Bold) };
            txtFullName = new TextBox() { Left = txtX, Top = startY + 22, Width = 320, Height = 28, Font = new Font("Arial", 10) };

            // Takım Seçimi
            Label lbl2 = new Label() { Text = "Takımı", Left = lblX, Top = startY + 60, Width = 320, ForeColor = Color.LightGray, Font = new Font("Arial", 9, FontStyle.Bold) };
            cmbTeams = new ComboBox() { Left = txtX, Top = startY + 82, Width = 320, Height = 28, Font = new Font("Arial", 10), DropDownStyle = ComboBoxStyle.DropDownList };

            // Mevki Seçimi
            Label lbl3 = new Label() { Text = "Mevki", Left = lblX, Top = startY + 120, Width = 320, ForeColor = Color.LightGray, Font = new Font("Arial", 9, FontStyle.Bold) };
            cmbPosition = new ComboBox() { Left = txtX, Top = startY + 142, Width = 320, Height = 28, Font = new Font("Arial", 10), DropDownStyle = ComboBoxStyle.DropDownList };

            // Yaş Seçimi (15 - 45 Arası Açılır Liste ve Açıklamalar)
            Label lbl4 = new Label() { Text = "Yaş (15 - 45)", Left = lblX, Top = startY + 180, Width = 320, ForeColor = Color.LightGray, Font = new Font("Arial", 9, FontStyle.Bold) };
            cmbAge = new ComboBox() { Left = txtX, Top = startY + 202, Width = 320, Height = 28, Font = new Font("Arial", 10), DropDownStyle = ComboBoxStyle.DropDownList };

            // Piyasa Değeri
            Label lbl5 = new Label() { Text = "Piyasa Değeri (Milyon € olarak - Örn: 180 veya 0.5)", Left = lblX, Top = startY + 240, Width = 320, ForeColor = Color.LightGray, Font = new Font("Arial", 8.5f, FontStyle.Bold) };
            txtMarketValue = new TextBox() { Left = txtX, Top = startY + 262, Width = 320, Height = 28, Font = new Font("Arial", 10), Text = "30" };

            btnSavePlayer = new Button()
            {
                Text = "Oyuncuyu Kaydet ve AI İstatistik Üret",
                Left = txtX,
                Top = startY + 310,
                Width = 320,
                Height = 42,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSavePlayer.FlatAppearance.BorderSize = 0;
            btnSavePlayer.Click += BtnSavePlayer_Click;

            this.Controls.Add(lblTitle);
            this.Controls.Add(lbl1);
            this.Controls.Add(txtFullName);
            this.Controls.Add(lbl2);
            this.Controls.Add(cmbTeams);
            this.Controls.Add(lbl3);
            this.Controls.Add(cmbPosition);
            this.Controls.Add(lbl4);
            this.Controls.Add(cmbAge);
            this.Controls.Add(lbl5);
            this.Controls.Add(txtMarketValue);
            this.Controls.Add(btnSavePlayer);
        }

        private void LoadDropdownData()
        {
            // 1. Takımlar (Sadece giriş yapan kullanıcının kendi takımları filtreleniyor)
            cmbTeams.Items.Clear();
            cmbTeams.Items.Add(new KeyValuePair<int, string>(0, "— Serbest Oyuncu —"));

            var userTeams = _teamRepository.GetTeamsByUserId();
            foreach (var team in userTeams)
            {
                cmbTeams.Items.Add(new KeyValuePair<int, string>(team.TeamId, team.TeamName));
            }

            cmbTeams.DisplayMember = "Value";
            cmbTeams.ValueMember = "Key";
            cmbTeams.SelectedIndex = 0;

            // 2. Mevkiler
            cmbPosition.Items.Add("Santrafor (ST)");
            cmbPosition.Items.Add("Sağ Kanat (RW)");
            cmbPosition.Items.Add("Sol Kanat (LW)");
            cmbPosition.Items.Add("Merkez Orta Saha (CM)");
            cmbPosition.Items.Add("On Numara (CAM)");
            cmbPosition.Items.Add("Defansif Orta Saha (CDM)");
            cmbPosition.Items.Add("Stoper (CB)");
            cmbPosition.Items.Add("Sağ Bek (RB)");
            cmbPosition.Items.Add("Sol Bek (LB)");
            cmbPosition.Items.Add("Kaleci (GK)");
            cmbPosition.SelectedIndex = 0;

            // 3. Yaş Aralığı (15 - 45) ve Açıklamalar
            cmbAge.Items.Clear();
            for (int i = 15; i <= 45; i++)
            {
                string desc = "";
                if (i >= 15 && i <= 20) desc = "Wonderkid / Genç Yetenek";
                else if (i >= 21 && i <= 24) desc = "Gelişime Açık";
                else if (i >= 25 && i <= 28) desc = "En Verimli Dönem / Prime";
                else if (i >= 29 && i <= 33) desc = "Olgun Yıldız / Deneyimli";
                else desc = "Kıdemli / Jübile Aşaması";

                cmbAge.Items.Add(new KeyValuePair<int, string>(i, $"{i} Yaş - {desc}"));
            }
            cmbAge.DisplayMember = "Value";
            cmbAge.ValueMember = "Key";
            cmbAge.SelectedIndex = 10; // Varsayılan 25 yaş
        }

        private void BtnSavePlayer_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFullName.Text.Trim()))
            {
                MessageBox.Show("Lütfen futbolcunun adını ve soyadını giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var selectedTeam = (KeyValuePair<int, string>)cmbTeams.SelectedItem;
                var selectedAgePair = (KeyValuePair<int, string>)cmbAge.SelectedItem;

                decimal parsedValue = decimal.Parse(txtMarketValue.Text.Trim().Replace(".", ","));

                Player newPlayer = new Player
                {
                    TeamId = selectedTeam.Key,
                    FullName = txtFullName.Text.Trim(),
                    Position = cmbPosition.SelectedItem.ToString(),
                    Age = selectedAgePair.Key,
                    MarketValue = parsedValue * 1000000m
                };

                bool success = _playerRepository.AddPlayer(newPlayer);
                if (success)
                {
                    MessageBox.Show("Futbolcu başarıyla eklendi ve AI algoritması özel istatistikler üretti!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Futbolcu eklenirken bir hata oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lütfen piyasa değerine geçerli bir sayı giriniz (Örn: 30 veya 0.5): " + ex.Message, "Geçersiz Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}