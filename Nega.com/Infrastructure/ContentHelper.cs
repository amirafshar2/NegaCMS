using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Html;

namespace Negacom.Infrastructure
{
    public static class ContentHelper
    {
        /// <summary>
        /// Very small, safe text formatter: everything is HTML-encoded, then
        /// "## " lines become headings, "- " lines list items, blank lines separate paragraphs.
        /// </summary>
        public static IHtmlContent ToHtml(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return HtmlString.Empty;
            var sb = new StringBuilder();
            var blocks = Regex.Split(text.Replace("\r\n", "\n").Trim(), @"\n\s*\n");
            foreach (var block in blocks)
            {
                var lines = block.Split('\n');
                var para = new List<string>();
                var list = new List<string>();
                void Flush()
                {
                    if (para.Count > 0) { sb.Append("<p>").Append(string.Join("<br>", para)).Append("</p>"); para.Clear(); }
                    if (list.Count > 0) { sb.Append("<ul>"); foreach (var l in list) sb.Append("<li>").Append(l).Append("</li>"); sb.Append("</ul>"); list.Clear(); }
                }
                foreach (var raw in lines)
                {
                    var line = raw.Trim();
                    if (line.StartsWith("## ")) { Flush(); sb.Append("<h2>").Append(WebUtility.HtmlEncode(line[3..])).Append("</h2>"); }
                    else if (line.StartsWith("- ")) { if (para.Count > 0) Flush(); list.Add(WebUtility.HtmlEncode(line[2..])); }
                    else if (line.Length > 0) { if (list.Count > 0) Flush(); para.Add(WebUtility.HtmlEncode(line)); }
                }
                Flush();
            }
            return new HtmlString(sb.ToString());
        }

        public static string YouTubeId(string link)
        {
            if (string.IsNullOrWhiteSpace(link)) return null;
            var m = Regex.Match(link, @"(?:youtu\.be/|v=|embed/|shorts/)([A-Za-z0-9_-]{11})");
            if (m.Success) return m.Groups[1].Value;
            return Regex.IsMatch(link.Trim(), "^[A-Za-z0-9_-]{11}$") ? link.Trim() : null;
        }

        public static string Initials(string name)
        {
            var parts = (name ?? "?").Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return string.Concat(parts.Take(2).Select(p => p[0])).ToUpperInvariant();
        }
    }
}
