using BE;
using BLL.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Negacom.Areas.Admin.Models;

namespace Negacom.Areas.Admin.Controllers
{
    /// <summary>Blog articles. Writers may only see and edit their own articles.</summary>
    public class BlogsController : AdminControllerBase
    {
        private readonly IBlogService _blogs;
        private readonly ICategoryService _categories;

        public BlogsController(IBlogService blogs, ICategoryService categories) { _blogs = blogs; _categories = categories; }

        private bool IsWriterOnly => User.IsInRole(RoleNames.Writer) && !User.IsInRole(RoleNames.Admin) && !User.IsInRole(RoleNames.Moderator);
        private bool CanEdit(Blog b) => !IsWriterOnly || b.UserId == CurrentUserId;

        public IActionResult Index(string q, int? category, int page = 1)
        {
            var paged = _blogs.GetPaged(false, category, q, page, 10);
            if (IsWriterOnly)
            {
                var own = _blogs.GetPaged(false, category, q, 1, 1000).Items.Where(b => b.UserId == CurrentUserId).ToList();
                paged = new BLL.Common.PagedList<Blog> { Items = own.Skip((page - 1) * 10).Take(10).ToList(), Total = own.Count, Page = page, PageSize = 10 };
            }
            return View(new BlogIndexViewModel { Blogs = paged, Categories = _categories.GetList(false), Search = q, CategoryId = category, OnlyOwn = IsWriterOnly });
        }

        [HttpGet]
        public IActionResult Create()
        {
            Prepare(null);
            return View("Form", new Blog { Date = DateTime.Now, Status = true });
        }

        [HttpPost]
        public IActionResult Create(Blog blog)
        {
            blog.UserId = CurrentUserId;
            blog.Date = blog.Date == default ? DateTime.Now : blog.Date;
            if (!TryUpload(url => blog.Picture = url)) { Prepare(blog.CategoryId); return View("Form", blog); }
            blog.Picture ??= "/uploads/seed/blog-1.jpg";
            var r = _blogs.Add(blog);
            if (!r.Success) { AddErrors(r); Prepare(blog.CategoryId); return View("Form", blog); }
            Ok("Der Artikel wurde veröffentlicht.");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var b = _blogs.GetById(id);
            if (b == null) return NotFound();
            if (!CanEdit(b)) return Forbid();
            Prepare(b.CategoryId);
            return View("Form", b);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, IFormCollection form)
        {
            var b = _blogs.GetById(id);
            if (b == null) return NotFound();
            if (!CanEdit(b)) return Forbid();
            var owner = b.UserId; var views = b.ViewCount;
            await TryUpdateModelAsync(b, "", x => x.Title, x => x.Summary, x => x.Content, x => x.CategoryId, x => x.Status, x => x.Date);
            b.Id = id; b.UserId = owner; b.ViewCount = views;
            if (!TryUpload(url => b.Picture = url)) { Prepare(b.CategoryId); return View("Form", b); }
            var r = _blogs.Update(b);
            if (!r.Success) { AddErrors(r); Prepare(b.CategoryId); return View("Form", b); }
            Ok("Der Artikel wurde gespeichert.");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Toggle(int id)
        {
            var b = _blogs.GetById(id);
            if (b != null && CanEdit(b)) { b.Status = !b.Status; _blogs.Update(b); Ok(b.Status ? "Der Artikel ist jetzt sichtbar." : "Der Artikel ist jetzt ausgeblendet."); }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var b = _blogs.GetById(id);
            if (b != null && CanEdit(b)) { _blogs.Delete(b); Ok("Der Artikel wurde gelöscht."); }
            return RedirectToAction(nameof(Index));
        }

        private void Prepare(int? selected) =>
            ViewBag.Categories = _categories.GetList(false).Select(c => new SelectListItem(c.Name, c.Id.ToString(), c.Id == selected)).ToList();
    }
}
