using Order.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Order.Service
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderSummary>> GetOrdersAsync();

        Task<OrderDetail> GetOrderByIdAsync(Guid orderId);

        /// <summary>
        /// To get order from status id 
        /// </summary>
        /// <param name="OrderStatusId"></param>
        /// <returns>List of orders with specified status</returns>
        Task<IEnumerable<OrderDetail>> GetOrdersByStatusIdAsync(Guid OrderStatusId);
        /// <summary>
        /// Update the Status of order
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderStatusID"></param>
        /// <returns>return either true or false  </returns>
        Task<bool?> UpdateOrderStatusAsync(Guid orderId, Guid orderStatusID);
        /// <summary>
        /// Create new order with order items
        /// </summary>
        /// <param name="newOrder">new order object</param>
        /// <returns>newly inserted Guid ID</returns>
        Task<Guid?> CreateOrder(NewOrder newOrder);
        /// <summary>
        /// To get profit of orders based on months
        /// </summary>
        /// <param name="month"></param>
        /// <returns>return profit from specific month from all years </returns>
        Task<IEnumerable<OrderMonthlyProfit>> GetProfitByMonth(int month);
        /// <summary>
        /// To get report of profit by years
        /// </summary>
        /// <param name="year"></param>
        /// <returns>return profit on year based monthly report</returns>
        Task<IEnumerable<OrderMonthlyProfit>> GetProfitForAllMonthsofYear(int year);


    }
}
