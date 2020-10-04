using Microsoft.EntityFrameworkCore;
using ShopApp.DataAccess.Abstract;
using ShopApp.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShopApp.DataAccess.Concrete.SQL
{
    public class SQLCardRepository : SQLGenericRepository<Card, ShopContext>, ICardRepository
    {
        public void ClearCard(int cardId)
        {
            using (var context = new ShopContext())
            {
                var query = @"delete from CardItems where cardId=@p0";
                context.Database.ExecuteSqlRaw(query, cardId);
            }
        }

        public void DeleteFromCard(int cardId, int productId)
        {
            using (var context = new ShopContext())
            {
                var query = @"delete from CardItems where cardId=@p0 and productId=@p1";
                context.Database.ExecuteSqlRaw(query, cardId, productId);
            }
        }

        public Card GetByUserId(string userId)
        {
            using (var context = new ShopContext())
            {
                return context.Card
                    .Include(c => c.CardItems)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefault(p => p.UserId == userId);
            };
        }

        public override void Update(Card entity)
        {
            using (var context = new ShopContext())
            {
                context.Card.Update(entity);
                context.SaveChanges();
            }
        }
    }
}
