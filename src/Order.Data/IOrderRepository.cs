using Order.Data.Entities;
using Order.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Order.Data
{
    public interface IOrderRepository
    {
        Task<IEnumerable<OrderSummary>> GetOrdersAsync();

        Task<OrderDetail> GetOrderByIdAsync(Guid orderId);
        /// <summary>
        /// Repository to insert the new order
        /// </summary>
        /// <param name="newOrder"></param>
        /// <returns>either true or false</returns>
        Task<bool?> CreateOrder(Data.Entities.Order newOrder);

    }
}
