using Microsoft.AspNetCore.Identity;

namespace Negacom.Infrastructure
{
    /// <summary>German error messages for ASP.NET Core Identity.</summary>
    public class GermanIdentityErrorDescriber : IdentityErrorDescriber
    {
        private static IdentityError E(string code, string text) => new() { Code = code, Description = text };
        public override IdentityError DefaultError() => E(nameof(DefaultError), "Ein unbekannter Fehler ist aufgetreten.");
        public override IdentityError DuplicateUserName(string n) => E(nameof(DuplicateUserName), $"Der Benutzername „{n}“ ist bereits vergeben.");
        public override IdentityError DuplicateEmail(string e) => E(nameof(DuplicateEmail), $"Die E-Mail-Adresse „{e}“ wird bereits verwendet.");
        public override IdentityError InvalidUserName(string n) => E(nameof(InvalidUserName), $"Der Benutzername „{n}“ ist ungültig (nur Buchstaben, Ziffern, . - _ @).");
        public override IdentityError InvalidEmail(string e) => E(nameof(InvalidEmail), $"Die E-Mail-Adresse „{e}“ ist ungültig.");
        public override IdentityError DuplicateRoleName(string r) => E(nameof(DuplicateRoleName), $"Die Rolle „{r}“ existiert bereits.");
        public override IdentityError PasswordMismatch() => E(nameof(PasswordMismatch), "Das aktuelle Passwort ist falsch.");
        public override IdentityError PasswordTooShort(int l) => E(nameof(PasswordTooShort), $"Das Passwort muss mindestens {l} Zeichen lang sein.");
        public override IdentityError PasswordRequiresDigit() => E(nameof(PasswordRequiresDigit), "Das Passwort muss mindestens eine Ziffer enthalten.");
        public override IdentityError PasswordRequiresLower() => E(nameof(PasswordRequiresLower), "Das Passwort muss mindestens einen Kleinbuchstaben enthalten.");
        public override IdentityError PasswordRequiresUpper() => E(nameof(PasswordRequiresUpper), "Das Passwort muss mindestens einen Großbuchstaben enthalten.");
        public override IdentityError PasswordRequiresNonAlphanumeric() => E(nameof(PasswordRequiresNonAlphanumeric), "Das Passwort muss ein Sonderzeichen enthalten.");
        public override IdentityError UserAlreadyInRole(string r) => E(nameof(UserAlreadyInRole), $"Der Benutzer hat die Rolle „{r}“ bereits.");
        public override IdentityError InvalidToken() => E(nameof(InvalidToken), "Ungültiges Token.");
    }
}
