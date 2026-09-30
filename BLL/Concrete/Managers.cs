using BE;
using BLL.Abstract;
using BLL.Common;
using DAL.Abstract;
using FluentValidation;

namespace BLL.Concrete
{
    public class AboutManager : ContentManager<About>, IAboutService
    {
        public AboutManager(IAboutDal dal, IValidator<About> v) : base(dal, v) { }
        public About Get() => Dal.GetAll().FirstOrDefault();
    }

    public class OurContactManager : ContentManager<OurContact>, IOurContactService
    {
        public OurContactManager(IOurContactDal dal, IValidator<OurContact> v) : base(dal, v) { }
        public OurContact Get() => Dal.GetAll().FirstOrDefault() ?? new OurContact { CompanyName = "NEGA" };
    }

    public class ServiceManager : ContentManager<Services>, IServiceService { public ServiceManager(IServiceDal d, IValidator<Services> v) : base(d, v) { } }
    public class SuccessManager : ContentManager<Success>, ISuccessService { public SuccessManager(ISuccessDal d, IValidator<Success> v) : base(d, v) { } }
    public class PackageManager : ContentManager<Package>, IPackageService { public PackageManager(IPackageDal d, IValidator<Package> v) : base(d, v) { } }
    public class VideoManager : ContentManager<Video>, IVideoService { public VideoManager(IVideoDal d, IValidator<Video> v) : base(d, v) { } }
    public class CustomerCommentManager : ContentManager<CustomerComment>, ICustomerCommentService { public CustomerCommentManager(ICustomerCommentDal d, IValidator<CustomerComment> v) : base(d, v) { } }

    public class PortfolioCategoryManager : ContentManager<PortfolioCategory>, IPortfolioCategoryService
    {
        public PortfolioCategoryManager(IPortfolioCategoryDal d, IValidator<PortfolioCategory> v) : base(d, v) { }
    }

    public class CategoryManager : ContentManager<Category>, ICategoryService
    {
        private readonly ICategoryDal _dal;
        private readonly IBlogDal _blogs;
        public CategoryManager(ICategoryDal dal, IBlogDal blogs, IValidator<Category> v) : base(dal, v) { _dal = dal; _blogs = blogs; }

        public List<(Category Category, int BlogCount)> GetWithBlogCount(bool onlyActive) => _dal.GetWithBlogCount(onlyActive);

        /// <summary>Business rule: a category that still has articles cannot be deleted.</summary>
        public Result SafeDelete(int id)
        {
            if (_blogs.Count(b => b.CategoryId == id) > 0) return Result.Fail("Diese Kategorie enthält noch Artikel und kann nicht gelöscht werden.");
            Dal.Delete(Dal.GetById(id));
            return Result.Ok();
        }
    }

    public class PortfolioManager : ContentManager<Portfolio>, IPortfolioService
    {
        private readonly IPortfolioDal _dal;
        public PortfolioManager(IPortfolioDal dal, IValidator<Portfolio> v) : base(dal, v) { _dal = dal; }
        public List<Portfolio> GetWithCategory(bool onlyActive) => _dal.GetWithCategory(onlyActive);
    }

    public class BlogManager : ContentManager<Blog>, IBlogService
    {
        private readonly IBlogDal _dal;
        public BlogManager(IBlogDal dal, IValidator<Blog> v) : base(dal, v) { _dal = dal; }

        public override Result Add(Blog item)
        {
            item.ReadingMinutes = EstimateReadingMinutes(item.Content);
            return base.Add(item);
        }

        public override Result Update(Blog item)
        {
            item.ReadingMinutes = EstimateReadingMinutes(item.Content);
            return base.Update(item);
        }

        public PagedList<Blog> GetPaged(bool onlyActive, int? categoryId, string search, int page, int pageSize = 6)
        {
            page = Math.Max(1, page);
            var (items, total) = _dal.GetPaged(onlyActive, categoryId, search, page, pageSize);
            return new PagedList<Blog> { Items = items, Total = total, Page = page, PageSize = pageSize };
        }

        public Blog GetDetail(int id) => _dal.GetWithRelations(id);

        /// <summary>Returns a published article and counts the view.</summary>
        public Blog ReadForVisitor(int id)
        {
            var b = _dal.GetWithRelations(id);
            if (b == null || !b.Status) return null;
            _dal.IncreaseView(id);
            b.ViewCount++;
            return b;
        }

