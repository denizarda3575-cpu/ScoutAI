using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ScoutAI.DAL;
using ScoutAI.Helpers;

namespace ScoutAI.UI
{
    public partial class AdminMainForm : Form
    {
        private readonly UserRepository _userRepository = new UserRepository();

        // Arayüz Bileşenleri (Modern Tema ve Düzen)
        private Panel pnlTopBar;
        private Label lblTitleHeader;
        private Label lblWelcome;

        private Label lblSearch;
        private TextBox txtSearch;
        private Button btnSearch;
        private DataGridView dgvUsers;

        private Button btnRefresh;
        private Button btnChangeRoleToAdmin;
        private Button btnChangeRoleToUser;
        private Button btnDeleteUser;
        private Button btnAuditLog;
        private Button btnPlayerManagement;
        private Button btnChangePassword;
        private Button btnLogout;

        private Timer inactivityTimer;
        private int idleSeconds = 0;

        public AdminMainForm()
        {
            InitializeComponentCustom();
            InitializeInactivityTimer();
            LoadUsers();
        }

        private void AdminMainForm_Load(object sender, EventArgs e)
        {
        }

        private void InitializeComponentCustom()
        {
            this.Text = "Scout AI - Yönetim Paneli";
            this.WindowState = FormWindowState.Maximized; // Tam ekran modern görünüm
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.BackColor = Color.FromArgb(20, 25, 35); // Koyu siyah/füme tema

            // Üst Bilgi Barı (Koyu Yeşil / Siyah Uyumlu)
            pnlTopBar = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 65,
                BackColor = Color.FromArgb(28, 35, 45)
            };

            // Sol Üst Kırmızı/Turuncu Dikkat Çekici Başlık
            lblTitleHeader = new Label()
            {
                Text = "SCOUT AI - YÖNETİM PANELİ",
                Left = 25,
                Top = 20,
                AutoSize = true,
                ForeColor = Color.FromArgb(231, 76, 60), // Şık kırmızı ton
                Font = new Font("Arial", 13, FontStyle.Bold)
            };

            // Sağ Üst Karşılama Yazısı (Hoş geldiniz, Ad Soyad (Rol))
            lblWelcome = new Label()
            {
                Text = $"Hoş geldiniz, {CurrentSession.FullName} [{CurrentSession.RoleName}]",
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                TextAlign = ContentAlignment.MiddleRight,
                Top = 22,
                Width = 500,
                Height = 25,
                ForeColor = Color.LightGray,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            // Ekran boyutuna göre sağa yaslama konumu
            lblWelcome.Left = Screen.PrimaryScreen.Bounds.Width - 530;

            pnlTopBar.Controls.Add(lblTitleHeader);
            pnlTopBar.Controls.Add(lblWelcome);

            // Arama Bölümü
            lblSearch = new Label()
            {
                Text = "E-posta Ara:",
                Left = 30,
                Top = 95,
                Width = 90,
                ForeColor = Color.LightGray,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };

            txtSearch = new TextBox()
            {
                Left = 125,
                Top = 92,
                Width = 280,
                Height = 28,
                Font = new Font("Arial", 10, FontStyle.Regular)
            };

            btnSearch = new Button()
            {
                Text = "Ara / Filtrele",
                Left = 415,
                Top = 90,
                Width = 120,
                Height = 30,
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += (s, e) => { ResetIdleTime(null, null); LoadUsers(txtSearch.Text.Trim()); };

            // Kullanıcı Tablosu (DataGridView)
            dgvUsers = new DataGridView()
            {
                Left = 30,
                Top = 140,
                Width = 1150,
                Height = 420,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.FromArgb(35, 42, 54),
                ForeColor = Color.Black
            };

            // Alt Yönetim Butonları (Koyu Yeşil / Siyah / Modern Uyumlu)
            int buttonTop = 585;

            btnRefresh = new Button()
            {
                Text = "Listeyi Yenile",
                Left = 30,
                Top = buttonTop,
                Width = 120,
                Height = 38,
                BackColor = Color.FromArgb(127, 140, 141),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 8.5f, FontStyle.Bold)
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += (s, e) => { ResetIdleTime(null, null); txtSearch.Clear(); LoadUsers(); };

            btnChangeRoleToAdmin = new Button()
            {
                Text = "Admin Yap",
                Left = 160,
                Top = buttonTop,
                Width = 110,
                Height = 38,
                BackColor = Color.FromArgb(39, 174, 96), // Koyu Yeşil
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 8.5f, FontStyle.Bold)
            };
            btnChangeRoleToAdmin.FlatAppearance.BorderSize = 0;
            btnChangeRoleToAdmin.Click += (s, e) => ChangeUserRole(2);

            btnChangeRoleToUser = new Button()
            {
                Text = "User Yap",
                Left = 280,
                Top = buttonTop,
                Width = 110,
                Height = 38,
                BackColor = Color.FromArgb(241, 196, 15),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 8.5f, FontStyle.Bold)
            };
            btnChangeRoleToUser.FlatAppearance.BorderSize = 0;
            btnChangeRoleToUser.Click += (s, e) => ChangeUserRole(3);

            btnDeleteUser = new Button()
            {
                Text = "Kullanıcıyı Sil",
                Left = 400,
                Top = buttonTop,
                Width = 120,
                Height = 38,
                BackColor = Color.FromArgb(192, 57, 43),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 8.5f, FontStyle.Bold)
            };
            btnDeleteUser.FlatAppearance.BorderSize = 0;
            btnDeleteUser.Click += BtnDeleteUser_Click;

