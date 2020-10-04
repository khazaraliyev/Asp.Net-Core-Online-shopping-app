using ShopApp.Business.Abstract;
using ShopApp.DataAccess.Abstract;
using ShopApp.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopApp.Business.Concrete
{
    public class CardManager : ICardService
    {
        private readonly ICardRepository cardRepository;

        public CardManager(ICardRepository cardRepository)
        {
            this.cardRepository = cardRepository;
        }

        public void AddToCard(string userId, int productId, int quantity)
        {
            var card = GetCardByUserId(userId);
            if (card!=null)
            {
                var index = card.CardItems.FindIndex(p => p.ProductId == productId);
                if (index<0)
                {
                    card.CardItems.Add(new CardItem()
                    {
                        ProductId=productId,
                        Quantity=quantity,
                        CardId=card.Id
                    });
                }

                else
                {
                    card.CardItems[index].Quantity += quantity;
                }

                cardRepository.Update(card);
            }
        }

        public void ClearCard(int cardId)
        {
            cardRepository.ClearCard(cardId);
        }

        public void DeleteFromCard(string userId, int productId)
        {
            var card = GetCardByUserId(userId);
            if (card!=null)
            {
                cardRepository.DeleteFromCard(card.Id,productId);
            }
        }

        public Card GetCardByUserId(string userId)
        {
            return cardRepository.GetByUserId(userId);
        }

        public void InitializeCard(string userId)
        {
            cardRepository.Create(new Card() { UserId = userId });
        }
    }
}
