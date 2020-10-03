using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShopApp.DataAccess.Abstract;
using ShopApp.WebUI.ViewModels;

namespace ShopApp.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductRepository productRepository;

        public HomeController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }
        public IActionResult Index()
        {
            var productViewModel = new ProductListViewModel()
            {
                Products = productRepository.GetHomePageProducts()
            };
            return View(productViewModel);
        }
    }
}
