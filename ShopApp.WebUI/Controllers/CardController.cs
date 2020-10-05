using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ShopApp.Business.Abstract;
using ShopApp.Entity;
using ShopApp.WebUI.Identity;
using ShopApp.WebUI.ViewModels;
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
                    Price = (double)i.Product.Price,
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
                    Price = (double)i.Product.Price,
                    Image = i.Product.ImageUrl,
                    Quantity = i.Quantity
                }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Checkout(OrderViewModel model)
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
                        Price = (double)i.Product.Price,
                        Image = i.Product.ImageUrl,
                        Quantity = i.Quantity
                    }).ToList()
                };
                var payment = PaymentProcess(model);
                if (payment.Status == "success")
                {
                    SaveOrder(model, payment, userId);
                    ClearCard(model.CardModel.CardId);
                    Console.WriteLine(payment.ErrorMessage);
                    return View("Success");

                }
                else
                {
                    TempData["message"] = payment.ErrorMessage;
                }
            }

            return View(model);

        }

        private void ClearCard(int cardId)
        {
            cardService.ClearCard(cardId);
        }

        private void SaveOrder(OrderViewModel model, Payment payment, string userId)
        {
            var order = new Order();
            order.OrderNumber = new Random().Next(111111, 999999).ToString();
            order.OrderState = ProductOrderState.completed;
            order.PaymentType = ProductPaymentTypes.CreditCard;
            order.PaymentId = payment.PaymentId;
            order.ConversationId = payment.ConversationId;
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
                    Price = item.Price,
                    Quantity = item.Quantity,
                    ProductId = item.ProductId
                };
                order.OrderItems = new List<Entity.OrderItem>();
                order.OrderItems.Add(orderItem);
            }
            orderService.Create(order);

        }

        public Payment PaymentProcess(OrderViewModel model)
        {

            Options options = new Options();
            options.ApiKey = "sandbox-lqG2J1PXtO7WRuLw8Mex8AVhZIBSE2GD";
            options.SecretKey = "sandbox-22dyNh0SS8XllPk2pWE6j9WPxOcDAB0u";
            options.BaseUrl = "https://sandbox-api.iyzipay.com";


            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.EN.ToString();
            request.ConversationId = new Random().Next(111111111, 999999999).ToString();
            request.Price = model.CardModel.TotalPrice().ToString();
            request.PaidPrice = model.CardModel.TotalPrice().ToString();
            request.Currency = Currency.USD.ToString();
            request.Installment = 1;
            request.BasketId = "B67832";
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();


            PaymentCard paymentCard = new PaymentCard();

            paymentCard.CardHolderName = model.CardName;
            paymentCard.CardNumber = model.CardNumber;
            paymentCard.ExpireMonth = model.ExpirationMonth;
            paymentCard.ExpireYear = model.ExpirationYear;
            paymentCard.Cvc = model.Cvv;
            paymentCard.RegisterCard = 0;
            request.PaymentCard = paymentCard;
            

            //paymentCard.CardNumber = "5528790000000008";
            //paymentCard.ExpireMonth = "12";
            //paymentCard.ExpireYear = "2030";
            //paymentCard.Cvc = "123";

            Buyer buyer = new Buyer();
            buyer.Id = "BY789";
            buyer.Name = model.Firstname;
            buyer.Surname = model.Lastname;
            buyer.GsmNumber = "+905350000000";
            buyer.Email = "email@email.com";
            buyer.IdentityNumber = "74300864791";
            buyer.LastLoginDate = "2015-10-05 12:43:35";
            buyer.RegistrationDate = "2013-04-21 15:12:09";
            buyer.RegistrationAddress = "28 may, Bulbul pr. N:1";
            buyer.Ip = "85.34.78.112";
            buyer.City = "Baku";
            buyer.Country = "Azerbaijan";
            buyer.ZipCode = "34732";
            request.Buyer = buyer;
         

            Address shippingAddress = new Address();
            shippingAddress.ContactName = "Jane Doe";
            shippingAddress.City = "Baku";
            shippingAddress.Country = "Azerbaijan";
            shippingAddress.Description = "28 may, Bulbul pr. N:1";
            shippingAddress.ZipCode = "1005";
            request.ShippingAddress = shippingAddress;
            

            Address billingAddress = new Address();
            billingAddress.ContactName = "Jane Doe";
            billingAddress.City = "Baku";
            billingAddress.Country = "Azerbaijan";
            billingAddress.Description = "28 may, Bulbul pr. N:1";
            billingAddress.ZipCode = "1005";
            request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();

            BasketItem basketItem;
            foreach (var item in model.CardModel.CardItems)
            {
                basketItem = new BasketItem();
                basketItem.Id = item.ProductId.ToString();
                basketItem.Name = item.Name;
                basketItem.Category1 = "Phone";
                basketItem.Price = item.Price.ToString();
                basketItem.ItemType = BasketItemType.PHYSICAL.ToString();
                basketItems.Add(basketItem);
            }
            request.BasketItems = basketItems;
            return Payment.Create(request, options);
        }
    }
}
