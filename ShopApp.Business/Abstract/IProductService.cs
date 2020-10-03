using ShopApp.DataAccess.Abstract;
using ShopApp.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopApp.Business.Abstract
{
    public interface IProductService
    {
        Product GetById(int id);
        Product GetByIdWithCategories(int id);
        Product GetDetailsWithCategory(int id);
        List<Product> GetProductsByCategory(string name,int page,int pageSize);
        List<Product> GetAll();
        void Create(Product entity);
        void Update(Product entity);
        List<Product> GetHomePageProducts();
        void Delete(Product T);
        int GetCountByCategory(string category);
        List<Product> GetSearchResult(string searchString);
        void Update(Product product, int[] categoryId);
    }
}
