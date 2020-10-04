using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols;
using ShopApp.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ShopApp.DataAccess.Concrete.SQL
{
    public class ShopContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=localhost;database=ShopDB;Trusted_Connection=True;MultipleActiveResultSets=true");
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Card> Card { get; set; }
        public DbSet<CardItem> CardItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductCategory>().HasKey(p => new { p.CategoryId, p.ProductId });
            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Product).WithMany(pc => pc.ProductCategories)
                .HasForeignKey(pc => pc.ProductId);
            modelBuilder.Entity<ProductCategory>()
               .HasOne(pc => pc.Category).WithMany(pc => pc.ProductCategories)
               .HasForeignKey(pc => pc.CategoryId);
        }
    }
}
