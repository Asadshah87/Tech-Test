using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NUnit.Framework;
using Order.Data;
using Order.Data.Entities;
using Order.Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Service.Tests
{
    public class OrderServiceTests
    {
        private IOrderService _orderService;
        private IOrderRepository _orderRepository;
        private OrderContext _orderContext;
        private DbConnection _connection;
        private IOrderProductRepository _orderProductRepository;
        private IOrderStatusRepository _orderStatusRepository;

        private readonly byte[] _orderStatusCreatedId = Guid.NewGuid().ToByteArray();
        private readonly byte[] _orderServiceEmailId = Guid.NewGuid().ToByteArray();
        private readonly byte[] _orderProductEmailId = Guid.NewGuid().ToByteArray();
        private readonly byte[] _orderStatusInProgressId = Guid.NewGuid().ToByteArray();
        private readonly byte[] _orderStatusCompletedId = Guid.NewGuid().ToByteArray();


        [SetUp]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<OrderContext>()
                .UseSqlite(CreateInMemoryDatabase())
                .EnableDetailedErrors(true)
                .EnableSensitiveDataLogging(true)
                .Options;

            _connection = RelationalOptionsExtension.Extract(options).Connection;

            _orderContext = new OrderContext(options);
            _orderContext.Database.EnsureDeleted();
            _orderContext.Database.EnsureCreated();

            _orderRepository = new OrderRepository(_orderContext);
            _orderProductRepository = new OrderProductRepository(_orderContext);
            _orderStatusRepository = new OrderStatusRepository(_orderContext);
            _orderService = new OrderService(_orderRepository, _orderProductRepository, _orderStatusRepository);

            await AddReferenceDataAsync(_orderContext);
        }

        [TearDown]
        public void TearDown()
        {
            _connection.Dispose();
            _orderContext.Dispose();
        }


        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            return connection;
        }

        [Test]
        public async Task GetOrdersAsync_ReturnsCorrectNumberOfOrders()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            await AddOrder(orderId1, 1);

            var orderId2 = Guid.NewGuid();
            await AddOrder(orderId2, 2);

            var orderId3 = Guid.NewGuid();
            await AddOrder(orderId3, 3);

            // Act
            var orders = await _orderService.GetOrdersAsync();

            // Assert
            Assert.AreEqual(3, orders.Count());
        }

        [Test]
        public async Task GetOrdersAsync_ReturnsOrdersWithCorrectTotals()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            await AddOrder(orderId1, 1);

            var orderId2 = Guid.NewGuid();
            await AddOrder(orderId2, 2);

            var orderId3 = Guid.NewGuid();
            await AddOrder(orderId3, 3);

            // Act
            var orders = await _orderService.GetOrdersAsync();

            // Assert
            var order1 = orders.SingleOrDefault(x => x.Id == orderId1);
            var order2 = orders.SingleOrDefault(x => x.Id == orderId2);
            var order3 = orders.SingleOrDefault(x => x.Id == orderId3);

            Assert.AreEqual(0.8m, order1.TotalCost);
            Assert.AreEqual(0.9m, order1.TotalPrice);

            Assert.AreEqual(1.6m, order2.TotalCost);
            Assert.AreEqual(1.8m, order2.TotalPrice);

            Assert.AreEqual(2.4m, order3.TotalCost);
            Assert.AreEqual(2.7m, order3.TotalPrice);
        }

        [Test]
        public async Task GetOrderByIdAsync_ReturnsCorrectOrder()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            await AddOrder(orderId1, 1);

            // Act
            var order = await _orderService.GetOrderByIdAsync(orderId1);

            // Assert
            Assert.AreEqual(orderId1, order.Id);
        }

        [Test]
        public async Task GetOrderByIdAsync_ReturnsCorrectOrderItemCount()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            await AddOrder(orderId1, 1);

            // Act
            var order = await _orderService.GetOrderByIdAsync(orderId1);

            // Assert
            Assert.AreEqual(1, order.Items.Count());
        }

        [Test]
        public async Task GetOrderByIdAsync_ReturnsOrderWithCorrectTotals()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            await AddOrder(orderId1, 2);

            // Act
            var order = await _orderService.GetOrderByIdAsync(orderId1);

            // Assert
            Assert.AreEqual(1.6m, order.TotalCost);
            Assert.AreEqual(1.8m, order.TotalPrice);
        }

        [Test]
        public async Task GetOrderStatusByIdAsync_ReturnsCorrectStatus()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            await AddOrder(orderId1, 1);

            // Act
            var orderDetail = await _orderService.GetOrdersByStatusIdAsync(new Guid(_orderStatusCreatedId));

            // Assert
            Assert.AreEqual(_orderStatusCreatedId, orderDetail.Select(x => x.StatusId).First().ToByteArray());
        }
        [Test]
        public async Task GetOrderProductByIdAsync_ReturnsCorrectStatus()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            await AddOrder(orderId1, 1);

            // Act
            var productDetail = await _orderProductRepository.GetOrderProductByIdAsync(new Guid(_orderProductEmailId));

            // Assert
            Assert.AreEqual(_orderProductEmailId, productDetail.Id);
        }

        [Test]
        public async Task GetOrderStatusByIdAsync_ReturnsInvalidStatus()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            await AddOrder(orderId1, 1);

            //Act

            // Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await _orderService.GetOrdersByStatusIdAsync(orderId1);
            });
        }

        [Test]
        public async Task GetProfitByMonthAsync_ReturnsInvalidMonth()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            await AddOrder(orderId1, 1);

            // Act

            // Assert
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            {
                await _orderService.GetProfitByMonth(0);
            });

        }

        [Test]
        public async Task GetProfitByMonthAsync_ReturnsSuccess()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            await AddOrder(orderId1, 1, true);

            // Act
            var orderDetail = await _orderService.GetProfitByMonth(8);

            // Assert
            Assert.AreEqual(0.1, orderDetail.Select(x => x.Profit).SingleOrDefault());

        }
        [Test]
        public async Task GetProfitForAllMonthsofYear_ReturnsInvalidYear()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            await AddOrder(orderId1, 1);

            // Act

            // Assert
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            {
                await _orderService.GetProfitForAllMonthsofYear(-1);
            });

        }

        [Test]
        public async Task UpdateOrderStatusAsync_ReturnsSuccess()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            await AddOrder(orderId1, 1);
            var orderId2 = Guid.NewGuid();
            await AddOrder(orderId2, 1, true);

            // Act
            var orderDetail = await _orderService.UpdateOrderStatusAsync(orderId1, new Guid(_orderStatusCompletedId));

            // Assert
            Assert.AreEqual(true, orderDetail);
        }
        [Test]
        public async Task UpdateOrderStatusAsync_ReturnsInvalidOrderID()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            await AddOrder(orderId1, 1);

            // Act

            // Assert

            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await _orderService.UpdateOrderStatusAsync(new Guid(_orderStatusInProgressId), new Guid(_orderStatusInProgressId));
            });
        }
        [Test]
        public async Task UpdateOrderStatusAsync_ReturnsInvalidStatusID()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            await AddOrder(orderId1, 1);

            // Act

            // Assert

            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await _orderService.UpdateOrderStatusAsync(orderId1, orderId1);
            });
        }
        [Test]
        public async Task CreateOrderAsync_ReturnsSuccess()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            await AddOrder(orderId1, 1);
            NewOrder orderDetail = new NewOrder()
            {
                ResellerId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                StatusId = new Guid(_orderStatusCreatedId),
                Items = new List<NewOrderItem>() { new NewOrderItem() {
                ProductId =new Guid(_orderProductEmailId),
                Quantity = 1,

                } }
            };

            // Act
            var id = await _orderService.CreateOrder(orderDetail);

            // Assert
            Assert.AreEqual(true, Guid.TryParse(id.Value.ToString(), out Guid result));

        }

        private async Task AddOrder(Guid orderId, int quantity, bool isCompleted = false)
        {
            var orderIdBytes = orderId.ToByteArray();
            _orderContext.Order.Add(new Data.Entities.Order
            {
                Id = orderIdBytes,
                ResellerId = Guid.NewGuid().ToByteArray(),
                CustomerId = Guid.NewGuid().ToByteArray(),
                CreatedDate = DateTime.Now,
                StatusId = !isCompleted ? _orderStatusCreatedId : _orderStatusCompletedId,
            });

            _orderContext.OrderItem.Add(new Data.Entities.OrderItem
            {
                Id = Guid.NewGuid().ToByteArray(),
                OrderId = orderIdBytes,
                ServiceId = _orderServiceEmailId,
                ProductId = _orderProductEmailId,
                Quantity = quantity
            });

            await _orderContext.SaveChangesAsync();
        }


        private async Task AddReferenceDataAsync(OrderContext orderContext)
        {
            orderContext.OrderStatus.Add(new OrderStatus
            {
                Id = _orderStatusCreatedId,
                Name = "Created",
            });

            orderContext.OrderStatus.Add(new OrderStatus
            {
                Id = _orderStatusInProgressId,
                Name = "In Progress",
            });
            orderContext.OrderStatus.Add(new OrderStatus
            {
                Id = _orderStatusCompletedId,
                Name = "Completed",
            });


            orderContext.OrderService.Add(new Data.Entities.OrderService
            {
                Id = _orderServiceEmailId,
                Name = "Email"
            });

            orderContext.OrderProduct.Add(new OrderProduct
            {
                Id = _orderProductEmailId,
                Name = "100GB Mailbox",
                UnitCost = 0.8m,
                UnitPrice = 0.9m,
                ServiceId = _orderServiceEmailId
            });

            await orderContext.SaveChangesAsync();
        }
    }
}
