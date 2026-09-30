using BLL.Abstract;
using Microsoft.AspNetCore.Mvc;
using Negacom.Areas.Admin.Models;

namespace Negacom.Areas.Admin.Controllers
{
    public class DashboardController : AdminControllerBase
    {
        private readonly IDashboardService _dashboard; private readonly ICommentService _comments;
        private readonly IContactService _contacts; private readonly IBlogService _blogs; private readonly INotificationService _notifications;

        public DashboardController(IDashboardService dashboard, ICommentService comments, IContactService contacts, IBlogService blogs, INotificationService notifications)
        {
            _dashboard = dashboard; _comments = comments; _contacts = contacts; _blogs = blogs; _notifications = notifications;
        }

        public IActionResult Index() => View(new DashboardViewModel
        {
            Stats = _dashboard.GetStats(),
            LatestComments = _comments.GetForAdmin(null).Take(5).ToList(),
            LatestContacts = _contacts.GetAll().OrderByDescending(c => c.Date).Take(5).ToList(),
            TopBlogs = _blogs.GetPopular(5),
            Notifications = _notifications.GetLatest(6),
        });
    }
}
