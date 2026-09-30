using BE;
using BLL.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Negacom.Areas.Admin.Models;

namespace Negacom.Areas.Admin.Controllers
{
    /// <summary>User management (Admin only). Demo accounts are protected.</summary>
    [Authorize(Roles = RoleNames.Admin)]
    public class UsersController : AdminControllerBase
    {
        private readonly UserManager<User> _users;
        private readonly RoleManager<UserRole> _roles;
        private readonly IUserService _userService;

        public UsersController(UserManager<User> users, RoleManager<UserRole> roles, IUserService userService)
        {
            _users = users; _roles = roles; _userService = userService;
        }

        public async Task<IActionResult> Index(string role)
        {
            var list = _userService.GetAllWithBlogCount(out var blogCounts);
            var rows = new List<UserRow>();
            foreach (var u in list)
            {
                var r = await _users.GetRolesAsync(u);
                if (!string.IsNullOrEmpty(role) && !r.Contains(role)) continue;
                rows.Add(new UserRow { User = u, Roles = r, Blogs = blogCounts.TryGetValue(u.Id, out var n) ? n : 0 });
            }
            ViewBag.Role = role;
            ViewBag.AllRoles = await _roles.Roles.Select(r => r.Name).ToListAsync();
            return View(rows);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.AllRoles = await _roles.Roles.ToListAsync();
            return View("Form", new UserFormModel { Roles = { RoleNames.Writer } });
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserFormModel m)
        {
            ViewBag.AllRoles = await _roles.Roles.ToListAsync();
            if (string.IsNullOrWhiteSpace(m.Password)) ModelState.AddModelError("", "Bitte ein Passwort vergeben.");
            if (string.IsNullOrWhiteSpace(m.Name)) ModelState.AddModelError("", "Bitte einen Vornamen eingeben.");
            if (!ModelState.IsValid) return View("Form", m);

            var u = new User { RegisterDate = DateTime.Now, Picture = "/uploads/seed/user-4.jpg" };
            Map(m, u);
            if (!TryUpload(url => u.Picture = url)) return View("Form", m);
            var res = await _users.CreateAsync(u, m.Password);
            if (!res.Succeeded) { foreach (var e in res.Errors) ModelState.AddModelError("", e.Description); return View("Form", m); }
            await _users.AddToRolesAsync(u, m.Roles.Where(r => RoleExists(r)));
            Ok($"Benutzer „{u.UserName}“ wurde angelegt.");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var u = await _users.FindByIdAsync(id.ToString());
            if (u == null) return NotFound();
            ViewBag.AllRoles = await _roles.Roles.ToListAsync();
            return View("Form", new UserFormModel
            {
                Id = u.Id, UserName = u.UserName, Email = u.Email, Name = u.Name, Family = u.Family, JobTitle = u.JobTitle, About = u.About,
                PhoneNumber = u.PhoneNumber, Picture = u.Picture, Status = u.Status, IsDemo = u.IsDemo, LinkedIn = u.LinkedIn, Instagram = u.Instagram,
                Roles = (await _users.GetRolesAsync(u)).ToList(),
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UserFormModel m)
        {
            ViewBag.AllRoles = await _roles.Roles.ToListAsync();
            var u = await _users.FindByIdAsync(id.ToString());
            if (u == null) return NotFound();
            m.Id = id; m.IsDemo = u.IsDemo; m.Picture = u.Picture;

            if (u.IsDemo)
            {
                // demo accounts: login name, status and roles stay fixed so the demo keeps working
                m.UserName = u.UserName; m.Status = true;
                m.Roles = (await _users.GetRolesAsync(u)).ToList();
            }
            Map(m, u);
            if (!TryUpload(url => u.Picture = url)) return View("Form", m);
            var res = await _users.UpdateAsync(u);
            if (!res.Succeeded) { foreach (var e in res.Errors) ModelState.AddModelError("", e.Description); return View("Form", m); }

            if (!u.IsDemo)
            {
                var current = await _users.GetRolesAsync(u);
                await _users.RemoveFromRolesAsync(u, current.Except(m.Roles));
                await _users.AddToRolesAsync(u, m.Roles.Except(current).Where(r => RoleExists(r)));
                if (!string.IsNullOrWhiteSpace(m.Password))
                {
                    var token = await _users.GeneratePasswordResetTokenAsync(u);
                    var pw = await _users.ResetPasswordAsync(u, token, m.Password);
                    if (!pw.Succeeded) { Error(string.Join(" ", pw.Errors.Select(e => e.Description))); return RedirectToAction(nameof(Edit), new { id }); }
                }
            }
            Ok($"Benutzer „{u.UserName}“ wurde gespeichert.");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Toggle(int id)
        {
            var u = await _users.FindByIdAsync(id.ToString());
            if (u == null) return NotFound();
            if (u.IsDemo) { Error("Demo-Konten können nicht deaktiviert werden."); return RedirectToAction(nameof(Index)); }
            u.Status = !u.Status;
            await _users.UpdateAsync(u);
            await _users.UpdateSecurityStampAsync(u); // signs the user out if deactivated
            Ok(u.Status ? "Benutzer wurde aktiviert." : "Benutzer wurde deaktiviert.");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var u = await _users.FindByIdAsync(id.ToString());
            if (u == null) return NotFound();
            if (u.IsDemo) { Error("Demo-Konten können nicht gelöscht werden."); return RedirectToAction(nameof(Index)); }
            var hasBlogs = _userService.GetAllWithBlogCount(out var counts) != null && counts.ContainsKey(id);
            if (hasBlogs) { Error("Dieser Benutzer hat noch Blogartikel. Bitte zuerst die Artikel löschen oder den Benutzer deaktivieren."); return RedirectToAction(nameof(Index)); }
            await _users.DeleteAsync(u);
            Ok("Der Benutzer wurde gelöscht.");
            return RedirectToAction(nameof(Index));
        }

        private bool RoleExists(string r) => _roles.Roles.Any(x => x.Name == r);

        private static void Map(UserFormModel m, User u)
        {
            u.UserName = m.UserName?.Trim(); u.Email = m.Email?.Trim(); u.Name = m.Name?.Trim(); u.Family = m.Family?.Trim();
            u.JobTitle = m.JobTitle; u.About = m.About; u.PhoneNumber = m.PhoneNumber; u.Status = m.Status;
            u.LinkedIn = m.LinkedIn; u.Instagram = m.Instagram;
        }
    }

    [Authorize(Roles = RoleNames.Admin)]
    public class RolesController : AdminControllerBase
    {
        private readonly RoleManager<UserRole> _roles;
        private readonly UserManager<User> _users;
        public RolesController(RoleManager<UserRole> roles, UserManager<User> users) { _roles = roles; _users = users; }

        public async Task<IActionResult> Index(int? edit)
        {
            var rows = new List<RoleRow>();
            foreach (var r in await _roles.Roles.OrderBy(r => r.Id).ToListAsync())
                rows.Add(new RoleRow { Role = r, Users = (await _users.GetUsersInRoleAsync(r.Name)).Count });
            ViewBag.Edit = edit.HasValue ? rows.FirstOrDefault(r => r.Role.Id == edit)?.Role : null;
            return View(rows);
        }

        [HttpPost]
        public async Task<IActionResult> Save(int id, string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name)) { Error("Bitte einen Rollennamen eingeben."); return RedirectToAction(nameof(Index)); }
            if (id == 0)
            {
                var res = await _roles.CreateAsync(new UserRole(name.Trim()) { Description = description });
                if (res.Succeeded) Ok("Die Rolle wurde angelegt."); else Error(string.Join(" ", res.Errors.Select(e => e.Description)));
            }
            else
            {
                var r = await _roles.FindByIdAsync(id.ToString());
                if (r == null) return NotFound();
                if (!RoleNames.All.Contains(r.Name)) r.Name = name.Trim(); // system roles keep their name
                r.Description = description;
                await _roles.UpdateAsync(r);
                Ok("Die Rolle wurde gespeichert.");
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var r = await _roles.FindByIdAsync(id.ToString());
            if (r == null) return NotFound();
            if (RoleNames.All.Contains(r.Name)) { Error("System-Rollen können nicht gelöscht werden."); return RedirectToAction(nameof(Index)); }
            await _roles.DeleteAsync(r);
            Ok("Die Rolle wurde gelöscht.");
            return RedirectToAction(nameof(Index));
        }
    }
}
