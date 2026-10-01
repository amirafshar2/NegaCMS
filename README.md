# NEGA CMS – Agentur-Website mit Admin-Panel

Ein vollständiges Content-Management-System für eine (fiktive) Digitalagentur, entwickelt mit **ASP.NET Core 8 MVC** in einer sauberen **3-Schichten-Architektur**.
Die öffentliche Website und das komplette Admin-Panel sind auf Deutsch. Die Datenbank ist eine **SQLite-Datei direkt im Projekt** – es wird kein Datenbankserver benötigt.

> **Live-Demo:** Der Admin-Bereich unter `/admin` öffnet sich im Demo-Modus **ohne Registrierung und ohne Anmeldung**.
> Alle Änderungen sind erlaubt und werden automatisch alle 60 Minuten zurückgesetzt.

![Startseite](docs/screenshots/01-startseite.png)

---

## Screenshots

### Öffentliche Website

| Leistungen | Referenzen mit Filter |
|---|---|
| ![Leistungen](docs/screenshots/02-leistungen.png) | ![Referenzen](docs/screenshots/03-referenzen.png) |
| **Preispakete** | **Kontaktformular** |
| ![Preise](docs/screenshots/04-preise.png) | ![Kontakt](docs/screenshots/05-kontakt.png) |
| **Blog mit Suche & Kategorien** | **Blogartikel mit Kommentaren** |
| ![Blog](docs/screenshots/06-blog.png) | ![Blogartikel](docs/screenshots/07-blog-artikel.png) |
| **Paket-Detailseite** | |
| ![Paket](docs/screenshots/08-paket-detail.png) | |

### Admin-Panel

![Admin-Dashboard](docs/screenshots/10-admin-dashboard.png)

| Blogartikel | Artikel bearbeiten |
|---|---|
| ![Artikel](docs/screenshots/11-admin-artikel.png) | ![Artikel bearbeiten](docs/screenshots/12-admin-artikel-bearbeiten.png) |
| **Kommentar-Moderation** | **Referenzen** |
| ![Kommentar](docs/screenshots/13-admin-kommentar.png) | ![Referenzen](docs/screenshots/14-admin-referenzen.png) |
| **Preispakete** | **Benutzer** |
| ![Preispakete](docs/screenshots/15-admin-preispakete.png) | ![Benutzer](docs/screenshots/16-admin-benutzer.png) |
| **Rollen & Rechte** | **Nachrichten** |
| ![Rollen](docs/screenshots/17-admin-rollen.png) | ![Nachrichten](docs/screenshots/18-admin-nachrichten.png) |
| **Einstellungen** | |
| ![Einstellungen](docs/screenshots/19-admin-einstellungen.png) | |

### Mobil (responsive)

| Startseite | Blogartikel | Admin-Dashboard | Admin-Artikelliste |
|---|---|---|---|
| ![Mobil Startseite](docs/screenshots/20-mobil-startseite.png) | ![Mobil Blog](docs/screenshots/21-mobil-blog.png) | ![Mobil Dashboard](docs/screenshots/22-mobil-admin-dashboard.png) | ![Mobil Artikel](docs/screenshots/23-mobil-admin-artikel.png) |

Auf kleinen Bildschirmen werden die Admin-Tabellen automatisch als Karten dargestellt.

---

## Architektur

```mermaid
flowchart TB
    UI["UI – Nega.com<br/>ASP.NET Core MVC · Controller · Views · Admin-Area"]
    BLL["BLL – Business Logic Layer<br/>Services/Manager · Geschäftsregeln · FluentValidation"]
    DAL["DAL – Data Access Layer<br/>EF Core · Repositories · SQLite · Seed-Daten"]
    BE["BE – Business Entities<br/>Blog, Kategorie, Paket, Benutzer, …"]
    UI --> BLL --> DAL --> BE
    UI -.-> BE
    BLL -.-> BE
```

| Schicht | Projekt | Aufgabe |
|---|---|---|
| **BE** | `BE/` | Reine Entitätsklassen ohne Abhängigkeiten zu anderen Schichten |
| **DAL** | `DAL/` | `DbContext` (SQLite), generisches Repository + spezifische Repositories, Seeder mit Demo-Inhalten |
| **BLL** | `BLL/` | Interfaces + Manager, Validierung (FluentValidation), Geschäftsregeln, DI-Registrierung |
| **UI** | `Nega.com/` | Controller, Razor-Views, Admin-Area, Datei-Upload, Demo-Modus |

**Prinzipien**
- Controller sprechen **nur mit der BLL** (Interfaces), nie direkt mit der Datenbank.
- Alle Abhängigkeiten per **Dependency Injection** (ein `DbContext` pro Request).
- Validierung und Regeln liegen in der BLL, z. B.
  - Besucher-Kommentare werden als „wartend“ gespeichert und müssen freigegeben werden
  - Kategorien mit Artikeln können nicht gelöscht werden
  - Newsletter-Adressen können nicht doppelt angemeldet werden
  - Lesezeit eines Artikels wird automatisch berechnet
  - Neue Kommentare, Nachrichten und Anmeldungen erzeugen Benachrichtigungen

## Funktionen

