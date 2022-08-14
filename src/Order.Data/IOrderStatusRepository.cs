using Order.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Data
{
    public interface IOrderStatusRepository
    {
        /// <summary>
        /// to get order status by ID
        /// </summary>
        /// <param name="StatusId"></param>
        /// <returns>List of order details</returns>
        Task<IEnumerable<OrderDetail>> GetOrdersByStatusIdAsync(Guid StatusId);
        /// <summary>
        ///  To update order status
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderStatusID"></param>
        /// <returns>Status is successfull or not (true/false)</returns>
        Task<bool?> UpdateOrderStatusAsync(Guid orderId, Guid orderStatusID);
        /// <summary>
        /// To get the status id by passing status name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Status id guid</returns>
        Task<Guid?> GetOrderStatusByName(string name);
    }
}
