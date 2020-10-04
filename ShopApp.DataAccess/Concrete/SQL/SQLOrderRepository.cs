using Microsoft.EntityFrameworkCore;
using ShopApp.DataAccess.Abstract;
using ShopApp.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShopApp.DataAccess.Concrete.SQL
{
    public class SQLOrderRepository : SQLGenericRepository<Order, ShopContext>, IOrderRepository
    {
        public List<Order> GetOrders(string userId)
        {
            using (var context = new ShopContext())
            {
                var orders = context.Orders
                                    .Include(o => o.OrderItems)
                                    .ThenInclude(o => o.Product)
                                    .AsQueryable();
                if (!string.IsNullOrEmpty(userId))
                {
                    orders = orders.Where(o => o.UserId == userId);
                }
                return orders.ToList();
            };
        }
    }
}
