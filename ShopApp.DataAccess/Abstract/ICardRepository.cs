using ShopApp.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopApp.DataAccess.Abstract
{
    public interface ICardRepository:IRepository<Card>
    {
        Card GetByUserId(string userId);
        void DeleteFromCard(int cardId, int productId);
        void ClearCard(int cardId);
    }
}
