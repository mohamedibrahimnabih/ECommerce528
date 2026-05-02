using ECommerce528.DataAccess.EntityTypeConfigurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ECommerce528.ViewModels;

namespace ECommerce528.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        //public ApplicationDbContext()
        //{
        //}

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<ProductSubImg> ProductSubImgs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);

        //    optionsBuilder.UseSqlServer("");
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductTypeConfiguration).Assembly);
        }
        public DbSet<ECommerce528.ViewModels.RegisterVM> RegisterVM { get; set; } = default!;
        public DbSet<ECommerce528.ViewModels.LoginVM> LoginVM { get; set; } = default!;
        public DbSet<ECommerce528.ViewModels.ResendEmailConfirmationVM> ResendEmailConfirmationVM { get; set; } = default!;
    }
}
