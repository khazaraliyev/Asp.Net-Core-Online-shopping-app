using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols;
using ShopApp.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Xml.XPath;

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

            //many to many relationship
            modelBuilder.Entity<ProductCategory>().HasKey(p => new { p.CategoryId, p.ProductId });
            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Product).WithMany(pc => pc.ProductCategories)
                .HasForeignKey(pc => pc.ProductId);
            modelBuilder.Entity<ProductCategory>()
               .HasOne(pc => pc.Category).WithMany(pc => pc.ProductCategories)
               .HasForeignKey(pc => pc.CategoryId);

            //Product table validations
            modelBuilder.Entity<Product>().Property(p => p.Name).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Product>().Property(p => p.Price).IsRequired().HasMaxLength(20);
            modelBuilder.Entity<Product>().Property(p => p.Description).IsRequired().HasMaxLength(500);
            modelBuilder.Entity<Product>().Property(p => p.IsApproved).HasDefaultValue(false);
            modelBuilder.Entity<Product>().Property(p => p.IsHome).HasDefaultValue(false);
            modelBuilder.Entity<Product>().Property(p => p.ImageUrl).IsRequired().HasMaxLength(50);

            //Order table validations
            modelBuilder.Entity<Order>().Property(p => p.FirstName).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Order>().Property(p => p.LastName).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Order>().Property(p => p.Address).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Order>().Property(p => p.UserId).IsRequired();
            modelBuilder.Entity<Order>().Property(p => p.City).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Order>().Property(p => p.Email).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Order>().Property(p => p.OrderDate).IsRequired();
            modelBuilder.Entity<Order>().Property(p => p.FirstName).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Order>().Property(p => p.FirstName).IsRequired().HasMaxLength(50);

            //Card table validation
            modelBuilder.Entity<Card>().Property(p => p.UserId).IsRequired();

            //Category table validation
            modelBuilder.Entity<Category>().Property(p => p.Name).IsRequired().HasMaxLength(20);
        }
    }
}
