namespace ScoutAI.Helpers
{
    public static class CurrentSession
    {
        public static int UserId { get; private set; }
        public static string Email { get; private set; }
        public static string FullName { get; private set; }
        public static string RoleName { get; private set; }

        public static void Start(int userId, string email, string fullName, string roleName)
        {
            UserId = userId;
            Email = email;
            FullName = fullName;
            RoleName = roleName;
        }

        public static void Clear()
        {
            UserId = 0;
            Email = null;
            FullName = null;
            RoleName = null;
        }
    }
}