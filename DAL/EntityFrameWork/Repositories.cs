using BE;
using DAL.Abstract;
using DAL.Context;
using DAL.Repository;
using Microsoft.EntityFrameworkCore;

namespace DAL.EntityFrameWork
{
    public class EFAboutRepository : GenericRepository<About>, IAboutDal { public EFAboutRepository(DB db) : base(db) { } }
    public class EFReplyRepository : GenericRepository<Reply>, IReplyDal { public EFReplyRepository(DB db) : base(db) { } }
    public class EFContactRepository : GenericRepository<Contact>, IContactDal { public EFContactRepository(DB db) : base(db) { } }
    public class EFCustomerCommentRepository : GenericRepository<CustomerComment>, ICustomerCommentDal { public EFCustomerCommentRepository(DB db) : base(db) { } }
    public class EFNewsLetterRepository : GenericRepository<NewsLetter>, INewsLetterDal { public EFNewsLetterRepository(DB db) : base(db) { } }
    public class EFOurContactRepository : GenericRepository<OurContact>, IOurContactDal { public EFOurContactRepository(DB db) : base(db) { } }
    public class EFPackageRepository : GenericRepository<Package>, IPackageDal { public EFPackageRepository(DB db) : base(db) { } }
    public class EFPortfolioCategoryRepository : GenericRepository<PortfolioCategory>, IPortfolioCategoryDal { public EFPortfolioCategoryRepository(DB db) : base(db) { } }
    public class EFServiceRepository : GenericRepository<Services>, IServiceDal { public EFServiceRepository(DB db) : base(db) { } }
    public class EFSuccessRepository : GenericRepository<Success>, ISuccessDal { public EFSuccessRepository(DB db) : base(db) { } }
    public class EFVideoRepository : GenericRepository<Video>, IVideoDal { public EFVideoRepository(DB db) : base(db) { } }

    public class EFCategoryRepository : GenericRepository<Category>, ICategoryDal
    {
        public EFCategoryRepository(DB db) : base(db) { }

        public List<(Category Category, int BlogCount)> GetWithBlogCount(bool onlyActive)
        {
            var q = Db.Categories.AsNoTracking().AsQueryable();
            if (onlyActive) q = q.Where(c => c.Status);
            return q.OrderBy(c => c.Name)
                .Select(c => new { c, n = c.Blogs.Count(b => !onlyActive || b.Status) })
                .AsEnumerable()
                .Select(x => (x.c, x.n))
                .ToList();
        }
    }

    public class EFBlogRepository : GenericRepository<Blog>, IBlogDal
    {
        public EFBlogRepository(DB db) : base(db) { }

        private IQueryable<Blog> WithRel() =>
            Db.Blogs.AsNoTracking().Include(b => b.Category).Include(b => b.User);

        public Blog GetWithRelations(int id) => WithRel().FirstOrDefault(b => b.Id == id);

        public (List<Blog> Items, int Total) GetPaged(bool onlyActive, int? categoryId, string search, int page, int pageSize)
        {
            var q = WithRel();
            if (onlyActive) q = q.Where(b => b.Status);
            if (categoryId.HasValue) q = q.Where(b => b.CategoryId == categoryId);
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                q = q.Where(b => b.Title.Contains(s) || b.Summary.Contains(s) || b.Content.Contains(s));
            }
            var total = q.Count();
            var items = q.OrderByDescending(b => b.Date).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return (items, total);
        }

        public List<Blog> GetLatest(int take, int? exceptId = null, int? categoryId = null)
        {
            var q = WithRel().Where(b => b.Status);
            if (exceptId.HasValue) q = q.Where(b => b.Id != exceptId);
            if (categoryId.HasValue) q = q.Where(b => b.CategoryId == categoryId);
            return q.OrderByDescending(b => b.Date).Take(take).ToList();
        }

