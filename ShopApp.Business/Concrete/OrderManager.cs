using ShopApp.Business.Abstract;
using ShopApp.DataAccess.Abstract;
using ShopApp.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopApp.Business.Concrete
{
    public class OrderManager : IOrderService
    {
        private readonly IOrderRepository orderRepository;

        public OrderManager(IOrderRepository orderRepository)
        {
            this.orderRepository = orderRepository;
        }
        public void Create(Order entity)
        {
            orderRepository.Create(entity);
        }

        public List<Order> GetOrders(string userId)
        {
           return orderRepository.GetOrders(userId);
        }
    }
}
