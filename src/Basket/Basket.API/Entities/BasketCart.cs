using System.Linq;
using System.Collections.Generic;

namespace Basket.API.Entities
{
    public class BasketCart
    {
        public string UserName { get; set; }
        public IEnumerable<BasketCartItem> Items { get; set; } = new List<BasketCartItem>();

        public BasketCart() { }

        public BasketCart(string username)
        {
            UserName = username;
        }

        public decimal TotalPrice
        {
            get => Items.Sum(i => (i.Price * i.Quantity));
        }
    }
}