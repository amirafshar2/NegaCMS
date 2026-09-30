using BE;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.Context
{
    public class DB : IdentityDbContext<User, UserRole, int>
    {
        public DB(DbContextOptions<DB> options) : base(options) { }

        public DbSet<About> Abouts { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Reply> Replies { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<CustomerComment> CustomerComments { get; set; }
        public DbSet<NewsLetter> NewsLetters { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<OurContact> OurContacts { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<PortfolioCategory> PortfolioCategories { get; set; }
        public DbSet<Services> Services { get; set; }
        public DbSet<Success> Successes { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<VisitorCount> VisitorCounts { get; set; }

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            b.Entity<Blog>(e =>
            {
                e.Property(x => x.Title).IsRequired().HasMaxLength(200);
                e.HasIndex(x => new { x.Status, x.Date });
                e.HasOne(x => x.Category).WithMany(c => c.Blogs).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.User).WithMany(u => u.Blogs).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            });

            b.Entity<Comment>(e =>
            {
                e.HasOne(c => c.Blog).WithMany(x => x.Comments).HasForeignKey(c => c.BlogId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(c => c.User).WithMany(u => u.Comments).HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.SetNull);
            });

            b.Entity<Reply>(e =>
            {
                e.HasOne(r => r.Comment).WithMany(c => c.Replies).HasForeignKey(r => r.CommentId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(r => r.User).WithMany(u => u.Replies).HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.SetNull);
            });

            b.Entity<Portfolio>()
                .HasOne(p => p.PortfolioCategory).WithMany(c => c.Portfolios)
                .HasForeignKey(p => p.PortfolioCategoryId).OnDelete(DeleteBehavior.Restrict);

            b.Entity<Package>().Property(p => p.Price).HasConversion<double>();
            b.Entity<VisitorCount>().HasIndex(v => v.VisitDate).IsUnique();
            b.Entity<Notification>().HasIndex(n => new { n.ReadStatus, n.Timestamp });
        }
    }
}
