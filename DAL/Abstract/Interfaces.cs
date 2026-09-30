using BE;

namespace DAL.Abstract
{
    public interface IAboutDal : IGenericDal<About> { }
    public interface ICategoryDal : IGenericDal<Category>
    {
        List<(Category Category, int BlogCount)> GetWithBlogCount(bool onlyActive);
    }
    public interface IBlogDal : IGenericDal<Blog>
    {
        Blog GetWithRelations(int id);
        (List<Blog> Items, int Total) GetPaged(bool onlyActive, int? categoryId, string search, int page, int pageSize);
        List<Blog> GetLatest(int take, int? exceptId = null, int? categoryId = null);
        List<Blog> GetPopular(int take);
        void IncreaseView(int id);
    }
    public interface ICommentDal : IGenericDal<Comment>
    {
        List<Comment> GetForBlog(int blogId, bool onlyApproved);
        List<Comment> GetAllWithRelations(bool? approved);
        Comment GetWithRelations(int id);
    }
    public interface IReplyDal : IGenericDal<Reply> { }
    public interface IContactDal : IGenericDal<Contact> { }
    public interface ICustomerCommentDal : IGenericDal<CustomerComment> { }
    public interface INewsLetterDal : IGenericDal<NewsLetter> { }
    public interface INotificationDal : IGenericDal<Notification>
    {
        void MarkAllAsRead();
    }
    public interface IOurContactDal : IGenericDal<OurContact> { }
    public interface IPackageDal : IGenericDal<Package> { }
    public interface IPortfolioDal : IGenericDal<Portfolio>
    {
        List<Portfolio> GetWithCategory(bool onlyActive);
        Portfolio GetWithCategory(int id);
    }
    public interface IPortfolioCategoryDal : IGenericDal<PortfolioCategory> { }
    public interface IServiceDal : IGenericDal<Services> { }
    public interface ISuccessDal : IGenericDal<Success> { }
    public interface IVideoDal : IGenericDal<Video> { }
    public interface IVisitorCountDal : IGenericDal<VisitorCount>
    {
        void Increment(DateTime day);
        List<VisitorCount> GetRange(DateTime from, DateTime to);
    }
    public interface IUserDal : IGenericDal<User>
    {
        User GetByUserName(string userName);
        List<User> GetAllWithBlogCount(out Dictionary<int, int> blogCounts);
    }
}