        public List<Blog> GetLatest(int take, int? exceptId = null, int? categoryId = null) => _dal.GetLatest(take, exceptId, categoryId);
        public List<Blog> GetPopular(int take) => _dal.GetPopular(take);

        private static int EstimateReadingMinutes(string text)
        {
            var words = (text ?? "").Split((char[])null, StringSplitOptions.RemoveEmptyEntries).Length;
            return Math.Max(1, (int)Math.Round(words / 200.0));
        }
    }

    public class CommentManager : GenericManager<Comment>, ICommentService
    {
        private readonly ICommentDal _dal;
        private readonly IReplyDal _replies;
        private readonly IBlogDal _blogs;
        private readonly INotificationService _notify;
        private readonly IValidator<Reply> _replyValidator;

        public CommentManager(ICommentDal dal, IReplyDal replies, IBlogDal blogs, INotificationService notify,
            IValidator<Comment> v, IValidator<Reply> rv) : base(dal, v)
        {
            _dal = dal; _replies = replies; _blogs = blogs; _notify = notify; _replyValidator = rv;
        }

        public List<Comment> GetForBlog(int blogId) => _dal.GetForBlog(blogId, true);
        public List<Comment> GetForAdmin(bool? approved) => _dal.GetAllWithRelations(approved);
        public Comment GetDetail(int id) => _dal.GetWithRelations(id);
        public int PendingCount() => _dal.Count(c => !c.Status);

        /// <summary>Visitor comments are stored as "pending" and must be approved by a moderator.</summary>
        public Result AddFromVisitor(Comment comment)
        {
            var blog = _blogs.GetById(comment.BlogId);
            if (blog == null || !blog.Status) return Result.Fail("Der Artikel wurde nicht gefunden.");
            comment.Status = false;
            comment.Date = DateTime.Now;
            var r = Add(comment);
            if (r.Success)
                _notify.Notify("comment", "Neuer Kommentar", $"{comment.AuthorName}: „{Short(comment.Content)}“ – {blog.Title}", "/admin/comments");
            return r;
        }

        public void ToggleApproval(int id)
        {
            var c = Dal.GetById(id);
            if (c == null) return;
            c.Status = !c.Status;
            Dal.Update(c);
        }

        public Result AddTeamReply(int commentId, User author, string content)
        {
            var reply = new Reply { CommentId = commentId, UserId = author.Id, AuthorName = author.FullName, Content = content?.Trim(), IsTeamReply = true };
            var v = _replyValidator.Validate(reply);
            if (!v.IsValid) return Result.Fail(v.Errors.Select(e => e.ErrorMessage).ToArray());
            var c = Dal.GetById(commentId);
            if (c == null) return Result.Fail("Der Kommentar wurde nicht gefunden.");
            if (!c.Status) { c.Status = true; Dal.Update(c); } // answering a comment also approves it
            _replies.Add(reply);
            return Result.Ok();
        }

        public Result AddVisitorReply(int commentId, string authorName, int? userId, string content)
        {
            var reply = new Reply { CommentId = commentId, UserId = userId, AuthorName = authorName, Content = content?.Trim(), Status = false };
            if (string.IsNullOrWhiteSpace(authorName)) return Result.Fail("Bitte einen Namen eingeben.");
            var v = _replyValidator.Validate(reply);
            if (!v.IsValid) return Result.Fail(v.Errors.Select(e => e.ErrorMessage).ToArray());
            _replies.Add(reply);
            return Result.Ok();
        }

        public void DeleteReply(int replyId) => _replies.Delete(_replies.GetById(replyId));

        public void ToggleReply(int replyId)
        {
            var r = _replies.GetById(replyId);
            if (r == null) return;
            r.Status = !r.Status;
            _replies.Update(r);
        }

        private static string Short(string s) => s.Length > 60 ? s[..60] + "…" : s;
    }

    public class ContactManager : GenericManager<Contact>, IContactService
    {
        private readonly INotificationService _notify;
        public ContactManager(IContactDal dal, IValidator<Contact> v, INotificationService notify) : base(dal, v) { _notify = notify; }

        public Result Send(Contact contact)
        {
            contact.Date = DateTime.Now;
            contact.IsRead = false;
            var r = Add(contact);
            if (r.Success) _notify.Notify("contact", "Neue Nachricht", $"{contact.UserName}: {contact.Subject}", "/admin/contacts");
            return r;
        }

