using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public class CustomerBasket
    {
        public CustomerBasket()
        {
        }

        public CustomerBasket(string id)
        {
            Id = id;
        }

        public string Id { get; set; } // ÇáÜ Id åäÇ string ÚÔÇä åíÈŞì generated client-side
        public List<BasketItem> Items { get; set; } = new List<BasketItem>();
    }
}