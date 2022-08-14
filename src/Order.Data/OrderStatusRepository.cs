using Microsoft.EntityFrameworkCore;
using Order.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Data
{
    public class OrderStatusRepository : IOrderStatusRepository
    {
        private readonly OrderContext _orderContext;

        public OrderStatusRepository(OrderContext orderContext)
        {
            _orderContext = orderContext;
        }

        public async Task<IEnumerable<OrderDetail>> GetOrdersByStatusIdAsync(Guid StatusId)
        {
            var statusIdBytes = StatusId.ToByteArray();
            var order = await _orderContext.Order.AsNoTracking()
                .Where(x => _orderContext.Database.IsInMemory() ? x.StatusId.SequenceEqual(statusIdBytes) : x.StatusId == statusIdBytes)
                .Select(x => new OrderDetail
                {
                    Id = new Guid(x.Id),
                    ResellerId = new Guid(x.ResellerId),
                    CustomerId = new Guid(x.CustomerId),
                    StatusId = new Guid(x.StatusId),
                    StatusName = x.Status.Name,
                    CreatedDate = x.CreatedDate,
                    TotalCost = x.Items.Sum(i => i.Quantity * i.Product.UnitCost).Value,
                    TotalPrice = x.Items.Sum(i => i.Quantity * i.Product.UnitPrice).Value,
                    Items = x.Items.Select(i => new Model.OrderItem
                    {
                        Id = new Guid(i.Id),
                        OrderId = new Guid(i.OrderId),
                        ServiceId = new Guid(i.ServiceId),
                        ServiceName = i.Service.Name,
                        ProductId = new Guid(i.ProductId),
                        ProductName = i.Product.Name,
                        UnitCost = i.Product.UnitCost,
                        UnitPrice = i.Product.UnitPrice,
                        TotalCost = i.Product.UnitCost * i.Quantity.Value,
                        TotalPrice = i.Product.UnitPrice * i.Quantity.Value,
                        Quantity = i.Quantity.Value,



                    }),

                }).OrderByDescending(x => x.CreatedDate).ToListAsync();

            return order;
        }

        public async Task<bool?> UpdateOrderStatusAsync(Guid orderId, Guid orderStatusID)
        {
            var orderIdBytes = orderId.ToByteArray();

            var order = await _orderContext.Order.SingleOrDefaultAsync(x => _orderContext.Database.IsInMemory() ? x.Id.SequenceEqual(orderIdBytes) : x.Id == orderIdBytes);
            if (order == null)
                return null;

            order.StatusId = orderStatusID.ToByteArray();
            var result = await _orderContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<Guid?> GetOrderStatusByName(string name)
        {
            var orderDetail = await _orderContext.OrderStatus
                 .FirstOrDefaultAsync(x => x.Name == name);
            if (orderDetail == null) return null;
            var result = new Guid(orderDetail.Id);
            return result;
        }
    }
}