            btnAuditLog = new Button()
            {
                Text = "Denetim (Listele)",
                Left = 530,
                Top = buttonTop,
                Width = 135,
                Height = 38,
                BackColor = Color.FromArgb(142, 68, 173),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 8.5f, FontStyle.Bold)
            };
            btnAuditLog.FlatAppearance.BorderSize = 0;
            btnAuditLog.Click += (s, e) => {
                ResetIdleTime(null, null);
                MessageBox.Show($"[DENETİM RAPORU - {CurrentSession.Email}]\n• Rol: {CurrentSession.RoleName}\n• Durum: Aktif Güvenli Oturum", "Denetim");
            };

            btnPlayerManagement = new Button()
            {
                Text = "Futbolcu Yönetimi",
                Left = 675,
                Top = buttonTop,
                Width = 145,
                Height = 38,
                BackColor = Color.FromArgb(211, 84, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 8.5f, FontStyle.Bold)
            };
            btnPlayerManagement.FlatAppearance.BorderSize = 0;
            btnPlayerManagement.Click += (s, e) => { ResetIdleTime(null, null); new PlayerManagementForm().ShowDialog(); };

            btnChangePassword = new Button()
            {
                Text = "Şifremi Değiştir",
                Left = 830,
                Top = buttonTop,
                Width = 135,
                Height = 38,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 8.5f, FontStyle.Bold)
            };
            btnChangePassword.FlatAppearance.BorderSize = 0;
            btnChangePassword.Click += (s, e) => { ResetIdleTime(null, null); new ChangePasswordForm().ShowDialog(); };

            // İstediğin gibi sağ alta konumlandırılan şık Oturumu Kapat butonu
            btnLogout = new Button()
            {
                Text = "Oturumu Kapat",
                Left = 975,
                Top = buttonTop,
                Width = 145,
                Height = 38,
                BackColor = Color.FromArgb(44, 62, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 8.5f, FontStyle.Bold)
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += (s, e) => LogoutSystem();

            // Kontrollerin Form Üzerine Eklenmesi
            this.Controls.Add(pnlTopBar);
            this.Controls.Add(lblSearch);
            this.Controls.Add(txtSearch);
            this.Controls.Add(btnSearch);
            this.Controls.Add(dgvUsers);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(btnChangeRoleToAdmin);
            this.Controls.Add(btnChangeRoleToUser);
            this.Controls.Add(btnDeleteUser);
            this.Controls.Add(btnAuditLog);
            this.Controls.Add(btnPlayerManagement);
            this.Controls.Add(btnChangePassword);
            this.Controls.Add(btnLogout);

            this.MouseMove += ResetIdleTime;
            this.KeyPress += ResetIdleTime;
        }

        private void LoadUsers(string searchFilter = "")
        {
            try
            {
                var users = _userRepository.GetAllUsers();
                DataTable dt = new DataTable();
                dt.Columns.Add("UserId", typeof(int));
                dt.Columns.Add("Email", typeof(string));
                dt.Columns.Add("FirstName", typeof(string));
                dt.Columns.Add("LastName", typeof(string));
                dt.Columns.Add("RoleId", typeof(int));
                dt.Columns.Add("RoleName", typeof(string));
                dt.Columns.Add("MustChangePassword", typeof(bool));

                foreach (var u in users)
                {
                    if (string.IsNullOrEmpty(searchFilter) || u.Email.ToLower().Contains(searchFilter.ToLower()))
                    {
                        string rName = u.RoleId == 1 ? "SuperAdmin" : (u.RoleId == 2 ? "Admin" : "User");
                        dt.Rows.Add(u.UserId, u.Email, u.FirstName, u.LastName, u.RoleId, rName, u.MustChangePassword);
                    }
                }
                dgvUsers.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void ChangeUserRole(int newRoleId)
        {
            ResetIdleTime(null, null);
            if (dgvUsers.SelectedRows.Count == 0) { MessageBox.Show("Lütfen bir kullanıcı seçiniz."); return; }
            int targetUserId = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["UserId"].Value);
            string targetEmail = dgvUsers.SelectedRows[0].Cells["Email"].Value.ToString();

            if (targetEmail.Equals(CurrentSession.Email, StringComparison.OrdinalIgnoreCase) && CurrentSession.RoleName == "SuperAdmin")
            {
                MessageBox.Show("SuperAdmin kendi rolünü değiştiremez!"); return;
            }
            _userRepository.UpdateUserRole(targetUserId, newRoleId);
            LoadUsers();
        }

        private void BtnDeleteUser_Click(object sender, EventArgs e)
        {
            ResetIdleTime(null, null);
            if (dgvUsers.SelectedRows.Count == 0) { MessageBox.Show("Lütfen bir kullanıcı seçiniz."); return; }
            int targetUserId = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["UserId"].Value);
            string targetEmail = dgvUsers.SelectedRows[0].Cells["Email"].Value.ToString();

            if (targetEmail.Equals(CurrentSession.Email, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Kendi hesabınızı silemezsiniz!"); return;
            }
            if (MessageBox.Show("Kullanıcıyı silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _userRepository.DeleteUser(targetUserId);
                LoadUsers();
            }
        }

        private void InitializeInactivityTimer()
        {
            inactivityTimer = new Timer { Interval = 1000 };
            inactivityTimer.Tick += (s, e) => {
                idleSeconds++;
                if (idleSeconds >= 300) LogoutSystem();
            };
            inactivityTimer.Start();
        }

        private void ResetIdleTime(object sender, EventArgs e) { idleSeconds = 0; }

        private void LogoutSystem()
        {
            if (inactivityTimer != null) inactivityTimer.Stop();
            CurrentSession.Clear();
            this.Close();
            new LoginForm().Show();
        }
    }
}