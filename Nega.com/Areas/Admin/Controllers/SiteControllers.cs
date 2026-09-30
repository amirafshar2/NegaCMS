using BE;
using BLL.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Negacom.Areas.Admin.Controllers
{
    [Authorize(Roles = RoleNames.Admin)]
    public class AboutController : AdminControllerBase
    {
        private readonly IAboutService _about;
        public AboutController(IAboutService about) => _about = about;

        public IActionResult Index() => View(_about.Get() ?? new About { Status = true });

        [HttpPost]
        public async Task<IActionResult> Index(IFormCollection form)
        {
            var a = _about.Get();
            var isNew = a == null;
            a ??= new About();
            await TryUpdateModelAsync(a, "", x => x.Title, x => x.Subtitle, x => x.Content, x => x.Content2, x => x.FoundedYear, x => x.Status);
            if (!TryUpload(url => a.Image = url)) return View(a);
            var r = isNew ? _about.Add(a) : _about.Update(a);
            if (!r.Success) { AddErrors(r); return View(a); }
            Ok("„Über uns“ wurde gespeichert.");
            return RedirectToAction(nameof(Index));
        }
    }

    [Authorize(Roles = RoleNames.Admin)]
    public class SettingsController : AdminControllerBase
    {
        private readonly IOurContactService _settings;
        public SettingsController(IOurContactService settings) => _settings = settings;

        public IActionResult Index() => View(_settings.Get());

        [HttpPost]
        public async Task<IActionResult> Index(IFormCollection form)
        {
            var s = _settings.GetAll().FirstOrDefault();
            var isNew = s == null;
            s ??= new OurContact();
            await TryUpdateModelAsync(s, "", x => x.CompanyName, x => x.Slogan, x => x.HeroTitle, x => x.HeroText, x => x.PhoneNumber, x => x.Email,
                x => x.Address, x => x.OpeningHours, x => x.Instagram, x => x.Telegram, x => x.LinkedIn, x => x.Github);
            var r = isNew ? _settings.Add(s) : _settings.Update(s);
            if (!r.Success) { AddErrors(r); return View(s); }
            Ok("Die Einstellungen wurden gespeichert.");
            return RedirectToAction(nameof(Index));
        }
    }

    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Moderator)]
    public class ContactsController : AdminControllerBase
    {
        private readonly IContactService _contacts;
        public ContactsController(IContactService contacts) => _contacts = contacts;

        public IActionResult Index(string filter = "all")
        {
            var all = _contacts.GetAll().OrderByDescending(c => c.Date).ToList();
            ViewBag.Filter = filter; ViewBag.All = all.Count; ViewBag.Unread = all.Count(c => !c.IsRead);
            return View(filter == "unread" ? all.Where(c => !c.IsRead).ToList() : all);
        }

        public IActionResult Detail(int id)
        {
            var c = _contacts.GetById(id);
            if (c == null) return NotFound();
            _contacts.MarkAsRead(id);
            return View(c);
        }

        [HttpPost]
        public IActionResult MarkUnread(int id) { _contacts.MarkAsRead(id, false); Ok("Als ungelesen markiert."); return RedirectToAction(nameof(Index)); }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var c = _contacts.GetById(id);
            if (c != null) { _contacts.Delete(c); Ok("Die Nachricht wurde gelöscht."); }
            return RedirectToAction(nameof(Index));
        }
    }

    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Moderator)]
    public class NewsletterController : AdminControllerBase
    {
        private readonly INewsLetterService _news;
        public NewsletterController(INewsLetterService news) => _news = news;

        public IActionResult Index() => View(_news.GetAll().OrderByDescending(n => n.Date).ToList());

        [HttpPost]
        public IActionResult Add(string mail)
        {
            var r = _news.Subscribe(mail);
            if (r.Success) Ok("Die Adresse wurde hinzugefügt."); else Error(r);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Toggle(int id)
        {
            var n = _news.GetById(id);
            if (n != null) { n.Status = !n.Status; _news.Update(n); }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var n = _news.GetById(id);
            if (n != null) { _news.Delete(n); Ok("Die Adresse wurde entfernt."); }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Export()
        {
            var sb = new StringBuilder("E-Mail;Angemeldet am;Aktiv\n");
            foreach (var n in _news.GetAll().OrderBy(n => n.Date)) sb.Append($"{n.Mail};{n.Date:dd.MM.yyyy};{(n.Status ? "ja" : "nein")}\n");
            return File(Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray(), "text/csv", "newsletter.csv");
        }
    }

    public class NotificationsController : AdminControllerBase
    {
        private readonly INotificationService _notifications;
        public NotificationsController(INotificationService notifications) => _notifications = notifications;

        public IActionResult Index() => View(_notifications.GetAll().OrderByDescending(n => n.Timestamp).ToList());

        [HttpPost]
        public IActionResult ReadAll() { _notifications.MarkAllAsRead(); Ok("Alle Benachrichtigungen als gelesen markiert."); return RedirectToAction(nameof(Index)); }

        /// <summary>Opens the target of a notification and marks it as read.</summary>
        public IActionResult Open(int id)
        {
            var n = _notifications.GetById(id);
            if (n == null) return RedirectToAction(nameof(Index));
            if (!n.ReadStatus) { n.ReadStatus = true; _notifications.Update(n); }
            return LocalRedirect(string.IsNullOrEmpty(n.Url) ? "/admin" : n.Url);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var n = _notifications.GetById(id);
            if (n != null) _notifications.Delete(n);
            return RedirectToAction(nameof(Index));
        }
    }
}
