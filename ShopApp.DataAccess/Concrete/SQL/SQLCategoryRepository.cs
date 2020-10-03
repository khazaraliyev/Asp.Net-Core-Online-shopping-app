using Microsoft.EntityFrameworkCore;
using ShopApp.DataAccess.Abstract;
using ShopApp.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShopApp.DataAccess.Concrete.SQL
{
    public class SQLCategoryRepository : SQLGenericRepository<Category, ShopContext>, ICategoryRepository
    {
        public void DeleteFromCategory(int productId, int categoryId)
        {
            using (var context=new ShopContext())
            {
                var query = "delete from productcategory where ProductId=@p0 and CategoryId=@p1";
                context.Database.ExecuteSqlRaw(query, productId, categoryId);
            }
        }

        public Category GetByIdWithProducts(int categoryId)
        {
            using (var context=new ShopContext())
            {
                return context.Categories
                              .Where(c => c.CategoryId == categoryId)
                              .Include(c => c.ProductCategories)
                              .ThenInclude(c => c.Product)
                              .FirstOrDefault();
            }
        }
    }
}
