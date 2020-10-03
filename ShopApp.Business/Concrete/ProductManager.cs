using ShopApp.Business.Abstract;
using ShopApp.DataAccess.Abstract;
using ShopApp.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShopApp.Business.Concrete
{
    public class ProductManager : IProductService
    {
        private readonly IProductRepository productRepository;

        public ProductManager(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }
        public void Create(Product entity)
        {
            productRepository.Create(entity);
        }

        public void Delete(Product T)
        {
            productRepository.Delete(T);
        }

        public List<Product> GetAll()
        {
            return productRepository.GetAll();
        }

        public Product GetById(int id)
        {
            return productRepository.GetById(id);
        }

        public Product GetByIdWithCategories(int id)
        {
            return productRepository.GetByIdWithCategories(id);
        }

        public int GetCountByCategory(string category)
        {
            return productRepository.GetCountByCategory(category);
        }

        public Product GetDetailsWithCategory(int id)
        {
            return productRepository.GetDetailsWithCategory(id);
        }

        public List<Product> GetHomePageProducts()
        {
            return productRepository.GetHomePageProducts();
        }

        public List<Product> GetProductsByCategory(string name, int page, int pageSize)
        {
            return productRepository.GetProductsByCategory(name, page, pageSize);
        }

        public List<Product> GetSearchResult(string searchString)
        {
            return productRepository.GetSearchResult(searchString);
        }

        public void Update(Product entity)
        {
            productRepository.Update(entity);
        }

        public void Update(Product product, int[] categoryId)
        {
            productRepository.Update(product, categoryId);
        }
    }
}
