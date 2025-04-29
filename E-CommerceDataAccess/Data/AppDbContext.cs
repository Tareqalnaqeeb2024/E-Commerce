using E_CommerceDataAccess.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.Data
{
    public class AppDbContext : IdentityDbContext<UserAccount>

    {
        public AppDbContext(DbContextOptions options ):base(options)
        {
            
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> Items { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>(product =>
            {
            product.HasOne(p => p.Category)
                  .WithMany(p => p.Products)
                  .HasForeignKey(p => p.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Product>(product =>
            {
                product.HasMany(p => p.OrderItems)
                       .WithOne(oi => oi.Product)
                       .HasForeignKey(oi => oi.ProductId)
                       .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Order>(order =>
            {

                order.HasMany(o => o.OrderItems)
                      .WithOne(o => o.Order)
                      .HasForeignKey(o => o.OrderId)
                      .OnDelete(DeleteBehavior.Cascade); // when delete order also orderItem delete
            });

            builder.Entity<UserAccount>(user =>
            {
                user.HasMany(u => u.Orders)
                    .WithOne(o => o.User)
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


        }
    }
}
