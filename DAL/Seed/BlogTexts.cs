namespace DAL.Seed
{
    /// <summary>Demo blog articles (German).</summary>
    internal static class BlogTexts
    {
        public static readonly (int cat, int minutes, string title, string summary, string content)[] German =
        {
            (0, 5, "Drei-Schichten-Architektur in ASP.NET Core verständlich erklärt",
             "Warum wir Entitäten, Datenzugriff und Geschäftslogik trennen – und wie das Projekte wartbarer macht.",
@"Wer ein Projekt schnell starten möchte, schreibt die Datenbankabfragen gerne direkt in den Controller. Das funktioniert – bis das Projekt wächst. Genau hier hilft eine saubere Drei-Schichten-Architektur.

## Die drei Schichten
Die BE-Schicht (Business Entities) enthält nur die Klassen, also z. B. Blog, Kategorie oder Benutzer. Die DAL-Schicht (Data Access Layer) kümmert sich um die Datenbank – bei NEGA mit Entity Framework Core und SQLite. Die BLL-Schicht (Business Logic Layer) enthält Regeln und Validierung. Die Oberfläche (UI) spricht ausschließlich mit der BLL.

## Warum der Aufwand?
Jede Schicht hat genau eine Aufgabe. Wechselt man die Datenbank von SQL Server zu SQLite, ändert sich nur die DAL. Die Controller bleiben unberührt. Außerdem lassen sich Manager-Klassen über Interfaces sehr einfach testen.

## Dependency Injection statt new
Repositories und Manager werden im Program.cs registriert und per Konstruktor injiziert. So gibt es pro Anfrage genau einen DbContext, und keine Klasse muss wissen, wie ihre Abhängigkeiten erzeugt werden.

## Fazit
Ein paar Dateien mehr am Anfang sparen später viele Stunden. Gerade für ein CMS mit Admin-Panel lohnt sich die Trennung von Anfang an."),

            (1, 4, "7 Designregeln für eine Website, der Kunden vertrauen",
             "Kontrast, Weißraum, klare Typografie: Mit diesen Regeln wirkt Ihre Website sofort professioneller.",
@"Besucher entscheiden in wenigen Sekunden, ob sie einer Website vertrauen. Gutes Design ist dabei kein Luxus, sondern Voraussetzung.

## 1. Genug Kontrast
Text muss auf jedem Hintergrund gut lesbar sein. Hellgrau auf Weiß sieht elegant aus, ist aber oft schwer zu lesen.

## 2. Weißraum bewusst einsetzen
Leerer Raum gibt Inhalten Luft und lenkt den Blick auf das Wesentliche.

## 3. Maximal zwei Schriftarten
Eine Schrift für Überschriften, eine für den Fließtext – mehr braucht es selten.

## 4. Eine klare Handlungsaufforderung
Jede Seite sollte ein Ziel haben: anrufen, anfragen, kaufen. Der wichtigste Button muss sofort auffallen.

## 5. Echte Fotos statt Stockbilder
Authentische Bilder vom Team oder von Projekten schaffen deutlich mehr Vertrauen.

## 6. Mobile zuerst
Über die Hälfte der Besuche kommt vom Smartphone. Das Layout muss dort zuerst funktionieren.

## 7. Impressum und Datenschutz sichtbar
In Deutschland Pflicht – und ein starkes Vertrauenssignal."),

            (2, 4, "Lokale SEO: So finden Kunden aus Ihrer Stadt Ihr Unternehmen",
             "Google-Unternehmensprofil, lokale Keywords und Bewertungen – die wichtigsten Schritte für mehr Sichtbarkeit vor Ort.",
@"Für viele Unternehmen zählen vor allem Kund:innen aus der eigenen Region. Lokale SEO sorgt dafür, dass Sie bei Suchen wie „Webdesign Stuttgart“ gefunden werden.

## Google-Unternehmensprofil pflegen
Name, Adresse und Telefonnummer müssen überall identisch sein. Ergänzen Sie Öffnungszeiten, Fotos und Leistungen.

## Lokale Keywords verwenden
Nennen Sie Ihre Stadt und Region natürlich in Überschriften, Texten und im Seitentitel.

## Bewertungen aktiv sammeln
Bitten Sie zufriedene Kund:innen um eine Bewertung und antworten Sie auf jede – auch auf kritische.

## Technik nicht vergessen
Schnelle Ladezeiten, HTTPS und eine mobile Darstellung sind die Grundlage für jedes gute Ranking."),

            (3, 3, "Website-Relaunch planen: Checkliste in 10 Schritten",
             "Von der Bestandsaufnahme bis zu den Weiterleitungen: So gelingt der Relaunch ohne Rankingverluste.",
@"Ein Relaunch ist eine große Chance – und ein Risiko, wenn wichtige Schritte vergessen werden.

## Vorbereitung
1. Ziele festlegen. 2. Bestehende Inhalte und Rankings analysieren. 3. Zielgruppe und Wettbewerber prüfen.

## Umsetzung
4. Seitenstruktur planen. 5. Design entwerfen und abstimmen. 6. Inhalte überarbeiten. 7. Technisch umsetzen und testen.

## Livegang
8. 301-Weiterleitungen für alte URLs einrichten. 9. Sitemap bei Google einreichen. 10. Nach dem Launch Fehlerseiten und Besucherzahlen beobachten.

## Unser Tipp
Planen Sie nach dem Livegang zwei Wochen für Feinschliff ein. Echte Nutzer finden immer noch Kleinigkeiten."),

            (1, 4, "Barrierefreie Websites: Was das BFSG für Unternehmen bedeutet",
             "Seit Juni 2025 gilt das Barrierefreiheitsstärkungsgesetz. Wir zeigen, worauf es bei Kontrast, Tastatur und Alternativtexten ankommt.",
@"Barrierefreiheit ist nicht nur ein Thema für Behörden. Mit dem Barrierefreiheitsstärkungsgesetz (BFSG) müssen viele digitale Angebote – etwa Online-Shops – seit Juni 2025 barrierefrei sein.

## Kontraste und Schriftgrößen
Texte brauchen ausreichend Kontrast zum Hintergrund und sollten sich ohne Layoutfehler vergrößern lassen.

## Bedienung per Tastatur
Alle Funktionen müssen auch ohne Maus erreichbar sein. Ein sichtbarer Fokus-Rahmen zeigt, wo man sich gerade befindet.

## Alternativtexte und Überschriften
Bilder brauchen aussagekräftige Alternativtexte, Seiten eine saubere Überschriften-Struktur. Das hilft Screenreadern – und ganz nebenbei auch der Suchmaschinenoptimierung.

## Unser Vorgehen
Wir prüfen neue Projekte mit automatischen Tests und manuell mit Tastatur und Screenreader. So entsteht Barrierefreiheit von Anfang an statt als teure Nacharbeit."),

            (0, 3, "Warum SQLite für kleine Projekte oft völlig ausreicht",
             "Kein Datenbankserver, keine Kosten, eine einzige Datei: SQLite ist ideal für Demos, Prototypen und kleine Websites.",
@"SQL Server oder PostgreSQL sind großartig – aber nicht jedes Projekt braucht einen eigenen Datenbankserver.

## Eine Datei, fertig
SQLite speichert die komplette Datenbank in einer einzigen Datei im Projekt. Kopieren, sichern, fertig.

## Perfekt für Demos
Diese Demo läuft genau so: Beim Start wird die Datenbank erstellt und mit Beispielinhalten gefüllt. Änderungen werden regelmäßig zurückgesetzt.

## Wann wechseln?
Bei vielen gleichzeitigen Schreibzugriffen oder mehreren Servern lohnt sich ein Datenbankserver. Dank Entity Framework Core ist der Wechsel später nur eine Zeile Code."),
        };
    }
}
