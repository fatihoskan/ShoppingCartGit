using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingCart.Db.Data;

namespace ShoppingCart.Db.Persistence.Confiurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(x => x.Id).HasDefaultValueSql("NEWID()");
            builder.HasIndex(r => r.Id).HasDatabaseName("ProductIdIndex").IsUnique();
            builder.Property(p => p.Price).HasColumnType("decimal(18,2)");
        }
    }
}
