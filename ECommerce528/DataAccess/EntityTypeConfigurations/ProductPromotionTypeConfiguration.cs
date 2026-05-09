using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce528.DataAccess.EntityTypeConfigurations
{
    public class ProductPromotionTypeConfiguration : IEntityTypeConfiguration<ProductPromotion>
    {
        public void Configure(EntityTypeBuilder<ProductPromotion> builder)
        {
            builder
                .HasIndex(b => b.Code)
                .IsUnique();
        }
    }
}
