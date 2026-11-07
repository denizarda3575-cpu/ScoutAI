using System.Text.RegularExpressions;

namespace ScoutAI.Helpers
{
    public static class PasswordPolicyValidator
    {
        public static bool IsValid(string password, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(password) || password.Length < 8)
            {
                errorMessage = "Parola en az 8 karakter uzunluğunda olmalıdır.";
                return false;
            }

            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                errorMessage = "Parola en az bir büyük harf içermelidir.";
                return false;
            }

            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                errorMessage = "Parola en az bir küçük harf içermelidir.";
                return false;
            }

            if (!Regex.IsMatch(password, @"[0-9]"))
            {
                errorMessage = "Parola en az bir rakam içermelidir.";
                return false;
            }

            return true;
        }
    }
}