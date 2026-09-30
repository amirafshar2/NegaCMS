using System.Globalization;

namespace Negacom.Infrastructure
{
    /// <summary>German formatting helpers for views.</summary>
    public static class Fmt
    {
        public static readonly CultureInfo De = new("de-DE");

        public static string Date(DateTime d) => d.ToString("d. MMMM yyyy", De);
        public static string DateTime(DateTime d) => d.ToString("dd.MM.yyyy, HH:mm", De) + " Uhr";
        public static string Short(DateTime d) => d.ToString("dd.MM.", De);
        public static string Num(int n) => n.ToString("#,0", De);
        public static string Price(decimal p, string unit) => p.ToString("#,0", De) + " " + (string.IsNullOrWhiteSpace(unit) ? "€" : unit);

        public static string Ago(DateTime d)
        {
            var s = System.DateTime.Now - d;
            if (s.TotalMinutes < 1) return "gerade eben";
            if (s.TotalMinutes < 60) return $"vor {(int)s.TotalMinutes} Min.";
            if (s.TotalHours < 24) return $"vor {(int)s.TotalHours} Std.";
            if (s.TotalDays < 2) return "gestern";
            if (s.TotalDays < 30) return $"vor {(int)s.TotalDays} Tagen";
            return Date(d);
        }
    }
}
