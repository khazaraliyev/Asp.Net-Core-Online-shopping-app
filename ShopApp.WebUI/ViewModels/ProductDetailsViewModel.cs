using ShopApp.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.WebUI.ViewModels
{
    public class ProductDetailsViewModel
    {
        public Product Product{ get; set; }
        public List<Category>Categories{ get; set; }
    }
}
