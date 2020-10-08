using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.WebUI.ViewModels
{
    public class CardViewModel
    {
        public int CardId { get; set; }
        public List<CardItemViewModel> CardItems { get; set; }

        public decimal TotalPrice()
        {
            return CardItems.Sum(i => i.Price * i.Quantity);
        }
    }

    public class CardItemViewModel
    {
        public int CradItemId { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public int Quantity { get; set; }
    }
}
