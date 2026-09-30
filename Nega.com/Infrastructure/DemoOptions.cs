namespace Negacom.Infrastructure
{
    public class DemoOptions
    {
        /// <summary>When true, the admin panel opens without login (auto sign-in as demo admin).</summary>
        public bool Enabled { get; set; } = true;
        /// <summary>The database is rebuilt from the seed data every N minutes (0 = never).</summary>
        public int ResetMinutes { get; set; } = 60;
        public int MaxUploadKb { get; set; } = 2048;
        public DateTime NextReset { get; set; } = DateTime.Now;
    }
}
