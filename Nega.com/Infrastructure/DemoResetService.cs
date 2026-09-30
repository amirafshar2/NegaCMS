using DAL.Context;
using DAL.Seed;
using Microsoft.Extensions.Options;

namespace Negacom.Infrastructure
{
    /// <summary>Rebuilds the demo database periodically so every visitor sees clean demo data.</summary>
    public class DemoResetService : BackgroundService
    {
        private readonly IServiceProvider _sp;
        private readonly DemoOptions _opt;
        private readonly ILogger<DemoResetService> _log;
        private readonly IWebHostEnvironment _env;

        public DemoResetService(IServiceProvider sp, IOptions<DemoOptions> opt, ILogger<DemoResetService> log, IWebHostEnvironment env)
        {
            _sp = sp; _opt = opt.Value; _log = log; _env = env;
        }

        protected override async Task ExecuteAsync(CancellationToken stop)
        {
            if (!_opt.Enabled || _opt.ResetMinutes <= 0) return;
            var interval = TimeSpan.FromMinutes(_opt.ResetMinutes);
            while (!stop.IsCancellationRequested)
            {
                _opt.NextReset = DateTime.Now.Add(interval);
                try { await Task.Delay(interval, stop); } catch (TaskCanceledException) { return; }
                try
                {
                    ResetNow(_sp, _env);
                    _log.LogInformation("Demo database has been reset.");
                }
                catch (Exception ex) { _log.LogError(ex, "Demo reset failed"); }
            }
        }

        public static void ResetNow(IServiceProvider sp, IWebHostEnvironment env)
        {
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DB>();
            DbSeeder.Reset(db, recreateSchema: false);
            // remove files uploaded by visitors
            var dir = Path.Combine(env.WebRootPath, "uploads", "content");
            if (Directory.Exists(dir))
                foreach (var f in Directory.GetFiles(dir)) { try { File.Delete(f); } catch { /* ignore */ } }
        }
    }
}
