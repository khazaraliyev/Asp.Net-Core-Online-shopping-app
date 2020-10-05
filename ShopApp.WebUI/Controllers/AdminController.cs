using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Business.Abstract;
using ShopApp.Entity;
using ShopApp.WebUI.ViewModels;

namespace ShopApp.WebUI.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly IProductService productService;
        private readonly ICategoryService categoryService;

        public AdminController(IProductService productService, ICategoryService categoryService)
        {
            this.productService = productService;
            this.categoryService = categoryService;
        }

        public IActionResult ProductList()
        {
            return View(new ProductListViewModel
            {
                Products = productService.GetAll()
            });
        }

        public IActionResult CategoryList()
        {
            return View(new CategoryListViewModel
            {
                Categories = categoryService.GetAll()
            });
        }

        [HttpGet]
        public IActionResult CreateProduct()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductViewModel productViewModel,IFormFile file)
        {
            var entity = new Product()
            {
                Name = productViewModel.Name,
                Price = productViewModel.Price,
                Description = productViewModel.Description,
            };

            if (file != null)
            {
                entity.ImageUrl = productViewModel.ImageUrl;
                var extension = Path.GetExtension(file.FileName);
                var fileName = string.Format($"{Guid.NewGuid()}{extension}");
                entity.ImageUrl = fileName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            productService.Create(entity);
            TempData["message"] = $"Product added:{entity.Name}";
            return RedirectToAction("ProductList");
        }


        [HttpPost]
        public IActionResult CreateCategory(CategoryViewModel categoryViewModel)
        {
            var category = new Category()
            {
                Name = categoryViewModel.Name,
            };
            categoryService.Create(category);
            TempData["message"] = $"Category added:{category.Name}";
            return RedirectToAction("CategoryList");
        }

        [HttpGet]
        public IActionResult EditProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = productService.GetByIdWithCategories((int)id);
            if (product == null)
            {
                return NotFound();
            }

            var model = new ProductViewModel()
            {
                ProductId = product.ProductId,
                Name = product.Name,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                Description = product.Description,
                IsApproved=product.IsApproved,
                IsHome=product.IsHome,
                SelectedCategories = product.ProductCategories.Select(p => p.Category).ToList(),
                Categories = categoryService.GetAll()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductViewModel productViewModel, int[] categoryId, IFormFile file)
        {
            var product = productService.GetById(productViewModel.ProductId);
            if (product == null)
            {
                return NotFound();
            }

            product.Name = productViewModel.Name;
            product.Description = productViewModel.Description;
            product.Price = productViewModel.Price;
            product.IsHome = productViewModel.IsHome;
            product.IsApproved = productViewModel.IsApproved;

            if (file != null)
            {
                product.ImageUrl = productViewModel.ImageUrl;
                var extension = Path.GetExtension(file.FileName);
                var fileName = string.Format($"{Guid.NewGuid()}{extension}");
                product.ImageUrl = fileName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            productService.Update(product, categoryId);
            TempData["message"] = $"Product updated:{product.Name}";
            return RedirectToAction("ProductList");
        }

        [HttpGet]
        public IActionResult EditCategory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = categoryService.GetByIdWithProducts((int)id);
            if (category == null)
            {
                return NotFound();
            }

            var model = new CategoryViewModel()
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Products = category.ProductCategories.Select(p => p.Product).ToList()
            };
            return View(model);
        }

   


        [HttpPost]
        public IActionResult EditCategory(CategoryViewModel categoryViewModel)
        {
            var category = categoryService.GetById(categoryViewModel.CategoryId);
            if (category == null)
            {
                return NotFound();
            }

            category.Name = categoryViewModel.Name;
            categoryService.Update(category);
            TempData["message"] = $"Category updated:{category.Name}";
            return RedirectToAction("CategoryList");
        }

        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            var product = productService.GetById(id);
            if (product != null)
            {
                productService.Delete(product);
            }
            TempData["message"] = $"Product deleted:{product.Name}";
            return RedirectToAction("ProductList");
        }

        [HttpPost]
        public IActionResult DeleteCategory(int id)
        {
            var category = categoryService.GetById(id);
            if (category != null)
            {
                categoryService.Delete(category);
            }
            TempData["message"] = $"Category deleted:{category.Name}";
            return RedirectToAction("CategoryList");
        }

        [HttpPost]
        public IActionResult DeleteFromCategory(int productId, int categoryId)
        {
            categoryService.DeleteFromCategory(productId, categoryId);
            return RedirectToAction("CategoryList");
        }
    }
}
