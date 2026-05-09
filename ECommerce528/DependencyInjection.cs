using ECommerce528.Services.IServices;
using ECommerce528.Utilities.DbInitializers;

namespace ECommerce528
{
    public static class DependencyInjection
    {
        public static void Configure(this IServiceCollection services)
        {
            services.AddScoped<IRepository<Category>, Repository<Category>>();
            services.AddScoped<IRepository<Category>, Repository<Category>>();
            services.AddScoped<IRepository<Brand>, Repository<Brand>>();
            services.AddScoped<IRepository<Product>, Repository<Product>>();
            services.AddScoped<IRepository<ApplicationUserOTP>, Repository<ApplicationUserOTP>>();
            services.AddScoped<IRepository<Cart>, Repository<Cart>>();
            services.AddScoped<IRepository<FavoriteItem>, Repository<FavoriteItem>>();
            services.AddScoped<IRepository<ProductPromotion>, Repository<ProductPromotion>>();
            services.AddScoped<IRepository<PromotionUserUsage>, Repository<PromotionUserUsage>>();
            services.AddScoped<IProductSubImgRepository, ProductSubImgRepository>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IDbInitializer, DbInitializer>();
        }
    }
}
