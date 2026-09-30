using BE;
using BLL.Abstract;
using BLL.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Negacom.Infrastructure;

namespace Negacom.Areas.Admin.Controllers
{
    /// <summary>Common base for all admin controllers (area, authorization, messages, uploads).</summary>
    [Area("Admin")]
    [Authorize(Roles = RoleNames.Panel)]
    [AutoValidateAntiforgeryToken]
    public abstract class AdminControllerBase : Controller
    {
        protected void Ok(string message) => TempData["ok"] = message;
        protected void Error(string message) => TempData["err"] = message;
        protected void Error(Result r) => TempData["err"] = string.Join("\n", r.Errors);

        protected void AddErrors(Result r)
        {
            foreach (var e in r.Errors) ModelState.AddModelError(string.Empty, e);
        }

        protected int CurrentUserId => int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;

        /// <summary>Stores an uploaded image (form field "ImageFile"). Returns false if the file was invalid.</summary>
        protected bool TryUpload(Action<string> setUrl, string field = "ImageFile")
        {
            var file = Request.Form.Files[field];
            if (file == null || file.Length == 0) return true;
            var storage = HttpContext.RequestServices.GetRequiredService<IFileStorage>();
            var (url, err) = storage.SaveImage(file);
            if (err != null) { ModelState.AddModelError(string.Empty, err); return false; }
            setUrl(url);
            return true;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewData["Section"] = ControllerContext.ActionDescriptor.ControllerName.ToLowerInvariant();
            base.OnActionExecuting(context);
        }
    }

    /// <summary>
    /// Generic CRUD controller for simple content types (services, counters, videos, …).
    /// All data access goes through the BLL service.
    /// </summary>
    public abstract class CrudControllerBase<T> : AdminControllerBase where T : class, new()
    {
        protected readonly IContentService<T> Service;
        protected CrudControllerBase(IContentService<T> service) => Service = service;

        protected abstract string ItemName { get; }
        protected virtual Action<T, string> SetImage => null;
        protected virtual void Prepare(T item) { }
        protected static int IdOf(T item) => (int)typeof(T).GetProperty("Id")!.GetValue(item)!;

        public virtual IActionResult Index() => View(Service.GetList(false));

        [HttpGet]
        public virtual IActionResult Create() { Prepare(new T()); return View("Form", new T()); }

        [HttpPost]
        public virtual IActionResult Create(T item)
        {
            if (SetImage != null && !TryUpload(url => SetImage(item, url))) { Prepare(item); return View("Form", item); }
            var r = Service.Add(item);
            if (!r.Success) { AddErrors(r); Prepare(item); return View("Form", item); }
            Ok($"{ItemName} wurde angelegt.");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public virtual IActionResult Edit(int id)
        {
            var item = Service.GetById(id);
            if (item == null) return NotFound();
            Prepare(item);
            return View("Form", item);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit(int id, IFormCollection form)
        {
            var item = Service.GetById(id);
            if (item == null) return NotFound();
            await TryUpdateModelAsync(item);
            typeof(T).GetProperty("Id")!.SetValue(item, id);
            if (SetImage != null && !TryUpload(url => SetImage(item, url))) { Prepare(item); return View("Form", item); }
            var r = Service.Update(item);
            if (!r.Success) { AddErrors(r); Prepare(item); return View("Form", item); }
            Ok($"{ItemName} wurde gespeichert.");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            var item = Service.GetById(id);
            if (item != null) { Service.Delete(item); Ok($"{ItemName} wurde gelöscht."); }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public virtual IActionResult Toggle(int id)
        {
            var item = Service.GetById(id);
            var prop = typeof(T).GetProperty("Status");
            if (item != null && prop != null)
            {
                prop.SetValue(item, !(bool)prop.GetValue(item)!);
                Service.Update(item);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
