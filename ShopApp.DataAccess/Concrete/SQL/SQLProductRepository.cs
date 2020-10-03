using Microsoft.EntityFrameworkCore;
using ShopApp.DataAccess.Abstract;
using ShopApp.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShopApp.DataAccess.Concrete.SQL
{
    public class SQLProductRepository : SQLGenericRepository<Product, ShopContext>, IProductRepository
    {
        public Product GetByIdWithCategories(int id)
        {
            using (var context = new ShopContext())
            {
                return context.Products
                               .Where(p => p.ProductId == id)
                               .Include(p => p.ProductCategories)
                               .ThenInclude(p => p.Category)
                               .FirstOrDefault();
            }
        }

        public int GetCountByCategory(string category)
        {
            using (var context = new ShopContext())
            {
                var products = context.Products.AsQueryable();

                if (!string.IsNullOrEmpty(category))
                {
                    products = products
                                .Include(i => i.ProductCategories)
                                .ThenInclude(i => i.Category)
                                .Where(i => i.ProductCategories.Any(a => a.Category.Name.ToLower() == category.ToLower()));
                }

                return products.Count();
            }
        }

        public Product GetDetailsWithCategory(int id)
        {
            using (var context = new ShopContext())
            {
                return context.Products.Where(p => p.ProductId == id)
                    .Include(p => p.ProductCategories).ThenInclude(p => p.Category).FirstOrDefault();
            }
        }

        public List<Product> GetHomePageProducts()
        {
            using (var context = new ShopContext())
            {
                return context.Products.Where(p => p.IsApproved && p.IsHome).ToList();
            }
        }

        public List<Product> GetProductsByCategory(string category, int page, int pageSize)
        {
            using (var context = new ShopContext())
            {
                var products = context.Products.AsQueryable();

                if (!string.IsNullOrEmpty(category))
                {
                    products = products
                                .Include(i => i.ProductCategories)
                                .ThenInclude(i => i.Category)
                                .Where(i => i.ProductCategories.Any(a => a.Category.Name.ToLower() == category.ToLower()));
                }

                return products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        public List<Product> GetSearchResult(string searchString)
        {
            using (var context = new ShopContext())
            {
                var products = context.Products
                    .Where(p => p.IsApproved && (p.Name.ToLower().Contains(searchString.ToLower()) || p.Description.ToLower().Contains(searchString.ToLower())))
                    .AsQueryable();
                return products.ToList();
            }
        }

        public void Update(Product product, int[] categoryId)
        {
            using (var context = new ShopContext())
            {
                var editProduct = context.Products
                    .Include(p => p.ProductCategories)
                    .FirstOrDefault(p => p.ProductId == product.ProductId);

                if (editProduct != null)
                {
                    editProduct.Name = product.Name;
                    editProduct.Price = product.Price;
                    editProduct.Description = product.Description;
                    editProduct.ImageUrl = product.ImageUrl;
                    editProduct.IsHome = product.IsHome;
                    editProduct.IsApproved = product.IsApproved; 

                    editProduct.ProductCategories = categoryId.Select(catId => new ProductCategory()
                    {
                        ProductId = product.ProductId,
                        CategoryId = catId
                    }).ToList();
                    context.SaveChanges();
                }
            }
        }
    }
}
