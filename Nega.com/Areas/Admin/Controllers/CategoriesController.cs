using BE;
using BLL.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Negacom.Areas.Admin.Controllers
{
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Moderator)]
    public class CategoriesController : AdminControllerBase
    {
        private readonly ICategoryService _categories;
        public CategoriesController(ICategoryService categories) => _categories = categories;

        public IActionResult Index(int? edit)
        {
            ViewBag.Edit = edit.HasValue ? _categories.GetById(edit.Value) : null;
            return View(_categories.GetWithBlogCount(false));
        }

        [HttpPost]
        public IActionResult Save(int id, string name, string description, bool status = true)
        {
            var c = id > 0 ? _categories.GetById(id) : new Category();
            if (c == null) return NotFound();
            c.Name = name?.Trim(); c.Description = description?.Trim(); c.Status = status;
            var r = id > 0 ? _categories.Update(c) : _categories.Add(c);
            if (r.Success) Ok(id > 0 ? "Die Kategorie wurde gespeichert." : "Die Kategorie wurde angelegt."); else Error(r);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Toggle(int id)
        {
            var c = _categories.GetById(id);
            if (c != null) { c.Status = !c.Status; _categories.Update(c); Ok(c.Status ? "Die Kategorie ist jetzt aktiv." : "Die Kategorie ist jetzt inaktiv."); }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var r = _categories.SafeDelete(id);
            if (r.Success) Ok("Die Kategorie wurde gelöscht."); else Error(r);
            return RedirectToAction(nameof(Index));
        }
    }
}
