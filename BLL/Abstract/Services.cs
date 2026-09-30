using BE;
using BLL.Common;

namespace BLL.Abstract
{
    public interface IGenericService<T> where T : class
    {
        Result Add(T item);
        Result Update(T item);
        void Delete(T item);
        T GetById(int id);
        List<T> GetAll();
    }

    /// <summary>Service for content with a Status flag (published / hidden) and an optional sort order.</summary>
    public interface IContentService<T> : IGenericService<T> where T : class
    {
        List<T> GetList(bool onlyActive);
    }

    public interface IAboutService : IContentService<About> { About Get(); }
    public interface IOurContactService : IContentService<OurContact> { OurContact Get(); }
    public interface IServiceService : IContentService<Services> { }
    public interface ISuccessService : IContentService<Success> { }
    public interface IPackageService : IContentService<Package> { }
    public interface IVideoService : IContentService<Video> { }
    public interface ICustomerCommentService : IContentService<CustomerComment> { }
    public interface IPortfolioCategoryService : IContentService<PortfolioCategory> { }

    public interface ICategoryService : IContentService<Category>
    {
        List<(Category Category, int BlogCount)> GetWithBlogCount(bool onlyActive);
        Result SafeDelete(int id);
    }

    public interface IPortfolioService : IContentService<Portfolio>
    {
        List<Portfolio> GetWithCategory(bool onlyActive);
    }

    public interface IBlogService : IContentService<Blog>
    {
        PagedList<Blog> GetPaged(bool onlyActive, int? categoryId, string search, int page, int pageSize = 6);
        Blog GetDetail(int id);
        Blog ReadForVisitor(int id);
        List<Blog> GetLatest(int take, int? exceptId = null, int? categoryId = null);
        List<Blog> GetPopular(int take);
    }

    public interface ICommentService : IGenericService<Comment>
    {
        List<Comment> GetForBlog(int blogId);
        List<Comment> GetForAdmin(bool? approved);
        Comment GetDetail(int id);
        Result AddFromVisitor(Comment comment);
        void ToggleApproval(int id);
        Result AddTeamReply(int commentId, User author, string content);
        Result AddVisitorReply(int commentId, string authorName, int? userId, string content);
        void DeleteReply(int replyId);
        void ToggleReply(int replyId);
        int PendingCount();
    }

    public interface IContactService : IGenericService<Contact>
    {
        Result Send(Contact contact);
        void MarkAsRead(int id, bool read = true);
        int UnreadCount();
    }

    public interface INewsLetterService : IGenericService<NewsLetter>
    {
        Result Subscribe(string mail);
    }

    public interface INotificationService : IGenericService<Notification>
    {
        List<Notification> GetLatest(int take);
        int UnreadCount();
        void MarkAllAsRead();
        void Notify(string type, string title, string message, string url);
    }

    public interface IVisitorService
    {
        void Track();
        int Today();
    }

    public interface IDashboardService
    {
        DashboardStats GetStats();
    }

    public interface IUserService
    {
        List<User> GetAllWithBlogCount(out Dictionary<int, int> blogCounts);
        User GetById(int id);
    }
}
