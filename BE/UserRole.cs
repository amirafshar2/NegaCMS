using Microsoft.AspNetCore.Identity;

namespace BE
{
    public class UserRole : IdentityRole<int>
    {
        public UserRole() { }
        public UserRole(string name) : base(name) { }

        public string Description { get; set; }
        public bool Status { get; set; } = true;
    }

    public static class RoleNames
    {
        public const string Admin = "Admin";
        public const string Moderator = "Moderator";
        public const string Writer = "Writer";
        public const string Member = "User";
        public static readonly string[] All = { Admin, Moderator, Writer, Member };
        public const string Panel = Admin + "," + Moderator + "," + Writer;
    }
}
