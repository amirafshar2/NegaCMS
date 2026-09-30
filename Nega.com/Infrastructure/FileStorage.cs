using Microsoft.Extensions.Options;

namespace Negacom.Infrastructure
{
    public interface IFileStorage
    {
        /// <summary>Saves an uploaded image and returns its URL, or null + error message.</summary>
        (string Url, string Error) SaveImage(IFormFile file);
    }

    public class FileStorage : IFileStorage
    {
        private static readonly string[] Allowed = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        private readonly IWebHostEnvironment _env;
        private readonly DemoOptions _opt;

        public FileStorage(IWebHostEnvironment env, IOptions<DemoOptions> opt) { _env = env; _opt = opt.Value; }

        public (string Url, string Error) SaveImage(IFormFile file)
        {
            if (file == null || file.Length == 0) return (null, null);
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!Allowed.Contains(ext) || !file.ContentType.StartsWith("image/")) return (null, "Nur Bilder (JPG, PNG, WEBP, GIF) sind erlaubt.");
            if (file.Length > _opt.MaxUploadKb * 1024L) return (null, $"Das Bild ist zu groß (max. {_opt.MaxUploadKb / 1024} MB).");

            var dir = Path.Combine(_env.WebRootPath, "uploads", "content");
            Directory.CreateDirectory(dir);
            var name = $"{Guid.NewGuid():N}{ext}";
            using (var fs = File.Create(Path.Combine(dir, name))) file.CopyTo(fs);
            return ($"/uploads/content/{name}", null);
        }
    }
}
