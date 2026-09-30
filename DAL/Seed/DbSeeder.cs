using BE;
using DAL.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DAL.Seed
{
    /// <summary>
    /// Creates the SQLite database and fills it with German demo content.
    /// </summary>
    public static class DbSeeder
    {
        public const string DemoAdminUserName = "demo.admin";
        public const string DemoPassword = "Demo#2026";
        private const string Img = "/uploads/seed/";

        /// <summary>
        /// Restores the demo data.
        /// recreateSchema = true  -> delete the database file and build it again (used at startup)
        /// recreateSchema = false -> keep the file, just empty all tables (safe while the app is running)
        /// </summary>
        public static void Reset(DB db, bool recreateSchema)
        {
            if (recreateSchema)
            {
                db.Database.EnsureDeleted();
            }
            else
            {
                db.Database.EnsureCreated();
                var conn = db.Database.GetDbConnection();
                conn.Open();
                using var tx = conn.BeginTransaction();
                var tables = new List<string>();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%'";
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) tables.Add(r.GetString(0));
                }
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = "PRAGMA defer_foreign_keys = ON;" + string.Concat(tables.Select(t => $"DELETE FROM \"{t}\";"))
                                      + "DELETE FROM sqlite_sequence;";
                    cmd.ExecuteNonQuery();
                }
                tx.Commit();
                db.ChangeTracker.Clear();
            }
            Seed(db);
        }

        public static void Seed(DB db)
        {
            db.Database.EnsureCreated();
            if (db.Users.Any()) return;

            var now = DateTime.Now;

            // ---------- Roles ----------
            var roles = new[]
            {
                new UserRole(RoleNames.Admin) { Description = "Voller Zugriff auf alle Bereiche" },
                new UserRole(RoleNames.Moderator) { Description = "Kommentare, Nachrichten und Inhalte verwalten" },
                new UserRole(RoleNames.Writer) { Description = "Eigene Blogartikel schreiben" },
                new UserRole(RoleNames.Member) { Description = "Registrierter Besucher" },
            };
            foreach (var r in roles) { r.NormalizedName = r.Name.ToUpperInvariant(); r.ConcurrencyStamp = Guid.NewGuid().ToString(); }
            db.Roles.AddRange(roles);
            db.SaveChanges();

            // ---------- Users ----------
            var hasher = new PasswordHasher<User>();
            User U(string userName, string name, string family, string job, string pic, string about, bool demo = false)
            {
                var u = new User
                {
                    UserName = userName, NormalizedUserName = userName.ToUpperInvariant(),
                    Email = userName + "@nega.example", NormalizedEmail = (userName + "@nega.example").ToUpperInvariant(),
                    EmailConfirmed = true, Name = name, Family = family, JobTitle = job, Picture = Img + pic,
                    About = about, IsDemo = demo, Status = true, RegisterDate = now.AddDays(-Random.Shared.Next(40, 400)),
                    SecurityStamp = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString(),
                    LinkedIn = "https://www.linkedin.com/", Instagram = "https://www.instagram.com/",
                };
                u.PasswordHash = hasher.HashPassword(u, DemoPassword);
                return u;
            }
            var admin = U(DemoAdminUserName, "Amir", "Afshar", "Gründer & Full-Stack-Entwickler", "user-1.jpg",
                "Entwickelt Webanwendungen mit ASP.NET Core und liebt saubere Architektur.", true);
            var writer = U("sara.writer", "Sara", "Mohammadi", "Content & UX Writerin", "user-2.jpg",
                "Schreibt über Design, Nutzerführung und Content-Strategie.", true);
            var moderator = U("lukas.moderator", "Lukas", "Weber", "Projektmanager", "user-3.jpg",
                "Koordiniert Projekte und betreut unsere Kund:innen.", true);
            var member = U("max.user", "Max", "Mustermann", "Besucher", "user-4.jpg", "Registrierter Leser des Blogs.", true);
            db.Users.AddRange(admin, writer, moderator, member);
            db.SaveChanges();

            void Role(User u, string role) =>
                db.UserRoles.Add(new IdentityUserRole<int> { UserId = u.Id, RoleId = roles.First(r => r.Name == role).Id });
            Role(admin, RoleNames.Admin); Role(writer, RoleNames.Writer); Role(moderator, RoleNames.Moderator); Role(member, RoleNames.Member);
            db.SaveChanges();

            // ---------- Site settings ----------
            db.OurContacts.Add(
                new OurContact
                {
                    CompanyName = "NEGA Digitalagentur", Slogan = "Websites, die verkaufen.",
                    HeroTitle = "Wir bauen digitale Erlebnisse, die wirken.",
                    HeroText = "Webdesign, individuelle Webentwicklung mit ASP.NET Core und Online-Marketing – alles aus einer Hand, von der Idee bis zum Launch.",
                    PhoneNumber = "+49 711 000 000", Email = "hallo@nega.example", Address = "Musterstraße 12, 70173 Stuttgart",
                    OpeningHours = "Mo–Fr 9:00–18:00 Uhr", Instagram = "https://www.instagram.com/", Telegram = "https://t.me/",
                    LinkedIn = "https://www.linkedin.com/", Github = "https://github.com/amirafshar2",
                });

            // ---------- About ----------
            db.Abouts.Add(
                new About
                {
                    Status = true, FoundedYear = 2019, Image = Img + "about.jpg",
                    Title = "Über NEGA", Subtitle = "Ein kleines Team mit großem Anspruch",
                    Content = "NEGA ist eine Digitalagentur, die Unternehmen dabei hilft, online sichtbar zu werden und Kund:innen zu gewinnen. Wir verbinden gutes Design mit solider Technik: schnelle, sichere und leicht pflegbare Websites auf Basis von ASP.NET Core.",
                    Content2 = "Jedes Projekt beginnt mit Zuhören. Erst wenn wir Ihre Ziele verstehen, entwerfen wir Struktur, Design und Funktionen – transparent, termintreu und mit persönlicher Betreuung auch nach dem Launch.",
                });

            // ---------- Services ----------
            var services = new (string icon, string de, string deText)[]
            {
                ("fas fa-pencil-ruler", "Webdesign", "Moderne, responsive Layouts, die Ihre Marke stärken und auf jedem Gerät gut aussehen."),
                ("fas fa-code", "Webentwicklung", "Individuelle Webanwendungen mit ASP.NET Core, sauberer Architektur und Admin-Panel."),
                ("fas fa-shopping-cart", "Online-Shops", "Shops mit Warenkorb, Zahlungsanbindung und einfacher Produktverwaltung."),
                ("fas fa-chart-line", "SEO & Marketing", "Technische SEO, Content-Strategie und Kampagnen, die messbar Ergebnisse bringen."),
                ("fas fa-palette", "Branding & Logo", "Logo, Farben und Typografie – ein konsistenter Auftritt für Ihr Unternehmen."),
                ("fas fa-server", "Hosting & Wartung", "Updates, Backups und Monitoring, damit Ihre Website sicher und schnell bleibt."),
            };
            for (int i = 0; i < services.Length; i++)
            {
                var s = services[i];
                db.Services.Add(new Services { Icon = s.icon, Title = s.de, Content = s.deText, Order = i });
            }

            // ---------- Counters ----------
            var counters = new (string icon, int n, string suf, string de)[]
            {
                ("fas fa-layer-group", 120, "+", "Abgeschlossene Projekte"),
                ("far fa-smile", 85, "", "Zufriedene Kund:innen"),
                ("fas fa-award", 6, "", "Jahre Erfahrung"),
                ("fas fa-coffee", 3400, "", "Tassen Kaffee"),
            };
            for (int i = 0; i < counters.Length; i++)
            {
                var c = counters[i];
                db.Successes.Add(new Success { Icon = c.icon, TotalSuccess = c.n, Suffix = c.suf, Title = c.de, Order = i });
            }

            // ---------- Packages ----------
            db.Packages.AddRange(
                new Package { Order = 0, Name = "Starter", Price = 490, PriceUnit = "€", DeliveryDays = 7, Picture = Img + "package-1.jpg",
                    Title = "Der perfekte Einstieg", Content = "Eine professionelle One-Page-Website für Selbstständige und kleine Unternehmen, die schnell online gehen möchten.",
                    Features = "One-Page-Website\nResponsive Design\nKontaktformular\nSSL-Zertifikat\n1 Korrekturschleife" },
                new Package { Order = 1, Name = "Business", Price = 1290, PriceUnit = "€", DeliveryDays = 21, IsPopular = true, Picture = Img + "package-2.jpg",
                    Title = "Für wachsende Unternehmen", Content = "Mehrseitige Website mit eigenem Admin-Panel, Blog und Grund-SEO – Inhalte pflegen Sie ganz einfach selbst.",
                    Features = "Bis zu 8 Seiten\nAdmin-Panel (CMS)\nBlog mit Kommentaren\nGrund-SEO & Google Search Console\nNewsletter-Anmeldung\n3 Korrekturschleifen" },
                new Package { Order = 2, Name = "Premium", Price = 2490, PriceUnit = "€", DeliveryDays = 40, Picture = Img + "package-3.jpg",
                    Title = "Individuelle Lösung", Content = "Maßgeschneiderte Webanwendung oder Online-Shop mit Schnittstellen, Rollen- und Rechteverwaltung.",
                    Features = "Unbegrenzte Seiten\nOnline-Shop oder Web-App\nBenutzer- & Rollenverwaltung\nAPI-Anbindungen\nPerformance-Optimierung\n3 Monate Wartung inklusive" });

            // ---------- Portfolio ----------
            var pcDe = new[] { "Webdesign", "Online-Shop", "Branding" };
            var catsDe = pcDe.Select(n => new PortfolioCategory { Name = n, Description = n }).ToList();
            db.PortfolioCategories.AddRange(catsDe);
            db.SaveChanges();
            var works = new (int cat, string brand, string de, string deText)[]
            {
                (0, "Café Morgenrot", "Website für ein Café in Stuttgart", "Warmes Design, Speisekarte und Tischreservierung online."),
                (1, "Urban Sneakers", "Sneaker-Onlineshop", "Shop mit Größenfilter, Warenkorb und Zahlungsanbindung."),
                (2, "Physio Balance", "Branding für eine Physiotherapie-Praxis", "Logo, Farbwelt und Geschäftsausstattung."),
                (0, "Kanzlei Berger", "Website für eine Anwaltskanzlei", "Seriöser Auftritt mit Terminbuchung und Blog."),
                (1, "Bio Hofladen", "Regionaler Hofladen online", "Wöchentliche Gemüsekisten mit Abo-Funktion."),
                (2, "Techno Start", "Corporate Identity für ein Start-up", "Markenstrategie, Logo und Pitch-Deck."),
            };
            for (int i = 0; i < works.Length; i++)
            {
                var w = works[i];
                var date = now.AddDays(-30 * (i + 1));
                db.Portfolios.Add(new Portfolio { PortfolioCategoryId = catsDe[w.cat].Id, Brand = w.brand, Title = w.de, Description = w.deText, Picture = Img + $"work-{i + 1}.jpg", Link = "#", Date = date });
            }

            // ---------- Video ----------
            db.Videos.Add(
                new Video { Title = "So arbeiten wir", Content = "In drei Minuten: vom ersten Gespräch über Design und Entwicklung bis zum Launch Ihrer Website.", VideoLink = "https://www.youtube.com/watch?v=aqz-KE-bpKQ", Picture = Img + "video.jpg", View = 1240 });

            // ---------- Testimonials ----------
            var t = new (string name, string brand, string de)[]
            {
                ("Julia Hartmann", "Café Morgenrot", "Seit dem Relaunch kommen deutlich mehr Reservierungen über die Website. Das Team war jederzeit erreichbar."),
                ("Reza Karimi", "Urban Sneakers", "Der Shop ist schnell, übersichtlich und die Verwaltung der Produkte ist kinderleicht."),
                ("Thomas Berger", "Kanzlei Berger", "Professionell, termintreu und mit viel Gespür für unsere Zielgruppe. Klare Empfehlung!"),
                ("Maryam Ahmadi", "Physio Balance", "Unser neues Logo und die Website passen perfekt zusammen. Wir sind begeistert."),
            };
            for (int i = 0; i < t.Length; i++)
            {
                db.CustomerComments.Add(new CustomerComment { NameSurname = t[i].name, Brand = t[i].brand, Content = t[i].de, Picture = Img + $"client-{i + 1}.jpg", Email = "kunde@nega.example", Date = now.AddDays(-15 * i) });
            }

            // ---------- Blog ----------
            var bcDe = new[] { ("Webentwicklung", "Technik, Code und Architektur"), ("Design", "UI, UX und Gestaltung"), ("Marketing", "SEO, Content und Social Media"), ("Tipps", "Praktische Ratschläge für Unternehmen") };
            var blogCatsDe = bcDe.Select(c => new Category { Name = c.Item1, Description = c.Item2 }).ToList();
            db.Categories.AddRange(blogCatsDe);
            db.SaveChanges();

            var authors = new[] { admin, writer, admin, writer, moderator, admin };
            var blogsDe = new List<Blog>();
            for (int i = 0; i < BlogTexts.German.Length; i++)
            {
                var d = BlogTexts.German[i];
                var date = now.AddDays(-(i * 9 + 2)).AddHours(-i);
                blogsDe.Add(new Blog { CategoryId = blogCatsDe[d.cat].Id, UserId = authors[i].Id, Title = d.title, Summary = d.summary, Content = d.content, Picture = Img + $"blog-{i + 1}.jpg", Date = date, ReadingMinutes = d.minutes, ViewCount = 60 + (6 - i) * 37 });
            }
            db.Blogs.AddRange(blogsDe);
            db.SaveChanges();

            // ---------- Comments & replies ----------
            void C(Blog b, string author, string mail, string text, bool approved, int daysAgo, string reply = null, User replier = null, User user = null)
            {
                var c = new Comment { BlogId = b.Id, AuthorName = author, Email = mail, Content = text, Status = approved, Date = now.AddDays(-daysAgo), UserId = user?.Id };
                if (reply != null)
                    c.Replies.Add(new Reply { AuthorName = replier.FullName, UserId = replier.Id, Content = reply, IsTeamReply = true, CreatedAt = now.AddDays(-daysAgo).AddHours(3) });
                db.Comments.Add(c);
            }
            C(blogsDe[0], "Jonas", "jonas@mail.example", "Super erklärt! Genau so habe ich mir die Trennung der Schichten vorgestellt.", true, 1, "Danke Jonas! Freut uns, dass der Artikel hilft.", admin);
            C(blogsDe[0], "Lea", "lea@mail.example", "Nutzt ihr auch Unit-Tests für die BLL-Schicht?", true, 2, "Ja – gerade die BLL lässt sich dank Interfaces sehr gut testen.", admin);
            C(blogsDe[1], "Max Mustermann", "max.user@nega.example", "Die Tipps zu Kontrasten sind Gold wert.", true, 3, null, null, member);
            C(blogsDe[2], "Petra", "petra@mail.example", "Welche Tools empfehlt ihr für die Keyword-Recherche?", false, 0);
            C(blogsDe[3], "Anna", "anna@mail.example", "Bitte mehr solcher Artikel!", false, 0);

            // ---------- Contacts ----------
            db.Contacts.AddRange(
                new Contact { UserName = "Martin Schulz", Mail = "martin@mail.example", Subject = "Angebot für Online-Shop", Message = "Hallo, wir möchten unseren Weinhandel online bringen. Können Sie uns ein Angebot für das Business-Paket machen?", Date = now.AddHours(-5) },
                new Contact { UserName = "Sabine Koch", Mail = "sabine@mail.example", Subject = "Wartungsvertrag", Message = "Bieten Sie auch Wartung für bestehende WordPress-Seiten an?", Date = now.AddDays(-3), IsRead = true },
                new Contact { UserName = "Kevin Braun", Mail = "kevin@mail.example", Subject = "Rückruf", Message = "Bitte rufen Sie mich wegen eines Relaunchs zurück.", Date = now.AddDays(-6), IsRead = true });

            // ---------- Newsletter ----------
            foreach (var (m, d) in new[] { ("info@firma.example", 2), ("kontakt@studio.example", 5), ("hanna@mail.example", 7), ("office@praxis.example", 12), ("jan@mail.example", 20) })
                db.NewsLetters.Add(new NewsLetter { Mail = m, Date = now.AddDays(-d) });

            // ---------- Notifications ----------
            db.Notifications.AddRange(
                new Notification { Type = "contact", Title = "Neue Nachricht", Message = "Martin Schulz: Angebot für Online-Shop", Url = "/admin/contacts", Timestamp = now.AddHours(-5) },
                new Notification { Type = "comment", Title = "Neuer Kommentar", Message = "Petra hat einen Kommentar geschrieben (wartet auf Freigabe)", Url = "/admin/comments", Timestamp = now.AddHours(-9) },
                new Notification { Type = "comment", Title = "Neuer Kommentar", Message = "Anna hat einen Kommentar geschrieben (wartet auf Freigabe)", Url = "/admin/comments", Timestamp = now.AddHours(-12) },
                new Notification { Type = "newsletter", Title = "Newsletter", Message = "info@firma.example hat den Newsletter abonniert", Url = "/admin/newsletter", Timestamp = now.AddDays(-2) },
                new Notification { Type = "user", Title = "Neuer Benutzer", Message = "Max Mustermann hat sich registriert", Url = "/admin/users", Timestamp = now.AddDays(-4), ReadStatus = true });

            // ---------- Visitor statistics (last 30 days) ----------
            var rnd = new Random(42);
            for (int i = 30; i >= 0; i--)
            {
                var day = now.Date.AddDays(-i);
                var weekend = day.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
                var count = rnd.Next(weekend ? 40 : 80, weekend ? 90 : 170) + (30 - i) * 2;
                if (i == 0) count = count * Math.Max(1, now.Hour) / 24; // today: only the hours so far
                db.VisitorCounts.Add(new VisitorCount { VisitDate = day, Count = count });
            }

            db.SaveChanges();
        }
    }
}
