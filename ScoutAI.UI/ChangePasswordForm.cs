using System;
using System.Drawing;
using System.Windows.Forms;
using ScoutAI.DAL;
using ScoutAI.Helpers;

namespace ScoutAI.UI
{
    public partial class ChangePasswordForm : Form
    {
        private readonly UserRepository _userRepository = new UserRepository();

        private Label lblOldPassword;
        private TextBox txtOldPassword;
        private Label lblNewPassword;
        private TextBox txtNewPassword;
        private Label lblConfirmPassword;
        private TextBox txtConfirmPassword;
        private Button btnSave;

        public ChangePasswordForm()
        {
            InitializeComponentCustom();
        }

        private void InitializeComponentCustom()
        {
            this.Text = "Scout AI - Güvenli Parola Yenileme";
            this.Size = new Size(400, 320);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            lblOldPassword = new Label() { Text = "Mevcut Parola:", Left = 30, Top = 30, Width = 120 };
            txtOldPassword = new TextBox() { Left = 160, Top = 28, Width = 180, UseSystemPasswordChar = true };

            lblNewPassword = new Label() { Text = "Yeni Parola:", Left = 30, Top = 80, Width = 120 };
            txtNewPassword = new TextBox() { Left = 160, Top = 78, Width = 180, UseSystemPasswordChar = true };

            lblConfirmPassword = new Label() { Text = "Yeni Parola (Tekrar):", Left = 30, Top = 130, Width = 120 };
            txtConfirmPassword = new TextBox() { Left = 160, Top = 128, Width = 180, UseSystemPasswordChar = true };

            btnSave = new Button() { Text = "Parolayı Güncelle", Left = 160, Top = 180, Width = 180, Height = 40, BackColor = Color.LightGreen, Font = new Font("Arial", 9, FontStyle.Bold) };
            btnSave.Click += BtnSave_Click;

            this.Controls.Add(lblOldPassword);
            this.Controls.Add(txtOldPassword);
            this.Controls.Add(lblNewPassword);
            this.Controls.Add(txtNewPassword);
            this.Controls.Add(lblConfirmPassword);
            this.Controls.Add(txtConfirmPassword);
            this.Controls.Add(btnSave);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string oldPass = txtOldPassword.Text;
            string newPass = txtNewPassword.Text;
            string confirmPass = txtConfirmPassword.Text;

            if (string.IsNullOrEmpty(oldPass) || string.IsNullOrEmpty(newPass) || string.IsNullOrEmpty(confirmPass))
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPass != confirmPass)
            {
                MessageBox.Show("Yeni parolalar birbiriyle uyuşmuyor!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Parola Politika Kontrolü
            if (!PasswordPolicyValidator.IsValid(newPass, out string policyError))
            {
                MessageBox.Show(policyError, "Zayıf Parola", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var user = _userRepository.GetByEmail(CurrentSession.Email);
            if (user == null)
            {
                MessageBox.Show("Oturum açmış kullanıcı bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool isOldValid = PasswordHasher.Verify(oldPass, user.PasswordHash, user.PasswordSalt, user.Iterations);
            if (!isOldValid)
            {
                MessageBox.Show("Mevcut parolanızı yanlış girdiniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (oldPass == newPass)
            {
                MessageBox.Show("Yeni parolanız mevcut parolanızla aynı olamaz!", "Güvenlik Politikası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_userRepository.CheckPasswordHistory(user.UserId, newPass))
            {
                MessageBox.Show("Güvenlik politikası gereği, yeni belirlediğiniz parola son kullandığınız 3 paroladan biri olamaz!", "Güvenlik İhlali", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var newHashResult = PasswordHasher.HashPassword(newPass);

            _userRepository.AddPasswordHistory(user.UserId, user.PasswordHash, user.PasswordSalt);

            // DÜZELTME BURADA: Şifreyi güncellerken MustChangePassword bayrağını da sıfırlıyoruz
            bool success = _userRepository.UpdatePasswordAndClearFlag(user.UserId, newHashResult.Hash, newHashResult.Salt, newHashResult.Iterations);

            if (success)
            {
                MessageBox.Show("Parolanız başarıyla güncellendi! Güvenliğiniz için lütfen yeni parolanızla tekrar giriş yapınız.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CurrentSession.Clear();
                this.Close();
                Application.Restart();
            }
            else
            {
                MessageBox.Show("Parola güncellenirken bir veritabanı hatası oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}