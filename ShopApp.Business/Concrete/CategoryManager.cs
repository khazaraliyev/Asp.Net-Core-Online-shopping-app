using ShopApp.Business.Abstract;
using ShopApp.DataAccess.Abstract;
using ShopApp.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopApp.Business.Concrete
{
    public class CategoryManager : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoryManager(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }
        public void Create(Category entity)
        {
            categoryRepository.Create(entity);
        }

        public void Delete(Category T)
        {
            categoryRepository.Delete(T);
        }

        public void DeleteFromCategory(int productId, int categoryId)
        {
            categoryRepository.DeleteFromCategory(productId, categoryId);
        }

        public List<Category> GetAll()
        {
            return categoryRepository.GetAll();
        }

        public Category GetById(int id)
        {
            return categoryRepository.GetById(id);
        }

        public Category GetByIdWithProducts(int categoryId)
        {
            return categoryRepository.GetByIdWithProducts(categoryId);
        }

        public void Update(Category entity)
        {
            categoryRepository.Update(entity);
        }
    }
}
