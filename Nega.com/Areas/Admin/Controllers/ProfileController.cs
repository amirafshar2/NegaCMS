using BE;
using BLL.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Negacom.Areas.Admin.Models;
using Negacom.Infrastructure;

namespace Negacom.Areas.Admin.Controllers
{
    public class ProfileController : AdminControllerBase
    {
        private readonly UserManager<User> _users;
        private readonly SignInManager<User> _signIn;
        private readonly IBlogService _blogs;
        private readonly DemoOptions _demo;

        public ProfileController(UserManager<User> users, SignInManager<User> signIn, IBlogService blogs, IOptions<DemoOptions> demo)
        {
            _users = users; _signIn = signIn; _blogs = blogs; _demo = demo.Value;
        }

        public async Task<IActionResult> Index()
        {
            var me = await _users.GetUserAsync(User);
            ViewBag.Roles = await _users.GetRolesAsync(me);
            ViewBag.MyBlogs = _blogs.GetPaged(false, null, null, 1, 1000).Items.Where(b => b.UserId == me.Id).ToList();
            ViewBag.DemoMode = _demo.Enabled;
            return View(me);
        }

        [HttpPost]
        public async Task<IActionResult> Update(string name, string family, string jobTitle, string about, string phoneNumber, string linkedIn, string instagram)
        {
            var me = await _users.GetUserAsync(User);
            if (string.IsNullOrWhiteSpace(name)) { Error("Bitte einen Vornamen eingeben."); return RedirectToAction(nameof(Index)); }
            me.Name = name.Trim(); me.Family = family?.Trim(); me.JobTitle = jobTitle; me.About = about;
            me.PhoneNumber = phoneNumber; me.LinkedIn = linkedIn; me.Instagram = instagram;
            if (!TryUpload(url => me.Picture = url)) { Error(string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))); return RedirectToAction(nameof(Index)); }
            await _users.UpdateAsync(me);
            Ok("Ihr Profil wurde gespeichert.");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Password(PasswordModel m)
        {
            var me = await _users.GetUserAsync(User);
            if (_demo.Enabled && me.IsDemo) { Error("Im Demo-Modus kann das Passwort der Demo-Konten nicht geändert werden."); return RedirectToAction(nameof(Index)); }
            if (m.New != m.Confirm) { Error("Die Passwörter stimmen nicht überein."); return RedirectToAction(nameof(Index)); }
            var res = await _users.ChangePasswordAsync(me, m.Current ?? "", m.New ?? "");
            if (!res.Succeeded) { Error(string.Join(" ", res.Errors.Select(e => e.Description))); return RedirectToAction(nameof(Index)); }
            await _signIn.RefreshSignInAsync(me);
            Ok("Ihr Passwort wurde geändert.");
            return RedirectToAction(nameof(Index));
        }
    }
}
