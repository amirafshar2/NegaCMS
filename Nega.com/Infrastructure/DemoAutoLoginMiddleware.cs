using BE;
using DAL.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Negacom.Infrastructure
{
    /// <summary>
    /// Demo mode: whoever opens /admin is signed in automatically as the demo admin,
    /// so visitors can explore the panel without registering.
    /// </summary>
    public class DemoAutoLoginMiddleware
    {
        private readonly RequestDelegate _next;
        public DemoAutoLoginMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext ctx, IOptions<DemoOptions> demo, SignInManager<User> signIn, UserManager<User> users)
        {
            if (demo.Value.Enabled
                && ctx.User.Identity?.IsAuthenticated != true
                && string.Equals(ctx.GetRouteValue("area") as string, "Admin", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(ctx.GetRouteValue("controller") as string, "Account", StringComparison.OrdinalIgnoreCase)
                && HttpMethods.IsGet(ctx.Request.Method))
            {
                var user = await users.FindByNameAsync(DbSeeder.DemoAdminUserName);
                if (user != null)
                {
                    await signIn.SignInAsync(user, isPersistent: false);
                    ctx.Response.Redirect(ctx.Request.Path + ctx.Request.QueryString);
                    return;
                }
            }
            await _next(ctx);
        }
    }
}
