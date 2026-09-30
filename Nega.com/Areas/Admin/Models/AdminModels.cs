using BE;
using BLL.Common;

namespace Negacom.Areas.Admin.Models
{
    public class DashboardViewModel
    {
        public DashboardStats Stats { get; set; }
        public List<Comment> LatestComments { get; set; }
        public List<Contact> LatestContacts { get; set; }
        public List<Blog> TopBlogs { get; set; }
        public List<Notification> Notifications { get; set; }
    }

    public class BlogIndexViewModel
    {
        public PagedList<Blog> Blogs { get; set; }
        public List<Category> Categories { get; set; }
        public string Search { get; set; }
        public int? CategoryId { get; set; }
        public bool OnlyOwn { get; set; }
    }

    public class UserFormModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
        public string JobTitle { get; set; }
        public string About { get; set; }
        public string PhoneNumber { get; set; }
        public string Picture { get; set; }
        public string Password { get; set; }
        public bool Status { get; set; } = true;
        public bool IsDemo { get; set; }
        public List<string> Roles { get; set; } = new();
        public string LinkedIn { get; set; }
        public string Instagram { get; set; }
    }

    public class UserRow
    {
        public User User { get; set; }
        public IList<string> Roles { get; set; }
        public int Blogs { get; set; }
    }

    public class RoleRow
    {
        public UserRole Role { get; set; }
        public int Users { get; set; }
    }

    public class PasswordModel
    {
        public string Current { get; set; }
        public string New { get; set; }
        public string Confirm { get; set; }
    }

    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}
