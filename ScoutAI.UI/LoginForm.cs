using System;
using System.Drawing;
using System.Windows.Forms;
using ScoutAI.BLL;
using ScoutAI.DAL;
using ScoutAI.Helpers;

namespace ScoutAI.UI
{
    public partial class LoginForm : Form
    {
        private readonly AuthService _authService = new AuthService();
        private readonly UserRepository _userRepository = new UserRepository();
        private readonly EmailService _emailService = new EmailService();

        // Modern Arayüz Bileşenleri ve Kart Yapısı
        private Panel pnlCard;
        private Label lblTitle;
        private TextBox txtEmail;
        private TextBox txtPassword;
        private TextBox txtSecurityAnswer;
        private Button btnLogin;
        private Button btnRegisterOrGetTemp;
        private Button btnForgotPassword;
        private Label lblEmail;
        private Label lblPassword;
        private Label lblSecurityQuestion;

        private System.Windows.Forms.Timer cooldownTimer;
        private int remainingSeconds = 180;
        private bool isCooldownActive = false;
        private int failedAttemptCount = 0;
        private const int MaxAllowedAttempts = 3;

        public LoginForm()
        {
            InitializeComponentCustom();
            InitializeCooldownTimer();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
        }

        private void InitializeComponentCustom()
        {
            this.Text = "Scout AI - Güvenli Giriş Paneli";
            this.WindowState = FormWindowState.Maximized; // Tam ekran modern görünüm
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(20, 25, 35); // Arka plan

            // Şık füme kart paneli
            pnlCard = new Panel()
            {
                Width = 460,
                Height = 440,
                BackColor = Color.FromArgb(45, 52, 64)
            };
            pnlCard.Location = new Point((this.ClientSize.Width - pnlCard.Width) / 2, (this.ClientSize.Height - pnlCard.Height) / 2);
            this.Resize += (s, e) => {
                if (pnlCard != null)
                    pnlCard.Location = new Point((this.ClientSize.Width - pnlCard.Width) / 2, (this.ClientSize.Height - pnlCard.Height) / 2);
            };

            // Başlık: SCOUT AI
            lblTitle = new Label()
            {
                Text = "SCOUT AI",
                ForeColor = Color.LightGreen,
                Font = new Font("Arial", 16, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Width = pnlCard.Width,
                Top = 20,
                Left = 0
            };

            // E-posta Alanı
            lblEmail = new Label()
            {
                Text = "E-Posta Adresi",
                ForeColor = Color.LightGray,
                Font = new Font("Arial", 9, FontStyle.Bold),
                Left = 45,
                Top = 65,
                AutoSize = true
            };
            txtEmail = new TextBox()
            {
                Left = 45,
                Top = 88,
                Width = 370,
                Height = 28,
                Font = new Font("Arial", 11, FontStyle.Regular)
            };

            // Parola Alanı
            lblPassword = new Label()
            {
                Text = "Şifre",
                ForeColor = Color.LightGray,
                Font = new Font("Arial", 9, FontStyle.Bold),
                Left = 45,
                Top = 135,
                AutoSize = true
            };
            txtPassword = new TextBox()
            {
                Left = 45,
                Top = 158,
                Width = 370,
                Height = 28,
                UseSystemPasswordChar = true,
                Font = new Font("Arial", 11, FontStyle.Regular)
            };

            // İlkokul No (Cevap) Alanı
            lblSecurityQuestion = new Label()
            {
                Text = "İlkokul No (Cevap)",
                ForeColor = Color.LightGray,
                Font = new Font("Arial", 9, FontStyle.Bold),
                Left = 45,
                Top = 205,
                AutoSize = true
            };
            txtSecurityAnswer = new TextBox()
            {
                Left = 45,
                Top = 228,
                Width = 370,
                Height = 28,
                Font = new Font("Arial", 11, FontStyle.Regular)
            };

            // Giriş Yap Butonu
            btnLogin = new Button()
            {
                Text = "Giriş Yap",
                Left = 45,
                Top = 275,
                Width = 370,
                Height = 36,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            // Kayıt Ol / Geçici Şifre Al Butonu
            btnRegisterOrGetTemp = new Button()
            {
                Text = "Kayıt Ol / Geçici Şifre Al",
                Left = 45,
                Top = 322,
                Width = 370,
                Height = 32,
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 8.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRegisterOrGetTemp.FlatAppearance.BorderSize = 0;
            btnRegisterOrGetTemp.Click += BtnRegisterOrGetTemp_Click;

            // Şifremi Unuttum Butonu
            btnForgotPassword = new Button()
            {
                Text = "Şifremi Unuttum",
                Left = 45,
                Top = 365,
                Width = 370,
                Height = 26,
                BackColor = Color.Transparent,
                ForeColor = Color.DarkGray,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 8f, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btnForgotPassword.FlatAppearance.BorderSize = 0;
            btnForgotPassword.Click += BtnForgotPassword_Click;

            // Bileşenlerin Kart Paneline Eklenmesi
            pnlCard.Controls.Add(lblTitle);
            pnlCard.Controls.Add(lblEmail);
            pnlCard.Controls.Add(txtEmail);
            pnlCard.Controls.Add(lblPassword);
            pnlCard.Controls.Add(txtPassword);
            pnlCard.Controls.Add(lblSecurityQuestion);
            pnlCard.Controls.Add(txtSecurityAnswer);
            pnlCard.Controls.Add(btnLogin);
            pnlCard.Controls.Add(btnRegisterOrGetTemp);
            pnlCard.Controls.Add(btnForgotPassword);

            this.Controls.Add(pnlCard);
            this.AcceptButton = btnLogin;
        }

        private void InitializeCooldownTimer()
        {
            cooldownTimer = new System.Windows.Forms.Timer();
            cooldownTimer.Interval = 1000;
            cooldownTimer.Tick += CooldownTimer_Tick;
        }

        private void CooldownTimer_Tick(object sender, EventArgs e)
        {
            remainingSeconds--;

            if (remainingSeconds > 0)
            {
                int minutes = remainingSeconds / 60;
                int seconds = remainingSeconds % 60;
                btnRegisterOrGetTemp.Text = $"Tekrar Gönder ({minutes:D2}:{seconds:D2})";
            }
            else
            {
                cooldownTimer.Stop();
                isCooldownActive = false;
                remainingSeconds = 180;
                btnRegisterOrGetTemp.Enabled = true;
                btnRegisterOrGetTemp.Text = "Kayıt Ol / Geçici Şifre Al";
            }
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            var user = _authService.Login(email, password, out string errorMessage);

            if (user == null)
            {
                failedAttemptCount++;
                int remainingRights = MaxAllowedAttempts - failedAttemptCount;

                if (failedAttemptCount >= MaxAllowedAttempts)
                {
                    MessageBox.Show("Üst üste 3 kez hatalı şifre girdiğiniz için güvenlik nedeniyle oturum kilitlendi.", "Güvenlik Kilidi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassword.Clear();
                    failedAttemptCount = 0;
                    return;
                }

                MessageBox.Show($"{errorMessage}\nKalan Hatalı Deneme Hakkınız: {remainingRights}", "Giriş Başarısız", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            failedAttemptCount = 0;
            if (cooldownTimer != null) cooldownTimer.Stop();

            string roleName = user.RoleId == 1 ? "SuperAdmin" : (user.RoleId == 2 ? "Admin" : "User");
            CurrentSession.Start(user.UserId, user.Email, user.FirstName + " " + user.LastName, roleName);

            // Veritabanı izolasyonu için aktif kullanıcı ID'si PlayerRepository'ye aktarılıyor
            PlayerRepository.CurrentUserId = user.UserId;

            MessageBox.Show("Giriş başarılı! Hoş geldiniz, " + CurrentSession.FullName, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (user.MustChangePassword)
            {
                ChangePasswordForm changeForm = new ChangePasswordForm();
                changeForm.ShowDialog();
            }

            if (CurrentSession.RoleName == "SuperAdmin" || CurrentSession.RoleName == "Admin")
            {
                AdminMainForm adminForm = new AdminMainForm();
                adminForm.Show();
            }
            else
            {
                UserMainForm userForm = new UserMainForm();
                userForm.Show();
            }

            this.Hide();
        }

        private void BtnRegisterOrGetTemp_Click(object sender, EventArgs e)
        {
            if (isCooldownActive)
            {
                int minutes = remainingSeconds / 60;
                int seconds = remainingSeconds % 60;
                MessageBox.Show($"Tekrar geçici şifre almak için lütfen verilen sürenin bitmesini bekleyiniz!\nKalan Süre: {minutes:D2}:{seconds:D2}",
                    "İşlem Sınırı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string email = txtEmail.Text.Trim();
            string securityAnswer = txtSecurityAnswer.Text.Trim();

            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
            {
                MessageBox.Show("Lütfen önce geçerli bir e-posta adresi yazınız!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var checkUser = _userRepository.GetByEmail(email);

            if (checkUser == null && string.IsNullOrWhiteSpace(securityAnswer))
            {
                MessageBox.Show("Sisteme ilk defa kayıt oluyorsunuz! Şifrenizi daha sonra güvenle kurtarabilmeniz için lütfen 'İlkokul No (Cevap)' alanını doldurunuz.",
                    "Gizli Soru Zorunlu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string tempPassword = PasswordHasher.GenerateRandomPassword();
            var hashResult = PasswordHasher.HashPassword(tempPassword);

            bool success = false;

            if (checkUser == null)
            {
                success = _userRepository.AddUser(email, hashResult.Hash, hashResult.Salt, hashResult.Iterations, "İlkokul numaranız nedir?", securityAnswer);
            }
            else
            {
                success = _userRepository.UpdateTemporaryPassword(checkUser.UserId, hashResult.Hash, hashResult.Salt, hashResult.Iterations);
            }

            if (success)
            {
                failedAttemptCount = 0;
                isCooldownActive = true;
                remainingSeconds = 180;
                btnRegisterOrGetTemp.Enabled = true;
                cooldownTimer.Start();

                if (_emailService.SendTemporaryPassword(email, tempPassword, out string emailError))
                {
                    MessageBox.Show("Geçici parolanız e-posta adresinize gönderildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"E-posta gönderilemedi (SMTP yapılandırması bekleniyor).\nGeçici Parolanız (Test): {tempPassword}", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("İşlem sırasında bir veritabanı hatası oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnForgotPassword_Click(object sender, EventArgs e)
        {
            ForgotPasswordForm forgotForm = new ForgotPasswordForm();
            forgotForm.ShowDialog();
        }
    }
}