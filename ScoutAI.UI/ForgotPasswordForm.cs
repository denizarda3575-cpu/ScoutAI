using System;
using System.Drawing;
using System.Windows.Forms;
using ScoutAI.BLL;
using ScoutAI.DAL;
using ScoutAI.Helpers;

namespace ScoutAI.UI
{
    public partial class ForgotPasswordForm : Form
    {
        private readonly UserRepository _userRepository = new UserRepository();
        private readonly EmailService _emailService = new EmailService();

        private Label lblEmail;
        private TextBox txtEmail;
        private Button btnGetQuestion;
        private Label lblQuestion;
        private Label lblQuestionText;
        private Label lblAnswer;
        private TextBox txtAnswer;
        private Button btnVerifyAndReset;

        public ForgotPasswordForm()
        {
            InitializeComponentCustom();
        }

        private void InitializeComponentCustom()
        {
            this.Text = "Scout AI - Şifremi Unuttum (Gizli Soru ile Doğrulama)";
            this.Size = new Size(420, 320);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            lblEmail = new Label() { Text = "E-posta Adresiniz:", Left = 30, Top = 30, Width = 110 };
            txtEmail = new TextBox() { Left = 150, Top = 28, Width = 210 };

            btnGetQuestion = new Button() { Text = "Soruyu Getir", Left = 150, Top = 65, Width = 210, Height = 30, BackColor = Color.LightSteelBlue };
            btnGetQuestion.Click += BtnGetQuestion_Click;

            lblQuestion = new Label() { Text = "Gizli Soru:", Left = 30, Top = 115, Width = 110, Visible = false };
            lblQuestionText = new Label() { Text = "-", Left = 150, Top = 115, Width = 210, Font = new Font("Arial", 9, FontStyle.Bold), Visible = false };

            lblAnswer = new Label() { Text = "Cevabınız:", Left = 30, Top = 150, Width = 110, Visible = false };
            txtAnswer = new TextBox() { Left = 150, Top = 148, Width = 210, Visible = false };

            btnVerifyAndReset = new Button() { Text = "Doğrula ve Şifre Al", Left = 150, Top = 195, Width = 210, Height = 35, BackColor = Color.LightGreen, Visible = false, Font = new Font("Arial", 9, FontStyle.Bold) };
            btnVerifyAndReset.Click += BtnVerifyAndReset_Click;

            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);
            this.Controls.Add(btnGetQuestion);
            this.Controls.Add(lblQuestion);
            this.Controls.Add(lblQuestionText);
            this.Controls.Add(lblAnswer);
            this.Controls.Add(txtAnswer);
            this.Controls.Add(btnVerifyAndReset);
        }

        private void BtnGetQuestion_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Lütfen e-posta adresinizi giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var user = _userRepository.GetByEmail(email);

            if (user == null)
            {
                MessageBox.Show("Bu e-posta adresiyle kayıtlı bir kullanıcı bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lblQuestionText.Text = string.IsNullOrEmpty(user.SecurityQuestion) ? "İlkokul numaranız nedir?" : user.SecurityQuestion;

            lblQuestion.Visible = true;
            lblQuestionText.Visible = true;
            lblAnswer.Visible = true;
            txtAnswer.Visible = true;
            btnVerifyAndReset.Visible = true;
        }

        private void BtnVerifyAndReset_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string answerInput = txtAnswer.Text.Trim();

            var user = _userRepository.GetByEmail(email);

            if (user == null) return;

            if (string.Equals(user.SecurityAnswer?.Trim(), answerInput, StringComparison.OrdinalIgnoreCase))
            {
                string tempPassword = PasswordHasher.GenerateRandomPassword();
                var hashResult = PasswordHasher.HashPassword(tempPassword);

                _userRepository.UpdateTemporaryPassword(user.UserId, hashResult.Hash, hashResult.Salt, hashResult.Iterations);

                if (_emailService.SendTemporaryPassword(email, tempPassword, out string emailError))
                {
                    MessageBox.Show("Gizli soru cevabınız doğru! Yeni geçici şifreniz e-posta adresinize gönderildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Gizli soru cevabınız doğru!\nGeçici Yeni Şifreniz (Test): {tempPassword}", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.Close();
            }
            else
            {
                MessageBox.Show("Gizli soru cevabı yanlış! Güvenlik nedeniyle işleminiz reddedildi.", "Güvenlik İptali", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }
    }
}