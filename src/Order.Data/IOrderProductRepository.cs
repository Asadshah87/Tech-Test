using Order.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Data
{
    public interface IOrderProductRepository
    {
        /// <summary>
        /// To get order product by product id
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns>Entity of order product</returns>
        Task<OrderProduct> GetOrderProductByIdAsync(Guid ProductId);
    }
}
