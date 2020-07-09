using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;
using Ordering.Infrastructure.Data;

namespace Ordering.Infrastructure.Repositories.Base
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(OrderContext dbContext) : base(dbContext) { }
        public async Task<IEnumerable<Order>> GetOrderByUserName(string username)
        {
            var orderList = await _dbContext.Orders.Where(x => x.UserName == username).ToListAsync();
            return orderList;
        }
    }
}