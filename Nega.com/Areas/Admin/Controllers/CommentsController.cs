using BE;
using BLL.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Negacom.Areas.Admin.Controllers
{
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Moderator)]
    public class CommentsController : AdminControllerBase
    {
        private readonly ICommentService _comments;
        private readonly UserManager<User> _users;
        public CommentsController(ICommentService comments, UserManager<User> users) { _comments = comments; _users = users; }

        public IActionResult Index(string filter = "all")
        {
            bool? approved = filter switch { "pending" => false, "approved" => true, _ => null };
            ViewBag.Filter = filter;
            ViewBag.All = _comments.GetForAdmin(null).Count;
            ViewBag.Pending = _comments.PendingCount();
            return View(_comments.GetForAdmin(approved));
        }

        public IActionResult Detail(int id)
        {
            var c = _comments.GetDetail(id);
            return c == null ? NotFound() : View(c);
        }

        [HttpPost]
        public IActionResult Toggle(int id, string back)
        {
            _comments.ToggleApproval(id);
            Ok("Der Status wurde geändert.");
            return Back(back, id);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var c = _comments.GetById(id);
            if (c != null) { _comments.Delete(c); Ok("Der Kommentar wurde gelöscht."); }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Reply(int id, string content)
        {
            var me = await _users.GetUserAsync(User);
            var r = _comments.AddTeamReply(id, me, content);
            if (r.Success) Ok("Ihre Antwort wurde veröffentlicht."); else Error(r);
            return RedirectToAction(nameof(Detail), new { id });
        }

        [HttpPost]
        public IActionResult ToggleReply(int id, int commentId)
        {
            _comments.ToggleReply(id);
            return RedirectToAction(nameof(Detail), new { id = commentId });
        }

        [HttpPost]
        public IActionResult DeleteReply(int id, int commentId)
        {
            _comments.DeleteReply(id);
            Ok("Die Antwort wurde gelöscht.");
            return RedirectToAction(nameof(Detail), new { id = commentId });
        }

        private IActionResult Back(string back, int id) =>
            back == "detail" ? RedirectToAction(nameof(Detail), new { id }) : RedirectToAction(nameof(Index), new { filter = back });
    }
}