        public void MarkAsRead(int id, bool read = true)
        {
            var c = Dal.GetById(id);
            if (c == null || c.IsRead == read) return;
            c.IsRead = read;
            Dal.Update(c);
        }

        public int UnreadCount() => Dal.Count(c => !c.IsRead);
    }

    public class NewsLetterManager : GenericManager<NewsLetter>, INewsLetterService
    {
        private readonly INotificationService _notify;
        public NewsLetterManager(INewsLetterDal dal, IValidator<NewsLetter> v, INotificationService notify) : base(dal, v) { _notify = notify; }

        public Result Subscribe(string mail)
        {
            mail = mail?.Trim().ToLowerInvariant();
            if (!string.IsNullOrEmpty(mail) && Dal.Count(n => n.Mail == mail) > 0) return Result.Fail("Diese E-Mail-Adresse ist bereits angemeldet.");
            var r = Add(new NewsLetter { Mail = mail });
            if (r.Success) _notify.Notify("newsletter", "Newsletter", $"{mail} hat den Newsletter abonniert", "/admin/newsletter");
            return r;
        }
    }

    public class NotificationManager : GenericManager<Notification>, INotificationService
    {
        private readonly INotificationDal _dal;
        public NotificationManager(INotificationDal dal) : base(dal) { _dal = dal; }

        public List<Notification> GetLatest(int take) => Dal.GetAll().OrderByDescending(n => n.Timestamp).Take(take).ToList();
        public int UnreadCount() => Dal.Count(n => !n.ReadStatus);
        public void MarkAllAsRead() => _dal.MarkAllAsRead();
        public void Notify(string type, string title, string message, string url) =>
            Dal.Add(new Notification { Type = type, Title = title, Message = message, Url = url, Timestamp = DateTime.Now });
    }

    public class VisitorManager : IVisitorService
    {
        private readonly IVisitorCountDal _dal;
        public VisitorManager(IVisitorCountDal dal) => _dal = dal;
        public void Track() => _dal.Increment(DateTime.Today);
        public int Today() => _dal.GetAll(v => v.VisitDate == DateTime.Today).Sum(v => v.Count);
    }

    public class UserManagerService : IUserService
    {
        private readonly IUserDal _dal;
        public UserManagerService(IUserDal dal) => _dal = dal;
        public List<User> GetAllWithBlogCount(out Dictionary<int, int> blogCounts) => _dal.GetAllWithBlogCount(out blogCounts);
        public User GetById(int id) => _dal.GetById(id);
    }

    public class DashboardManager : IDashboardService
    {
        private readonly IBlogDal _blogs; private readonly ICommentDal _comments; private readonly IUserDal _users;
        private readonly IContactDal _contacts; private readonly INewsLetterDal _news; private readonly IPortfolioDal _portfolios;
        private readonly IPackageDal _packages; private readonly IVisitorCountDal _visitors; private readonly ICategoryDal _categories;

        public DashboardManager(IBlogDal blogs, ICommentDal comments, IUserDal users, IContactDal contacts, INewsLetterDal news,
            IPortfolioDal portfolios, IPackageDal packages, IVisitorCountDal visitors, ICategoryDal categories)
        {
            _blogs = blogs; _comments = comments; _users = users; _contacts = contacts; _news = news;
            _portfolios = portfolios; _packages = packages; _visitors = visitors; _categories = categories;
        }

        public DashboardStats GetStats()
        {
            var from = DateTime.Today.AddDays(-13);
            var range = _visitors.GetRange(from, DateTime.Today).ToDictionary(v => v.VisitDate.Date, v => v.Count);
            return new DashboardStats
            {
                Blogs = _blogs.Count(),
                Comments = _comments.Count(),
                PendingComments = _comments.Count(c => !c.Status),
                Users = _users.Count(),
                UnreadContacts = _contacts.Count(c => !c.IsRead),
                Subscribers = _news.Count(n => n.Status),
                Portfolios = _portfolios.Count(),
                Packages = _packages.Count(),
                VisitorsToday = range.TryGetValue(DateTime.Today, out var t) ? t : 0,
                VisitorsTotal = _visitors.GetAll().Sum(v => v.Count),
                VisitorsLast14Days = Enumerable.Range(0, 14).Select(i => from.AddDays(i))
                    .Select(d => (d, range.TryGetValue(d, out var c) ? c : 0)).ToList(),
                BlogsPerCategory = _categories.GetWithBlogCount(false).Select(x => (x.Category.Name, x.BlogCount)).ToList(),
            };
        }
    }
}
