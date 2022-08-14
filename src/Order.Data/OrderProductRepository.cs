using Order.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Data
{
    public class OrderProductRepository : IOrderProductRepository
    {
        private readonly OrderContext _orderContext;

        public OrderProductRepository(OrderContext orderContext)
        {
            _orderContext = orderContext;
        }
        public  async Task<OrderProduct> GetOrderProductByIdAsync(Guid ProductId)
        {
            var productIdBytes = ProductId.ToByteArray();
            return  _orderContext.OrderProduct.SingleOrDefault(x => x.Id == productIdBytes);
        }
    }
}
