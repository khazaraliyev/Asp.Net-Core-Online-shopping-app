using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ShopApp.Business.Abstract;
using ShopApp.Entity;
using ShopApp.WebUI.Identity;
using ShopApp.WebUI.ViewModels;
using Stripe;
using static ShopApp.Entity.Order;

namespace ShopApp.WebUI.Controllers
{
    [Authorize]
    public class CardController : Controller
    {
        private readonly ICardService cardService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IOrderService orderService;

        public CardController(ICardService cardService, UserManager<ApplicationUser> userManager, IOrderService orderService)
        {
            this.cardService = cardService;
            this.userManager = userManager;
            this.orderService = orderService;
        }
        public IActionResult Index()
        {
            var card = cardService.GetCardByUserId(userManager.GetUserId(User));
            if (card == null)
            {
                return NotFound();
            }
            return View(new CardViewModel()
            {
                CardId = card.Id,
                CardItems = card.CardItems.Select(i => new CardItemViewModel()
                {
                    CradItemId = i.Id,
                    ProductId = i.ProductId,
                    Name = i.Product.Name,
                    Price = (decimal)i.Product.Price,
                    Image = i.Product.ImageUrl,
                    Quantity = i.Quantity
                }).ToList()
            });
        }

        [HttpPost]
        public IActionResult AddToCard(int productId, int quantity)
        {
            var userId = userManager.GetUserId(User);
            cardService.AddToCard(userId, productId, quantity);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteFromCard(int productId)
        {
            var userId = userManager.GetUserId(User);
            cardService.DeleteFromCard(userId, productId);
            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            var card = cardService.GetCardByUserId(userManager.GetUserId(User));
            var model = new OrderViewModel();
            model.CardModel = new CardViewModel()
            {
                CardId = card.Id,
                CardItems = card.CardItems.Select(i => new CardItemViewModel()
                {
                    CradItemId = i.Id,
                    ProductId = i.ProductId,
                    Name = i.Product.Name,
                    Price = (decimal)i.Product.Price,
                    Image = i.Product.ImageUrl,
                    Quantity = i.Quantity
                }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Checkout(OrderViewModel model, string stripeEmail, string stripeToken)
        {
            if (ModelState.IsValid)
            {
                var userId = userManager.GetUserId(User);
                var card = cardService.GetCardByUserId(userId);
                model.CardModel = new CardViewModel()
                {
                    CardId = card.Id,
                    CardItems = card.CardItems.Select(i => new CardItemViewModel()
                    {
                        CradItemId = i.Id,
                        ProductId = i.ProductId,
                        Name = i.Product.Name,
                        Price = (decimal)i.Product.Price,
                        Image = i.Product.ImageUrl,
                        Quantity = i.Quantity
                    }).ToList()
                };
                var payment = PaymentProcess(model, stripeEmail, stripeToken);
                if (payment.Status == "succeeded")
                {
                    SaveOrder(model, userId);
                    ClearCard(model.CardModel.CardId);
                    return View("Success");

                }
                else
                {
                    TempData["message"] = payment.FailureMessage;
                }
            }

            return View(model);

        }

        private void ClearCard(int cardId)
        {
            cardService.ClearCard(cardId);
        }

        private void SaveOrder(OrderViewModel model, string userId)
        {
            var order = new Entity.Order();
            order.OrderNumber = new Random().Next(111111, 999999).ToString();
            order.OrderState = ProductOrderState.completed;
            order.PaymentType = ProductPaymentTypes.CreditCard;
            //order.PaymentId = payment.PaymentId;
            //order.ConversationId = payment.ConversationId;
            order.OrderDate = new DateTime();
            order.FirstName = model.Firstname;
            order.LastName = model.Lastname;
            order.UserId = userId;
            order.Address = model.Address;
            order.Phone = model.Phone;
            order.Email = model.Email;
            order.City = model.City;
            order.Note = model.Note;
            order.OrderItems = new List<Entity.OrderItem>();
            foreach (var item in model.CardModel.CardItems)
            {
                var orderItem = new ShopApp.Entity.OrderItem()
                {
                    Price = (double)item.Price,
                    Quantity = item.Quantity,
                    ProductId = item.ProductId
                };
                order.OrderItems.Add(orderItem);
            }
            orderService.Create(order);

        }

        public Charge PaymentProcess(OrderViewModel model, string stripeEmail, string stripeToken)
        {
            var customers = new CustomerService();
            var charges = new ChargeService();

            var customer = customers.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                Source = stripeToken
            });

            var charge = new ChargeCreateOptions
            {
                Amount = (long)model.CardModel.TotalPrice(),
                Description = "default description",
                Currency = "USD",
                Customer = customer.Id,
                ReceiptEmail = stripeEmail
            };
            Charge createcharge = charges.Create(charge);
            return createcharge;

        }
    }
}
