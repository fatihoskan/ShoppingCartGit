using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingCart.Db.Data;

namespace ShoppingCart.Db.Persistence.Confiurations
{
    public class CartProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(x => x.Id).HasDefaultValueSql("NEWID()");
            builder.HasIndex(r => r.Id).HasDatabaseName("CartProductIdIndex").IsUnique();
        }
    }
}
