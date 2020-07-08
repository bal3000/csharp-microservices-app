using System.Threading.Tasks;
using Basket.API.Data.Interfaces;
using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Newtonsoft.Json;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IBasketContext _basketContext;

        public BasketRepository(IBasketContext basketContext)
        {
            _basketContext = basketContext;
        }

        public async Task<BasketCart> GetBasket(string username)
        {
            var basket = await _basketContext.Redis.StringGetAsync(username);

            if (basket.IsNullOrEmpty)
                return null;

            return JsonConvert.DeserializeObject<BasketCart>(basket);
        }

        public async Task<BasketCart> UpdateBasket(BasketCart basket)
        {
            var updated = await _basketContext.Redis
                .StringSetAsync(basket.UserName, JsonConvert.SerializeObject(basket));

            if (!updated)
                return null;

            return await GetBasket(basket.UserName);
        }

        public async Task<bool> DeleteBasket(string username) =>
            await _basketContext.Redis.KeyDeleteAsync(username);
    }
}