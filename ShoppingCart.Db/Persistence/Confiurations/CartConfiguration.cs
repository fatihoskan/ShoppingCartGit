using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingCart.Db.Data;

namespace ShoppingCart.Db.Persistence.Confiurations
{
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.Property(x => x.Id).HasDefaultValueSql("NEWID()");
            builder.HasIndex(r => r.Id).HasDatabaseName("CartIdIndex").IsUnique();
            builder.Property(p => p.TotalPrice).HasColumnType("decimal(18,2)");
        }
    }
}
