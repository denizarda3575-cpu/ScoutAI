using System;

namespace ScoutAI.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public int Iterations { get; set; }
        public bool MustChangePassword { get; set; }
        public bool IsActive { get; set; } = true; // Varsayılan olarak aktif
        public int RoleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}