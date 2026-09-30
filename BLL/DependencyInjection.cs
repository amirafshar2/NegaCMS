using BE;
using BLL.Abstract;
using BLL.Concrete;
using BLL.ValidationRules;
using DAL.Abstract;
using DAL.EntityFrameWork;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BLL
{
    /// <summary>Registers every repository (DAL) and manager (BLL) in the DI container.</summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddNegaLayers(this IServiceCollection s)
        {
            // DAL
            s.AddScoped<IAboutDal, EFAboutRepository>();
            s.AddScoped<IBlogDal, EFBlogRepository>();
            s.AddScoped<ICategoryDal, EFCategoryRepository>();
            s.AddScoped<ICommentDal, EFCommentRepository>();
            s.AddScoped<IReplyDal, EFReplyRepository>();
            s.AddScoped<IContactDal, EFContactRepository>();
            s.AddScoped<ICustomerCommentDal, EFCustomerCommentRepository>();
            s.AddScoped<INewsLetterDal, EFNewsLetterRepository>();
            s.AddScoped<INotificationDal, EFNotificationRepository>();
            s.AddScoped<IOurContactDal, EFOurContactRepository>();
            s.AddScoped<IPackageDal, EFPackageRepository>();
            s.AddScoped<IPortfolioDal, EFPortfolioRepository>();
            s.AddScoped<IPortfolioCategoryDal, EFPortfolioCategoryRepository>();
            s.AddScoped<IServiceDal, EFServiceRepository>();
            s.AddScoped<ISuccessDal, EFSuccessRepository>();
            s.AddScoped<IVideoDal, EFVideoRepository>();
            s.AddScoped<IVisitorCountDal, EFVisitorCountRepository>();
            s.AddScoped<IUserDal, EFUserRepository>();

            // Validation
            s.AddScoped<IValidator<Blog>, BlogValidator>();
            s.AddScoped<IValidator<Category>, CategoryValidator>();
            s.AddScoped<IValidator<Comment>, CommentValidator>();
            s.AddScoped<IValidator<Reply>, ReplyValidator>();
            s.AddScoped<IValidator<Contact>, ContactValidator>();
            s.AddScoped<IValidator<NewsLetter>, NewsLetterValidator>();
            s.AddScoped<IValidator<Package>, PackageValidator>();
            s.AddScoped<IValidator<Portfolio>, PortfolioValidator>();
            s.AddScoped<IValidator<PortfolioCategory>, PortfolioCategoryValidator>();
            s.AddScoped<IValidator<Services>, ServiceValidator>();
            s.AddScoped<IValidator<Success>, SuccessValidator>();
            s.AddScoped<IValidator<Video>, VideoValidator>();
            s.AddScoped<IValidator<CustomerComment>, CustomerCommentValidator>();
            s.AddScoped<IValidator<About>, AboutValidator>();
            s.AddScoped<IValidator<OurContact>, OurContactValidator>();

            // BLL
            s.AddScoped<IAboutService, AboutManager>();
            s.AddScoped<IOurContactService, OurContactManager>();
            s.AddScoped<IServiceService, ServiceManager>();
            s.AddScoped<ISuccessService, SuccessManager>();
            s.AddScoped<IPackageService, PackageManager>();
            s.AddScoped<IVideoService, VideoManager>();
            s.AddScoped<ICustomerCommentService, CustomerCommentManager>();
            s.AddScoped<IPortfolioCategoryService, PortfolioCategoryManager>();
            s.AddScoped<ICategoryService, CategoryManager>();
            s.AddScoped<IPortfolioService, PortfolioManager>();
            s.AddScoped<IBlogService, BlogManager>();
            s.AddScoped<ICommentService, CommentManager>();
            s.AddScoped<IContactService, ContactManager>();
            s.AddScoped<INewsLetterService, NewsLetterManager>();
            s.AddScoped<INotificationService, NotificationManager>();
            s.AddScoped<IVisitorService, VisitorManager>();
            s.AddScoped<IUserService, UserManagerService>();
            s.AddScoped<IDashboardService, DashboardManager>();
            return s;
        }
    }
}
