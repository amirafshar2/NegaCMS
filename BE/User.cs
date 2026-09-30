using Microsoft.AspNetCore.Identity;

namespace BE
{
    public class User : IdentityUser<int>
    {
        public string Name { get; set; }
        public string Family { get; set; }
        public string Picture { get; set; }
        public string JobTitle { get; set; }
        public string About { get; set; }
        public string Address { get; set; }
        public DateTime RegisterDate { get; set; } = DateTime.Now;
        public bool Status { get; set; } = true;
        /// <summary>Protected demo account: cannot be deleted or deactivated.</summary>
        public bool IsDemo { get; set; }
        public string Instagram { get; set; }
        public string Telegram { get; set; }
        public string LinkedIn { get; set; }

        public string FullName => $"{Name} {Family}".Trim();

        public List<Blog> Blogs { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
        public List<Reply> Replies { get; set; } = new();
    }
}
