using BE;
using BLL.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Negacom.Models;

namespace Negacom.Controllers
{
    public class BlogController : Controller
    {
        private readonly IBlogService _blogs;
        private readonly ICategoryService _categories;
        private readonly ICommentService _comments;
        private readonly UserManager<User> _users;

        public BlogController(IBlogService blogs, ICategoryService categories, ICommentService comments, UserManager<User> users)
        {
            _blogs = blogs; _categories = categories; _comments = comments; _users = users;
        }

        public IActionResult Index(int? category, string q, int page = 1)
        {
            var vm = new BlogListViewModel
            {
                Blogs = _blogs.GetPaged(true, category, q, page),
                Categories = _categories.GetWithBlogCount(true),
                Popular = _blogs.GetPopular(3),
                CategoryId = category,
                Search = q,
            };
            return View(vm);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var blog = _blogs.ReadForVisitor(id);
            if (blog == null) return NotFound();
            var vm = new BlogDetailViewModel
            {
                Blog = blog,
                Comments = _comments.GetForBlog(id),
                Related = _blogs.GetLatest(3, id, blog.CategoryId),
                Categories = _categories.GetWithBlogCount(true),
                Form = { BlogId = id },
            };
            if (vm.Related.Count < 2) vm.Related = _blogs.GetLatest(3, id);
            var user = await _users.GetUserAsync(User);
            if (user != null) { vm.Form.AuthorName = user.FullName; vm.Form.Email = user.Email; }
            return View(vm);
        }

        private const string Pending = "Danke! Ihr Beitrag wird nach der Prüfung durch unser Team veröffentlicht.";

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Comment(CommentForm form)
        {
            var user = await _users.GetUserAsync(User);
            var r = _comments.AddFromVisitor(new Comment
            {
                BlogId = form.BlogId, AuthorName = form.AuthorName?.Trim(), Email = form.Email?.Trim(),
                Content = form.Content?.Trim(), UserId = user?.Id,
            });
            TempData[r.Success ? "ok" : "err"] = r.Success ? Pending : string.Join(" ", r.Errors);
            return Redirect($"/blog/{form.BlogId}#comments");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(int blogId, int commentId, string authorName, string content)
        {
            var user = await _users.GetUserAsync(User);
            var r = _comments.AddVisitorReply(commentId, user?.FullName ?? authorName?.Trim(), user?.Id, content);
            TempData[r.Success ? "ok" : "err"] = r.Success ? Pending : string.Join(" ", r.Errors);
            return Redirect($"/blog/{blogId}#comment-{commentId}");
        }
    }
}
