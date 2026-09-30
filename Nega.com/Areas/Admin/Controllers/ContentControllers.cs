using BE;
using BLL.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Negacom.Areas.Admin.Controllers
{
    [Authorize(Roles = RoleNames.Admin)]
    public class ServicesController : CrudControllerBase<Services>
    {
        public ServicesController(IServiceService s) : base(s) { }
        protected override string ItemName => "Die Leistung";
    }

    [Authorize(Roles = RoleNames.Admin)]
    public class CountersController : CrudControllerBase<Success>
    {
        public CountersController(ISuccessService s) : base(s) { }
        protected override string ItemName => "Der Zähler";
    }

    [Authorize(Roles = RoleNames.Admin)]
    public class VideosController : CrudControllerBase<Video>
    {
        public VideosController(IVideoService s) : base(s) { }
        protected override string ItemName => "Das Video";
        protected override Action<Video, string> SetImage => (v, url) => v.Picture = url;
    }

    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Moderator)]
    public class TestimonialsController : CrudControllerBase<CustomerComment>
    {
        public TestimonialsController(ICustomerCommentService s) : base(s) { }
        protected override string ItemName => "Die Kundenstimme";
        protected override Action<CustomerComment, string> SetImage => (c, url) => c.Picture = url;
        public override Microsoft.AspNetCore.Mvc.IActionResult Index() => View(Service.GetList(false).OrderByDescending(x => x.Date).ToList());
    }

    [Authorize(Roles = RoleNames.Admin)]
    public class PackagesController : CrudControllerBase<Package>
    {
        public PackagesController(IPackageService s) : base(s) { }
        protected override string ItemName => "Das Paket";
        protected override Action<Package, string> SetImage => (p, url) => p.Picture = url;
    }

    [Authorize(Roles = RoleNames.Admin)]
    public class PortfolioCategoriesController : CrudControllerBase<PortfolioCategory>
    {
        private readonly IPortfolioService _portfolio;
        public PortfolioCategoriesController(IPortfolioCategoryService s, IPortfolioService portfolio) : base(s) { _portfolio = portfolio; }
        protected override string ItemName => "Die Kategorie";

        public override Microsoft.AspNetCore.Mvc.IActionResult Index()
        {
            ViewBag.Counts = _portfolio.GetAll().GroupBy(p => p.PortfolioCategoryId).ToDictionary(g => g.Key, g => g.Count());
            return base.Index();
        }

        public override Microsoft.AspNetCore.Mvc.IActionResult Delete(int id)
        {
            if (_portfolio.GetAll().Any(p => p.PortfolioCategoryId == id))
            {
                Error("Diese Kategorie enthält noch Projekte und kann nicht gelöscht werden.");
                return RedirectToAction(nameof(Index));
            }
            return base.Delete(id);
        }
    }

    [Authorize(Roles = RoleNames.Admin)]
    public class PortfolioController : CrudControllerBase<Portfolio>
    {
        private readonly IPortfolioService _portfolio;
        private readonly IPortfolioCategoryService _categories;
        public PortfolioController(IPortfolioService s, IPortfolioCategoryService categories) : base(s) { _portfolio = s; _categories = categories; }
        protected override string ItemName => "Das Projekt";
        protected override Action<Portfolio, string> SetImage => (p, url) => p.Picture = url;

        public override Microsoft.AspNetCore.Mvc.IActionResult Index() => View(_portfolio.GetWithCategory(false));

        protected override void Prepare(Portfolio item) =>
            ViewBag.Categories = _categories.GetList(false).Select(c => new SelectListItem(c.Name, c.Id.ToString(), c.Id == item.PortfolioCategoryId)).ToList();
    }
}
