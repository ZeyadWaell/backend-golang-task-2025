using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using EasyOrder.Application.Contracts.DTOs;
using EasyOrder.Application.Contracts.DTOs.Responses.Global;
using EasyOrder.Application.Contracts.Filters;
using EasyOrder.Application.Contracts.Interfaces.InternalServices;
using EasyOrder.Application.Contracts.Interfaces.Main;
using EasyOrder.Application.Contracts.Interfaces.Repository;
using EasyOrder.Application.Queries.Services;
using EasyOrder.Domain.Entities;
using EasyOrder.Domain.Enums;
using Hangfire;
using Moq;
using Xunit;


namespace EasyOrder.Application.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IOrderRepository> _orderRepoMock;
        private readonly Mock<ICurrentUserService> _userServiceMock;
        private readonly Mock<IBackgroundJobClient> _jobsMock;
        private readonly IMapper _mapper;
        private readonly OrderService _service;

        public OrderServiceTests()
        {
            // 1) Mock the UoW and its OrdersRepository
            _uowMock = new Mock<IUnitOfWork>();
            _orderRepoMock = new Mock<IOrderRepository>();
            _uowMock.Setup(u => u.OrdersRepository)
                    .Returns(_orderRepoMock.Object);

            // 2) Mock CurrentUserService
            _userServiceMock = new Mock<ICurrentUserService>();
            _userServiceMock.SetupGet(x => x.UserId).Returns("user-123");

            // 3) Mock Hangfire jobs
            _jobsMock = new Mock<IBackgroundJobClient>();

            // 4) Configure AutoMapper with only the needed maps
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Order, OrderListDto>();
                cfg.CreateMap<Order, OrderDetailsDto>();
                cfg.CreateMap<Order, OrderStatusDto>();
                cfg.CreateMap<CreateOrderDto, Order>();
                cfg.CreateMap<CreateOrderItemDto, OrderItem>();
                cfg.CreateMap<CreatePaymentDto, Payment>();
            });
            _mapper = config.CreateMapper();

            // 5) Instantiate the real service
            _service = new OrderService(
                _uowMock.Object,
                _userServiceMock.Object,
                _mapper,
                inventoryChecker: Mock.Of<Contracts.Interfaces.GrpcServices.IInventoryChecker>(),
                bus: Mock.Of<MassTransit.IBus>(),
                jobs: _jobsMock.Object
            );
        }

        [Fact]
        public async Task CreateOrderAsync_NoItems_ReturnsBadRequest()
        {
            // Arrange
            var dto = new CreateOrderDto
            {
                Currency = Currency.USD,
                Items = new List<CreateOrderItemDto>()
            };

            // Act
            var response = await _service.CreateOrderAsync(dto);

            // Assert
            Assert.IsType<ErrorResponse>(response);
            var err = (ErrorResponse)response;
            // Look at the full Message, not the first char
            Assert.Contains(
                "at least one order item",
                err.Message,
                StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CreateOrderAsync_ReservationFails_ReturnsBadRequest()
        {
            // Arrange
            var dto = new CreateOrderDto
            {
                Currency = Currency.USD,
                Items = new List<CreateOrderItemDto> {
            new CreateOrderItemDto { ProductItemId = 42, Quantity = 5 }
        },
                Payment = new CreatePaymentDto { Method = PaymentMethod.Cash }
            };
            // Simulate inventory reservation failure
            var invMock = new Mock<Contracts.Interfaces.GrpcServices.IInventoryChecker>();
            invMock.Setup(x => x.ReserveAsync(42, 5)).ReturnsAsync(false);

            // Inject the failing inventory checker
            typeof(OrderService)
                .GetField("_inventoryChecker", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(_service, invMock.Object);

            // Act
            var response = await _service.CreateOrderAsync(dto);

            // Assert
            Assert.IsType<ErrorResponse>(response);
            var err = (ErrorResponse)response;
            // Check the full Message, not just its first character
            Assert.Contains(
                "Failed to reserve inventory",
                err.Message,
                StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CreateOrderAsync_Valid_SucceedsAndEnqueuesJob()
        {
            // Arrange
            var dto = new CreateOrderDto
            {
                Currency = Currency.USD,
                Items = new List<CreateOrderItemDto> {
                    new CreateOrderItemDto { ProductItemId = 1, Quantity = 2 }
                },
                Payment = new CreatePaymentDto { Method = PaymentMethod.Cash }
            };
            // Simulate reservation success
            var invMock = Mock.Get(Mock.Of<Contracts.Interfaces.GrpcServices.IInventoryChecker>());
            invMock.Setup(x => x.ReserveAsync(1, 2)).ReturnsAsync(true);
            typeof(OrderService)
                .GetField("_inventoryChecker", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(_service, invMock.Object);

            // Capture the added Order
            Order added = null;
            _orderRepoMock
                .Setup(r => r.AddAsync(
                    It.IsAny<Order>(),
                    It.IsAny<CancellationToken>()        // explicitly match ANY cancellation token
                ))
                .Callback<Order, CancellationToken>((o, ct) => added = o)
                .Returns(Task.CompletedTask);

            // 2) Setup SaveChangesAsync likewise
            _uowMock
                .Setup(u => u.SaveChangesAsync(
                    It.IsAny<CancellationToken>()        // explicitly match ANY cancellation token
                ))
                .ReturnsAsync(1);

            // Act
            // Act
            var response = await _service.CreateOrderAsync(dto);

            // Assert
            Assert.IsType<SuccessResponse<object>>(response);
            Assert.NotNull(added);

            // Verify that Hangfire was asked to create a job (via the Create method)
            _jobsMock.Verify(
                j => j.Create(
                    It.IsAny<Hangfire.Common.Job>(),
                    It.IsAny<Hangfire.States.IState>()),
                Times.Once);

        }

        [Fact]
        public async Task GetOrderByIdAsync_NotFound_ReturnsNotFound()
        {
            _orderRepoMock
                .Setup(r => r.GetIncludingAsync(
                    It.IsAny<Expression<Func<Order, bool>>>(),
                    It.IsAny<Expression<Func<Order, object>>[]>()))
                .ReturnsAsync((Order)null);

            var response = await _service.GetOrderByIdAsync(42);

            Assert.IsType<ErrorResponse>(response);
            var err = (ErrorResponse)response;
            // Look at the full Message
            Assert.Contains(
                "Order not found",
                err.Message,
                StringComparison.OrdinalIgnoreCase);
        }


        [Fact]
        public async Task GetOrderByIdAsync_Exists_ReturnsDetails()
        {
            // Arrange
            var domain = new Order { Id = 7, TotalAmount = 123.45M };
            _orderRepoMock
                .Setup(r => r.GetIncludingAsync(
                    It.IsAny<Expression<Func<Order, bool>>>(),
                    It.IsAny<Expression<Func<Order, object>>[]>()
                ))
                .ReturnsAsync(domain);

            var expectedDto = new OrderDetailsDto { Id = 7, TotalAmount = 123.45M };
            // Let AutoMapper map it automatically
            // (or you can set up _mapperMock if you injected a mock)

            // Act
            var response = await _service.GetOrderByIdAsync(7);

            // Assert
            Assert.IsType<SuccessResponse<object>>(response);
            var success = (SuccessResponse<object>)response;
            var dto = Assert.IsType<OrderDetailsDto>(success.Data);
            Assert.Equal(7, dto.Id);
            Assert.Equal(123.45M, dto.TotalAmount);
        }
    }
}
