using BE;
using DAL.Seed;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Negacom.Areas.Admin.Models;
using Negacom.Infrastructure;

namespace Negacom.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AllowAnonymous]
    [AutoValidateAntiforgeryToken]
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signIn;
        private readonly UserManager<User> _users;
        private readonly DemoOptions _demo;

        public AccountController(SignInManager<User> signIn, UserManager<User> users, IOptions<DemoOptions> demo)
        {
            _signIn = signIn; _users = users; _demo = demo.Value;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.Demo = _demo.Enabled;
            return View(new LoginModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel m)
        {
            ViewBag.Demo = _demo.Enabled;
            var user = string.IsNullOrWhiteSpace(m.UserName) ? null : await _users.FindByNameAsync(m.UserName.Trim());
            if (user == null || !user.Status)
            {
                ModelState.AddModelError("", "Benutzername oder Passwort ist falsch.");
                return View(m);
            }
            var res = await _signIn.PasswordSignInAsync(user, m.Password ?? "", false, lockoutOnFailure: true);
            if (!res.Succeeded)
            {
                ModelState.AddModelError("", res.IsLockedOut ? "Das Konto ist vorübergehend gesperrt." : "Benutzername oder Passwort ist falsch.");
                return View(m);
            }
            var roles = await _users.GetRolesAsync(user);
            if (!roles.Any(r => r is RoleNames.Admin or RoleNames.Moderator or RoleNames.Writer)) return Redirect("/");
            return LocalRedirect(Url.IsLocalUrl(m.ReturnUrl) ? m.ReturnUrl : "/admin");
        }

        /// <summary>Demo only: switch between the demo accounts to see the panel with different roles.</summary>
        [HttpPost]
        public async Task<IActionResult> SwitchRole(string role, string returnUrl)
        {
            if (!_demo.Enabled) return NotFound();
            var userName = role switch
            {
                RoleNames.Moderator => "lukas.moderator",
                RoleNames.Writer => "sara.writer",
                _ => DbSeeder.DemoAdminUserName,
            };
            var user = await _users.FindByNameAsync(userName);
            if (user == null) return Redirect("/admin");
            await _signIn.SignOutAsync();
            await _signIn.SignInAsync(user, false);
            TempData["ok"] = $"Sie sind jetzt als {user.FullName} ({role ?? RoleNames.Admin}) angemeldet.";
            return Redirect(role is RoleNames.Admin or null && Url.IsLocalUrl(returnUrl) ? returnUrl : "/admin");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signIn.SignOutAsync();
            return Redirect("/");
        }

        public IActionResult Denied() => View();
    }
}
