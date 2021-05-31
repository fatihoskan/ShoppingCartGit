using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using ShoppingCart.Db.Data;
using ShoppingCart.Db.Persistence;
using System;
using System.Collections.Generic;

namespace ShoppingCart.Db.Extensions
{
    public static class MigrationExtensions
    {
        public static void EnsureMigration(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetService<ShoppingCartDbContext>();
            if (dbContext.Database.IsSqlServer())
            {
                ///db yoksa yaratılacak ve yeni objeler eklencek
                if (dbContext.Database.EnsureCreated())
                {
                    SeedNewData(dbContext);

                }
                else
                {
                    dbContext.Database.Migrate();
                }
            }
        }

        private static void SeedNewData(ShoppingCartDbContext dbContext)
        {
            User user = new()
            {
                Email = "fatihoskan@gmail.com",
                Id = Guid.Parse("681325d1-7858-43f0-99bb-0ea292940492"),
                Name = "Fatih ÖSKAN",
                Username = "fatihoskan",
                DateOfBirth = new DateTime(1985, 9, 18),
                CreatedBy = "system"
            };

            dbContext.Users.Add(user);

            Product product = new()
            {
                Id = Guid.Parse("7C124CFB-5578-4232-8D44-16C20B5FBF46"),
                Name = "Kol saati",
                Price = 159.99M,
                Count = 100,
                CreatedBy = "system"
            };

            Product product2 = new()
            {
                Id = Guid.Parse("5D9BB3BC-84B2-45D9-9FDC-E6A4C28D541E"),
                Name = "Kalem",
                Price = 9.99M,
                Count = 1,
                CreatedBy = "system"
            };

            Product product3 = new()
            {
                Id = Guid.Parse("C3EA27F3-0EBB-4A8C-8009-7F13D5628CEE"),
                Name = "Kulaklık",
                Price = 19.99M,
                Count = 7,
                CreatedBy = "system"
            };

            dbContext.Products.AddRange(new List<Product>() { product3, product2, product });

            Cart cart = new Cart
            {
                SessionId = Guid.Parse("e56d2f23-536c-474c-b3f0-eb046fde66e0"),
                UserId = Guid.Parse("21F48D20-06E7-47DC-8FF0-E93FE1E70A7C"),
                Status = Common.Enums.CartStatus.Active,
                TotalPrice = 100
            };

            dbContext.Carts.Add(cart);

            dbContext.SaveChanges();

            /*
             * 
             * test için user eklendi ona ait token bilgileri....
             * 
             eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiRmF0aWggw5Zza2FuIiwiaWF0IjoxNTE2MjM5MDIyLCJ1bmlxdWVfbmFtZSI6IjY4MTMyNWQxLTc4NTgtNDNmMC05OWJiLTBlYTI5Mjk0MDQ5MiIsImVtYWlsIjoiZmF0aWhvc2thbkBnbWFpbC5jb20ifQ.SMs9LzNgYQHOsQQncrXGUHRYmfWLMsicoIauYT02ABw
                {  
                    "name": "Fatih Öskan",
                    "iat": 1516239022,
                    "unique_name" : "681325d1-7858-43f0-99bb-0ea292940492",
                    "email":"fatihoskan@gmail.com"
                }
             */
        }
    }
}
