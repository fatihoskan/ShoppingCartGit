using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingCart.Db.Data;

namespace ShoppingCart.Db.Persistence.Confiurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.Id).HasDefaultValueSql("NEWID()");
            builder.HasIndex(r => r.Id).HasDatabaseName("UserIdIndex").IsUnique();
        }
    }
}
