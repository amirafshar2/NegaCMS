using BE;
using BLL.Common;

namespace Negacom.Models
{
    public class HomeViewModel
    {
        public OurContact Settings { get; set; }
        public About About { get; set; }
        public List<Services> Services { get; set; }
        public List<Success> Counters { get; set; }
        public List<Portfolio> Portfolios { get; set; }
        public List<PortfolioCategory> PortfolioCategories { get; set; }
        public Video Video { get; set; }
        public List<Package> Packages { get; set; }
        public List<CustomerComment> Testimonials { get; set; }
        public List<Blog> LatestBlogs { get; set; }
        public ContactForm Contact { get; set; } = new();
    }

    public class ContactForm
    {
        public string UserName { get; set; }
        public string Mail { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }

    public class BlogListViewModel
    {
        public PagedList<Blog> Blogs { get; set; }
        public List<(Category Category, int BlogCount)> Categories { get; set; }
        public List<Blog> Popular { get; set; }
        public int? CategoryId { get; set; }
        public string Search { get; set; }
        public Category ActiveCategory => Categories?.Select(c => c.Category).FirstOrDefault(c => c.Id == CategoryId);
    }

    public class BlogDetailViewModel
    {
        public Blog Blog { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Blog> Related { get; set; }
        public List<(Category Category, int BlogCount)> Categories { get; set; }
        public CommentForm Form { get; set; } = new();
    }

    public class CommentForm
    {
        public int BlogId { get; set; }
        public string AuthorName { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }
    }

    public class ErrorViewModel
    {
        public int StatusCode { get; set; }
    }
}
