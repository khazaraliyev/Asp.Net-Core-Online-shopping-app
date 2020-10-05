using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Business.Abstract;
using ShopApp.DataAccess.Abstract;
using ShopApp.Entity;
using ShopApp.WebUI.ViewModels;

namespace ShopApp.WebUI.Controllers
{
    public class ShopController : Controller
    {
        private readonly IProductService productService;

        public ShopController(IProductService productService)
        {
            this.productService = productService;
        }
        public IActionResult List(string category, int page = 1)
        {
            const int pageSize = 6;
            return View(new ProductListViewModel()
            {
                Products = productService.GetProductsByCategory(category, page, pageSize).Where(p=>p.IsApproved).ToList(),
                PageInfo = new PageInfo()
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = productService.GetCountByCategory(category),
                    CurrentCategory = category
                }
            });
        }

        public IActionResult Details(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }

           Product product= productService.GetDetailsWithCategory((int)id);
            if (product==null)
            {
                return NotFound();
            }
            return View(new ProductDetailsViewModel
            {
                Product = product,
                Categories = product.ProductCategories.Select(p => p.Category).ToList()
            }); 
        }

        public IActionResult Search(string searchString)
        {
            var productViewModel = new ProductListViewModel()
            {
                Products = productService.GetSearchResult(searchString)
            };
            return View(productViewModel);
        }
    }
}
