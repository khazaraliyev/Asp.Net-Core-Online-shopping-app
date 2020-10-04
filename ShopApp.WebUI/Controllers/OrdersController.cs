using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Business.Abstract;
using ShopApp.WebUI.Identity;
using ShopApp.WebUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.WebUI.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderService orderService;
        private readonly UserManager<ApplicationUser> userManager;

        public OrdersController(IOrderService orderService, UserManager<ApplicationUser> userManager)
        {
            this.orderService = orderService;
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
            string userId = userManager.GetUserId(User);
            var orders = orderService.GetOrders(userId);

            var orderlistmodel = new List<OrderListViewModel>();
            OrderListViewModel orderListViewModel;

            foreach (var order in orders)
            {
                orderListViewModel = new OrderListViewModel()
                {
                    OrderId = order.Id,
                    OrderNumber = order.OrderNumber,
                    OrderDate = order.OrderDate,
                    Phone = order.Phone,
                    FirstName = order.FirstName,
                    LastName = order.LastName,
                    Email = order.Email,
                    Address = order.Address,
                    City = order.Address,
                    OrderState=order.OrderState,
                    PaymentType=order.PaymentType,
                    OrderItems = order.OrderItems.Select(o => new OrderItemModel()
                    {
                        OrderItemId=o.Id,
                        Name=o.Product.Name,
                        Price=(double)o.Price,
                        Quantity=o.Quantity,
                        ImageUrl=o.Product.ImageUrl
                    }).ToList()
                };
                orderlistmodel.Add(orderListViewModel);
            }
            return View(orderlistmodel);
        }
    }
}
