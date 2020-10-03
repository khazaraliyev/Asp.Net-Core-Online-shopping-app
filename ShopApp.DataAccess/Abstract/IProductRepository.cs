using ShopApp.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopApp.DataAccess.Abstract
{
    public interface IProductRepository : IRepository<Product>
    {
        Product GetDetailsWithCategory(int id);
        int GetCountByCategory(string category);
        List<Product> GetProductsByCategory(string name,int page,int pageSize);
        List<Product> GetHomePageProducts();
        Product GetByIdWithCategories(int id);
        List<Product> GetSearchResult(string searchString);
        void Update(Product product, int[] categoryId);

    }
}
