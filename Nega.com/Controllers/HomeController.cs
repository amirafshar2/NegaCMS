using BE;
using BLL.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Negacom.Models;

namespace Negacom.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOurContactService _settings; private readonly IAboutService _about; private readonly IServiceService _services;
        private readonly ISuccessService _counters; private readonly IPortfolioService _portfolio; private readonly IPortfolioCategoryService _portfolioCats;
        private readonly IVideoService _videos; private readonly IPackageService _packages; private readonly ICustomerCommentService _testimonials;
        private readonly IBlogService _blogs; private readonly IContactService _contacts; private readonly INewsLetterService _news;
        private readonly IVisitorService _visitors;

        public HomeController(IOurContactService settings, IAboutService about, IServiceService services, ISuccessService counters,
            IPortfolioService portfolio, IPortfolioCategoryService portfolioCats, IVideoService videos, IPackageService packages,
            ICustomerCommentService testimonials, IBlogService blogs, IContactService contacts, INewsLetterService news, IVisitorService visitors)
        {
            _settings = settings; _about = about; _services = services; _counters = counters; _portfolio = portfolio;
            _portfolioCats = portfolioCats; _videos = videos; _packages = packages; _testimonials = testimonials; _blogs = blogs;
            _contacts = contacts; _news = news; _visitors = visitors;
        }

        public IActionResult Index()
        {
            _visitors.Track();
            return View(BuildHome());
        }

        private HomeViewModel BuildHome()
        {
            var portfolios = _portfolio.GetWithCategory(true);
            return new HomeViewModel
            {
                Settings = _settings.Get(),
                About = _about.Get(),
                Services = _services.GetList(true),
                Counters = _counters.GetList(true),
                Portfolios = portfolios,
                PortfolioCategories = _portfolioCats.GetList(true).Where(c => portfolios.Any(p => p.PortfolioCategoryId == c.Id)).ToList(),
                Video = _videos.GetList(true).FirstOrDefault(),
                Packages = _packages.GetList(true),
                Testimonials = _testimonials.GetList(true),
                LatestBlogs = _blogs.GetLatest(3),
            };
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Contact(ContactForm form)
        {
            var r = _contacts.Send(new Contact { UserName = form.UserName?.Trim(), Mail = form.Mail?.Trim(), Subject = form.Subject?.Trim(), Message = form.Message?.Trim() });
            var msg = r.Success ? "Vielen Dank! Ihre Nachricht wurde gesendet – wir melden uns in Kürze." : string.Join(" ", r.Errors);
            if (IsAjax) return Json(new { success = r.Success, message = msg });
            TempData[r.Success ? "ok" : "err"] = msg;
            return Redirect("/#contact");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Newsletter(string mail)
        {
            var r = _news.Subscribe(mail);
            var msg = r.Success ? "Danke für Ihre Anmeldung zum Newsletter!" : string.Join(" ", r.Errors);
            if (IsAjax) return Json(new { success = r.Success, message = msg });
            TempData[r.Success ? "ok" : "err"] = msg;
            return Redirect("/#newsletter");
        }

        // If the demo is part of a personal website, the legal pages of that website apply (appsettings "Legal").
        public IActionResult Privacy([FromServices] IConfiguration config) =>
            config["Legal:PrivacyUrl"] is { Length: > 0 } url ? Redirect(url) : View(_settings.Get());

        public IActionResult Imprint([FromServices] IConfiguration config) =>
            config["Legal:ImprintUrl"] is { Length: > 0 } url ? Redirect(url) : View(_settings.Get());

        public new IActionResult NotFound() => Status(404);

        public IActionResult Status(int code)
        {
            Response.StatusCode = code;
            return View("Error", new ErrorViewModel { StatusCode = code });
        }

        public IActionResult Error() => View(new ErrorViewModel { StatusCode = 500 });

        private bool IsAjax => Request.Headers.XRequestedWith == "XMLHttpRequest";
    }
}
