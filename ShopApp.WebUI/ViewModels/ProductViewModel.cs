using ShopApp.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.WebUI.ViewModels
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage ="Name field is required")]
        [MaxLength(50,ErrorMessage ="Name field can take max 50 characters")]
        public string Name { get; set; }
        [Required(ErrorMessage ="Price field is required")]
        public double? Price { get; set; }
        [Required(ErrorMessage ="Description field is required")]
        [MaxLength(500,ErrorMessage ="Description field can take max 500 characters")]
        [MinLength(10,ErrorMessage ="Minimum 10 characters is required")]
        public string Description { get; set; }
        [Required(ErrorMessage ="Image is required")]
        [MaxLength(50,ErrorMessage ="Image name can be max 50 characters length")]
        public string ImageUrl { get; set; }
        public bool  IsApproved { get; set; }
        public bool IsHome { get; set; }
        public List<Category> SelectedCategories { get; set; }
        public List<Category> Categories { get; set; }
    }
}