### Öffentliche Website
- Startseite: Hero, Leistungen, Über uns mit animierten Zählern, Referenzen mit Kategorie-Filter, Video, Preispakete, Kundenstimmen, neueste Artikel, Newsletter, Kontaktformular (AJAX)
- Blog mit Suche, Kategorien, Seitennavigation, beliebten/ähnlichen Artikeln
- Artikelseite mit Kommentaren und Antworten (Moderation im Admin-Panel)
- Paket-Detailseiten, Impressum, Datenschutz, eigene Fehlerseiten
- Responsive Design (Mobile first), Barrierefreiheit (Labels, Kontraste, Tastaturbedienung)
- **DSGVO-freundlich:** keine Tracking-Cookies, Schriften/Icons lokal, YouTube erst nach Klick (2-Klick-Lösung)

### Admin-Panel (`/admin`)
- **Dashboard** mit Besucherstatistik (SVG-Diagramm), Kennzahlen, neuesten Kommentaren/Nachrichten, Top-Artikeln
- **Blog:** Artikel (inkl. Bild-Upload, Entwurf/Veröffentlicht), Kategorien, Kommentare mit Freigabe und Team-Antworten
- **Website:** Leistungen, Über uns, Zähler, Referenzen + Kategorien, Preispakete, Videos, Kundenstimmen
- **Kommunikation:** Nachrichten-Posteingang (gelesen/ungelesen), Newsletter mit CSV-Export, Benachrichtigungen
- **System:** Benutzer, Rollen & Rechte (ASP.NET Core Identity), Website-Einstellungen, eigenes Profil

### Rollen
| Bereich | Admin | Moderator | Writer |
|---|:-:|:-:|:-:|
| Dashboard, Profil, eigene Artikel | ✔ | ✔ | ✔ |
| Alle Artikel, Kategorien, Kommentare | ✔ | ✔ | – |
| Nachrichten, Newsletter, Kundenstimmen | ✔ | ✔ | – |
| Website-Inhalte, Benutzer, Rollen, Einstellungen | ✔ | – | – |

Im Demo-Modus kann man oben im Admin-Panel über **„Ansicht als“** zwischen Admin, Moderator und Writer wechseln.

## Demo-Modus

In `appsettings.json`:

```json
"Demo": { "Enabled": true, "ResetMinutes": 60, "MaxUploadKb": 2048 }
```

- Aufruf von `/admin` meldet automatisch das Demo-Konto an – keine Registrierung nötig.
- Beim Start wird die Datenbank neu erstellt und mit Demo-Inhalten gefüllt; danach alle `ResetMinutes` Minuten zurückgesetzt (Tabellen werden geleert und neu befüllt, hochgeladene Bilder gelöscht).
- Demo-Konten können nicht gelöscht, deaktiviert oder in ihren Rollen verändert werden; Passwortänderungen sind gesperrt.
- Mit `"Enabled": false` verhält sich die Anwendung wie ein normales CMS mit Login (`/admin/account/login`).

| Benutzername | Rolle | Passwort (nur ohne Demo-Modus relevant) |
|---|---|---|
| `demo.admin` | Admin | `Demo#2026` |
| `lukas.moderator` | Moderator | `Demo#2026` |
| `sara.writer` | Writer | `Demo#2026` |

## Lokal starten

Voraussetzung: [.NET 8 SDK](https://dotnet.microsoft.com/download) (oder Visual Studio 2022 ab 17.8).

```bash
dotnet run --project Nega.com
```

Danach `https://localhost:7070` öffnen. In Visual Studio einfach `Nega-com.sln` öffnen und **F5** drücken.

Die Datenbank liegt unter `Nega.com/App_Data/nega.db` und wird beim Start automatisch erstellt/befüllt.

## Deployment (Docker / Render)

```bash
docker build -t nega-cms .
docker run -p 8080:8080 nega-cms
```

Für [Render](https://render.com) liegt eine `render.yaml` bei (Free-Plan, Region Frankfurt, Health-Check `/health`).

## Technologien

ASP.NET Core 8 MVC · Entity Framework Core 8 · SQLite · ASP.NET Core Identity · FluentValidation · Razor Views · HTML5/CSS3 (Custom Design, CSS-Grid, ohne Framework) · Vanilla JavaScript · Font Awesome (lokal) · Docker

## Projektstruktur

```
NegaCMS/
├── BE/                 Entitäten
├── DAL/
│   ├── Abstract/       Repository-Interfaces
│   ├── Context/        DB (IdentityDbContext, SQLite)
│   ├── EntityFrameWork/ EF-Repositories
│   ├── Repository/     GenericRepository<T>
│   └── Seed/           Demo-Daten
├── BLL/
│   ├── Abstract/       Service-Interfaces
│   ├── Concrete/       Manager (Geschäftslogik)
│   ├── ValidationRules/ FluentValidation
│   └── DependencyInjection.cs
└── Nega.com/           UI (MVC)
    ├── Areas/Admin/    Admin-Panel
    ├── Controllers/    Öffentliche Seiten
    ├── Infrastructure/ Demo-Modus, Upload, Hilfsklassen
    ├── Views/
    └── wwwroot/
```

---

**Entwickler:** Amir Reza Afshar – Umschulung Fachinformatiker für Anwendungsentwicklung
GitHub: [github.com/amirafshar2](https://github.com/amirafshar2)

*Alle Firmen, Personen, Referenzen und Kundenstimmen in den Demo-Daten sind frei erfunden.*