        public List<Blog> GetPopular(int take) =>
            WithRel().Where(b => b.Status).OrderByDescending(b => b.ViewCount).Take(take).ToList();

        public void IncreaseView(int id)
        {
            var b = Db.Blogs.Find(id);
            if (b == null) return;
            b.ViewCount++;
            Db.SaveChanges();
        }
    }

    public class EFCommentRepository : GenericRepository<Comment>, ICommentDal
    {
        public EFCommentRepository(DB db) : base(db) { }

        public List<Comment> GetForBlog(int blogId, bool onlyApproved) =>
            Db.Comments.AsNoTracking()
                .Include(c => c.User)
                .Include(c => c.Replies.Where(r => r.Status)).ThenInclude(r => r.User)
                .Where(c => c.BlogId == blogId && (!onlyApproved || c.Status))
                .OrderByDescending(c => c.Date)
                .ToList();

        public List<Comment> GetAllWithRelations(bool? approved)
        {
            var q = Db.Comments.AsNoTracking().Include(c => c.Blog).Include(c => c.User).Include(c => c.Replies).AsQueryable();
            if (approved.HasValue) q = q.Where(c => c.Status == approved.Value);
            return q.OrderByDescending(c => c.Date).ToList();
        }

        public Comment GetWithRelations(int id) =>
            Db.Comments.AsNoTracking().Include(c => c.Blog).Include(c => c.User)
                .Include(c => c.Replies).ThenInclude(r => r.User).FirstOrDefault(c => c.Id == id);
    }

    public class EFNotificationRepository : GenericRepository<Notification>, INotificationDal
    {
        public EFNotificationRepository(DB db) : base(db) { }

        public void MarkAllAsRead()
        {
            foreach (var n in Db.Notifications.Where(n => !n.ReadStatus)) n.ReadStatus = true;
            Db.SaveChanges();
        }
    }

    public class EFPortfolioRepository : GenericRepository<Portfolio>, IPortfolioDal
    {
        public EFPortfolioRepository(DB db) : base(db) { }

        public List<Portfolio> GetWithCategory(bool onlyActive)
        {
            var q = Db.Portfolios.AsNoTracking().Include(p => p.PortfolioCategory).AsQueryable();
            if (onlyActive) q = q.Where(p => p.Status);
            return q.OrderByDescending(p => p.Date).ToList();
        }

        public Portfolio GetWithCategory(int id) =>
            Db.Portfolios.AsNoTracking().Include(p => p.PortfolioCategory).FirstOrDefault(p => p.Id == id);
    }

    public class EFVisitorCountRepository : GenericRepository<VisitorCount>, IVisitorCountDal
    {
        public EFVisitorCountRepository(DB db) : base(db) { }

        public void Increment(DateTime day)
        {
            day = day.Date;
            var row = Db.VisitorCounts.FirstOrDefault(v => v.VisitDate == day);
            if (row == null) Db.VisitorCounts.Add(new VisitorCount { VisitDate = day, Count = 1 });
            else row.Count++;
            Db.SaveChanges();
        }

        public List<VisitorCount> GetRange(DateTime from, DateTime to) =>
            Db.VisitorCounts.AsNoTracking().Where(v => v.VisitDate >= from.Date && v.VisitDate <= to.Date)
                .OrderBy(v => v.VisitDate).ToList();
    }

    public class EFUserRepository : GenericRepository<User>, IUserDal
    {
        public EFUserRepository(DB db) : base(db) { }

        public User GetByUserName(string userName) =>
            Db.Users.AsNoTracking().FirstOrDefault(u => u.UserName == userName);

        public List<User> GetAllWithBlogCount(out Dictionary<int, int> blogCounts)
        {
            blogCounts = Db.Blogs.GroupBy(b => b.UserId).Select(g => new { g.Key, C = g.Count() })
                .ToDictionary(x => x.Key, x => x.C);
            return Db.Users.AsNoTracking().OrderBy(u => u.Id).ToList();
        }
    }
}
