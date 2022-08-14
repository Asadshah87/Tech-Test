using Order.Data;
using Order.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderProductRepository _orderProductRepository;
        private readonly IOrderStatusRepository _orderStatusRepository;

        public OrderService(IOrderRepository orderRepository, IOrderProductRepository orderProductRepository, IOrderStatusRepository orderStatusRepository)
        {
            _orderRepository = orderRepository;
            _orderProductRepository = orderProductRepository;
            _orderStatusRepository = orderStatusRepository;
        }

        public async Task<IEnumerable<OrderSummary>> GetOrdersAsync()
        {
            var orders = await _orderRepository.GetOrdersAsync();
            return orders;
        }

        public async Task<OrderDetail> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            return order;
        }

        public async Task<IEnumerable<OrderDetail>> GetOrdersByStatusIdAsync(Guid OrderStatusId)
        {
            var orderStatus = await _orderStatusRepository.GetOrdersByStatusIdAsync(OrderStatusId);
            if (orderStatus == null || orderStatus.Count() < 1)
                throw new KeyNotFoundException("Order status is not valid");

            return orderStatus;



        }


        public async Task<bool?> UpdateOrderStatusAsync(Guid orderId, Guid orderStatusID)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
                throw new KeyNotFoundException("Order id doesnt exist in application");
            var orderStatus = await _orderStatusRepository.GetOrdersByStatusIdAsync(orderStatusID);
            if (orderStatus == null || orderStatus.Count() < 1)
                throw new KeyNotFoundException("StatusID doesnot exist in the application");
            return await _orderStatusRepository.UpdateOrderStatusAsync(orderId, orderStatusID);

        }

        public async Task<Guid?> CreateOrder(NewOrder newOrder)
        {
            var orderId = Guid.NewGuid();

            Guid? orderStatus = await _orderStatusRepository.GetOrderStatusByName("Created");
            if (orderStatus == null)
                throw new KeyNotFoundException("Status Completed doesnot exist in the application");


            // convert model to entity
            var order = new Data.Entities.Order
            {
                Id = orderId.ToByteArray(),
                CustomerId = newOrder.CustomerId.Value.ToByteArray(),
                ResellerId = newOrder.ResellerId.Value.ToByteArray(),
                StatusId =  orderStatus.Value.ToByteArray(),
                CreatedDate = DateTime.UtcNow,
                Items = new List<Data.Entities.OrderItem>()

            };


            foreach (var item in newOrder.Items)
            {
                //validate product id
                var orderProduct = await _orderProductRepository.GetOrderProductByIdAsync(item.ProductId.Value);
                if (orderProduct == null)
                    throw new KeyNotFoundException("Product id is not valid");
                order.Items.Add(new Data.Entities.OrderItem
                {
                    Id = Guid.NewGuid().ToByteArray(),
                    ProductId = item.ProductId.Value.ToByteArray(),
                    ServiceId = orderProduct.ServiceId,
                    Quantity = item.Quantity
                });
            }


            var result = await _orderRepository.CreateOrder(order);
            if (result.HasValue)
                return orderId;
            return null;

        }

        public async Task<IEnumerable<OrderMonthlyProfit>> GetProfitByMonth(int month)
        {
            if (month < 1 || month > 12)
                throw new ArgumentOutOfRangeException("Month must be between 1 to 12");

            Guid? orderStatus = await _orderStatusRepository.GetOrderStatusByName("Completed");
            if (orderStatus == null)
                throw new KeyNotFoundException("Status Completed doesnot exist in the application");
            var orderDetail = await _orderStatusRepository.GetOrdersByStatusIdAsync(orderStatus.Value);
            var monthlyGrouped = orderDetail.Where(x => x.CreatedDate.Month == month).GroupBy(x => new { month = x.CreatedDate.Month, x.CreatedDate.Year })
                .Select(y => new OrderMonthlyProfit()
                {
                    Year = y.Key.Year,
                    Month = y.Key.month,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                    Profit = y.Select(z => z.Items.Select(c => c.TotalPrice - c.TotalCost).Sum()).Sum()

                });
            return monthlyGrouped;
        }

        public async Task<IEnumerable<OrderMonthlyProfit>> GetProfitForAllMonthsofYear(int year)
        {
            if (!Int32.TryParse(year.ToString(), out int result) || (year < 1 || year > 9999))
                throw new ArgumentOutOfRangeException("Year must be valid number");

            Guid? orderStatus = await _orderStatusRepository.GetOrderStatusByName("Completed");
            if (orderStatus == null)
                throw new KeyNotFoundException("Status Completed doesnot exist in the application");
            var orderDetail = await _orderStatusRepository.GetOrdersByStatusIdAsync(orderStatus.Value);

            var months = Enumerable.Range(1, 12)
                                .Select(x => new
                                {
                                    _year = year,
                                    month = x
                                });


            var data = months.GroupJoin(orderDetail,
                        m => new { month = m.month, year = m._year },
                        revision => new
                        {
                            month = revision.CreatedDate.Month,
                            year = revision.CreatedDate.Year
                        },
                        (p, g) => new OrderMonthlyProfit
                        {
                            Year = p._year,
                            Month = p.month,
                            MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(p.month),
                            Profit = g.Select(z => z.Items.Select(c => c.TotalPrice - c.TotalCost).Sum()).Sum()



                        });
            return data;
        }
    }
}

