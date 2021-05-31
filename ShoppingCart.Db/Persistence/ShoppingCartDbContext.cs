using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ShoppingCart.Common.Core.Data;
using ShoppingCart.Common.Extensions;
using ShoppingCart.Db.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Db.Persistence
{
    public class ShoppingCartDbContext : DbContext
    {
        private readonly CurrentUser currentUser;

        public ShoppingCartDbContext(DbContextOptions<ShoppingCartDbContext> options, CurrentUser currentUser = null)
           : base(options)
        {
            this.currentUser = currentUser;
        }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CartProduct> CartProducts { get; set; }
        public DbSet<User> Users { get; set; }

        public override int SaveChanges()
        {
            SetAuditableEntity();
            return base.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            SetAuditableEntity();
            return await base.SaveChangesAsync();
        }
        private void SetAuditableEntity()
        {
            var entityObjects = ChangeTracker.Entries().Where(e => (e.State == EntityState.Modified || e.State == EntityState.Added));
            foreach (var entityObject in entityObjects)
            {
                if (entityObject.Entity is IAuditableEntity auditable)
                {
                    if (entityObject.State == EntityState.Added)
                    {
                        auditable.DateCreated = DateTime.UtcNow;
                        entityObject.Property("DateCreated").IsModified = false;
                    }

                    if (entityObject.State == EntityState.Modified)
                    {
                        auditable.DateUpdated = DateTime.UtcNow;
                    }
                }

                if (entityObject.Entity is AuditableEntity changedOrAddedItem)
                {
                    if (entityObject.State == EntityState.Added)
                    {
                        if (currentUser != null)
                        {
                            changedOrAddedItem.CreatedBy = currentUser.Username;
                            entityObject.Property("CreatedBy").IsModified = false;
                        }
                    }

                    if (entityObject.State == EntityState.Modified)
                    {
                        if (currentUser != null)
                        {
                            changedOrAddedItem.UpdatedBy = currentUser.Username;
                        }
                    }
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            modelBuilder.UseValueConverterForType(typeof(DateTime), dateTimeConverter);
            modelBuilder.UseValueConverterForType(typeof(DateTime?), dateTimeConverter);

            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShoppingCartDbContext).Assembly);
        }
    }
}