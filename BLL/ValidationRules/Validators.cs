using BE;
using FluentValidation;

namespace BLL.ValidationRules
{
    public class BlogValidator : AbstractValidator<Blog>
    {
        public BlogValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Bitte einen Titel eingeben.").MaximumLength(200).WithMessage("Der Titel ist zu lang (max. 200 Zeichen).");
            RuleFor(x => x.Summary).NotEmpty().WithMessage("Bitte eine Kurzbeschreibung eingeben.").MaximumLength(400).WithMessage("Die Kurzbeschreibung ist zu lang (max. 400 Zeichen).");
            RuleFor(x => x.Content).NotEmpty().WithMessage("Bitte einen Inhalt eingeben.").MinimumLength(50).WithMessage("Der Inhalt ist zu kurz.");
            RuleFor(x => x.CategoryId).GreaterThan(0).WithMessage("Bitte eine Kategorie wählen.");
        }
    }

    public class CategoryValidator : AbstractValidator<Category>
    {
        public CategoryValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Bitte einen Namen eingeben.").MaximumLength(80).WithMessage("Der Name ist zu lang.");
        }
    }

    public class PortfolioCategoryValidator : AbstractValidator<PortfolioCategory>
    {
        public PortfolioCategoryValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Bitte einen Namen eingeben.").MaximumLength(80).WithMessage("Der Name ist zu lang.");
        }
    }

    public class CommentValidator : AbstractValidator<Comment>
    {
        public CommentValidator()
        {
            RuleFor(x => x.AuthorName).NotEmpty().WithMessage("Bitte einen Namen eingeben.").MaximumLength(60).WithMessage("Der Name ist zu lang.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Bitte eine E-Mail-Adresse eingeben.").EmailAddress().WithMessage("Die E-Mail-Adresse ist ungültig.");
            RuleFor(x => x.Content).NotEmpty().WithMessage("Bitte eine Nachricht eingeben.").MinimumLength(3).WithMessage("Die Nachricht ist zu kurz.")
                .MaximumLength(1500).WithMessage("Die Nachricht ist zu lang.");
        }
    }

    public class ReplyValidator : AbstractValidator<Reply>
    {
        public ReplyValidator()
        {
            RuleFor(x => x.Content).NotEmpty().WithMessage("Bitte eine Nachricht eingeben.").MaximumLength(1500).WithMessage("Die Nachricht ist zu lang.");
        }
    }

    public class ContactValidator : AbstractValidator<Contact>
    {
        public ContactValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Bitte einen Namen eingeben.").MaximumLength(80).WithMessage("Der Name ist zu lang.");
            RuleFor(x => x.Mail).NotEmpty().WithMessage("Bitte eine E-Mail-Adresse eingeben.").EmailAddress().WithMessage("Die E-Mail-Adresse ist ungültig.");
            RuleFor(x => x.Subject).NotEmpty().WithMessage("Bitte einen Betreff eingeben.").MaximumLength(150).WithMessage("Der Betreff ist zu lang.");
            RuleFor(x => x.Message).NotEmpty().WithMessage("Bitte eine Nachricht eingeben.").MinimumLength(10).WithMessage("Die Nachricht ist zu kurz.")
                .MaximumLength(3000).WithMessage("Die Nachricht ist zu lang.");
        }
    }

    public class NewsLetterValidator : AbstractValidator<NewsLetter>
    {
        public NewsLetterValidator()
        {
            RuleFor(x => x.Mail).NotEmpty().WithMessage("Bitte eine E-Mail-Adresse eingeben.").EmailAddress().WithMessage("Die E-Mail-Adresse ist ungültig.");
        }
    }

    public class PackageValidator : AbstractValidator<Package>
    {
        public PackageValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Bitte einen Namen eingeben.");
            RuleFor(x => x.Title).NotEmpty().WithMessage("Bitte einen Titel eingeben.");
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0).WithMessage("Der Preis ist ungültig.");
            RuleFor(x => x.Content).NotEmpty().WithMessage("Bitte einen Inhalt eingeben.").MinimumLength(20).WithMessage("Der Inhalt ist zu kurz.");
        }
    }

    public class PortfolioValidator : AbstractValidator<Portfolio>
    {
        public PortfolioValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Bitte einen Titel eingeben.");
            RuleFor(x => x.Brand).NotEmpty().WithMessage("Bitte einen Kunden / eine Marke eingeben.");
            RuleFor(x => x.PortfolioCategoryId).GreaterThan(0).WithMessage("Bitte eine Kategorie wählen.");
        }
    }

    public class ServiceValidator : AbstractValidator<Services>
    {
        public ServiceValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Bitte einen Titel eingeben.");
            RuleFor(x => x.Content).NotEmpty().WithMessage("Bitte einen Inhalt eingeben.");
        }
    }

    public class SuccessValidator : AbstractValidator<Success>
    {
        public SuccessValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Bitte einen Titel eingeben.");
            RuleFor(x => x.TotalSuccess).GreaterThanOrEqualTo(0).WithMessage("Die Zahl ist ungültig.");
        }
    }

    public class VideoValidator : AbstractValidator<Video>
    {
        public VideoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Bitte einen Titel eingeben.");
            RuleFor(x => x.VideoLink).NotEmpty().WithMessage("Bitte einen Link eingeben.");
        }
    }

    public class CustomerCommentValidator : AbstractValidator<CustomerComment>
    {
        public CustomerCommentValidator()
        {
            RuleFor(x => x.NameSurname).NotEmpty().WithMessage("Bitte einen Namen eingeben.");
            RuleFor(x => x.Content).NotEmpty().WithMessage("Bitte einen Inhalt eingeben.");
            RuleFor(x => x.Rating).InclusiveBetween(1, 5).WithMessage("Die Bewertung muss zwischen 1 und 5 liegen.");
        }
    }

    public class AboutValidator : AbstractValidator<About>
    {
        public AboutValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Bitte einen Titel eingeben.");
            RuleFor(x => x.Content).NotEmpty().WithMessage("Bitte einen Inhalt eingeben.");
        }
    }

    public class OurContactValidator : AbstractValidator<OurContact>
    {
        public OurContactValidator()
        {
            RuleFor(x => x.CompanyName).NotEmpty().WithMessage("Bitte einen Namen eingeben.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Bitte eine E-Mail-Adresse eingeben.").EmailAddress().WithMessage("Die E-Mail-Adresse ist ungültig.");
        }
    }
}
