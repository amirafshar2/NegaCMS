using BLL.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Negacom.Controllers
{
    public class PackageController : Controller
    {
        private readonly IPackageService _packages;
        public PackageController(IPackageService packages) { _packages = packages; }

        public IActionResult Index() => View(_packages.GetList(true));

        public IActionResult Detail(int id)
        {
            var p = _packages.GetById(id);
            if (p == null || !p.Status) return NotFound();
            ViewBag.Others = _packages.GetList(true).Where(x => x.Id != id).ToList();
            return View(p);
        }
    }
}
