using AutoMapper;
using Customers.Application.Repositories;
using Customers.Domain.Entities;
using Customers.Persistence.Contexts;
using Customers.Persistence.Messaging.Consumers;
using FluentAssertions;
using MassTransit;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Shared.Events;
using System.Linq.Expressions;
using Transactions.Application.DTOs;
using Transactions.Application.Repositories;
using Transactions.Application.Services.Persistence;
using Transactions.Domain;
using Transactions.Domain.Entities;
using Transactions.Persistence.Services;

namespace Transactions.Tests.Unit;

public class TransactionServiceTests
{
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRequestClient<PurchaseEvent>> _purchaseClientMock;
    private readonly TransactionService _transactionService;

    public TransactionServiceTests()
    {
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _mapperMock = new Mock<IMapper>();
        _purchaseClientMock = new Mock<IRequestClient<PurchaseEvent>>();
        _customerRepositoryMock = new Mock<ICustomerRepository>();

        _transactionService = new TransactionService(
            _transactionRepositoryMock.Object,
            _publishEndpointMock.Object,
            _purchaseClientMock.Object,
            _mapperMock.Object
        );
    }


    [Fact]
    public async Task TopUpAsync_ShouldIncreaseBalanceBy20AZN_WhenTopUpSucceeds()
    {
        // Arrange
        var transactionDto = new AddTransactionDTO(1, 20.0);
        var transaction = new Transaction { CustomerId = 1, Amount = 20.0, Type = TransactionTypes.TopUp };
        var customer = new Customer { Id = 1, Balance = 30.0, Birthdate = DateTime.Now, Name = "", Surname = "", PhoneNumber = "" };

        _mapperMock.Setup(m => m.Map<Transaction>(transactionDto)).Returns(transaction);
        _customerRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>())).ReturnsAsync(customer);

        _customerRepositoryMock.Setup(r => r.Update(It.IsAny<Customer>())).Verifiable();
        _customerRepositoryMock.Setup(r => r.SaveAsync()).ReturnsAsync(1);
        _customerRepositoryMock.Setup(r => r.BeginTransaction()).Returns(Mock.Of<IDbContextTransaction>());

        _publishEndpointMock.Setup(x => x.Publish(It.IsAny<TopUpEvent>(), It.IsAny<CancellationToken>())).Verifiable();

        // Act
        var result = await _transactionService.TopUpAsync(transactionDto);

        var topUpEvent = new TopUpEvent(1, 20.0);
        var consumer = new TopUpEventConsumer(_customerRepositoryMock.Object);
        var consumeContextMock = new Mock<ConsumeContext<TopUpEvent>>();
        consumeContextMock.SetupGet(x => x.Message).Returns(topUpEvent);
        await consumer.Consume(consumeContextMock.Object);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Operation completed successfully.");

        _transactionRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Transaction>()), Times.Once);
        _transactionRepositoryMock.Verify(x => x.SaveAsync(), Times.Once);

        customer.Balance.Should().Be(50.0);

        _customerRepositoryMock.Verify(x => x.Update(It.Is<Customer>(c => c.Balance == 50.0)), Times.Once);

        _publishEndpointMock.Verify(x => x.Publish(It.Is<TopUpEvent>(e =>
            e.CustomerId == 1 && e.Amount == 20.0), CancellationToken.None), Times.Once);
    }



    [Fact]
    public async Task PurchaseAsync_ShouldDecreaseBalanceBy10AZN_WhenPurchaseSucceeds()
    {
        // Arrange
        var transactionDto = new AddTransactionDTO(1, 10.0);
        var transaction = new Transaction { CustomerId = 1, Amount = 10.0, Type = TransactionTypes.Purchase };
        var customer = new Customer { Id = 1, Balance = 30.0, Birthdate = DateTime.Now, Name = "", Surname = "", PhoneNumber = "" };

        _mapperMock.Setup(m => m.Map<Transaction>(transactionDto)).Returns(transaction);

        _transactionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Transaction>())).Verifiable();
        _transactionRepositoryMock.Setup(r => r.SaveAsync()).Verifiable();

        _customerRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>())).ReturnsAsync(customer);
        _customerRepositoryMock.Setup(r => r.BeginTransaction()).Returns(Mock.Of<IDbContextTransaction>());
        _customerRepositoryMock.Setup(r => r.Update(It.IsAny<Customer>())).Verifiable();
        _customerRepositoryMock.Setup(r => r.SaveAsync()).Verifiable();

        _purchaseClientMock
            .Setup(m => m.GetResponse<PurchaseResultEvent>(
                It.IsAny<PurchaseEvent>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<RequestTimeout>()
            ))
            .ReturnsAsync(Mock.Of<Response<PurchaseResultEvent>>(r =>
                r.Message == new PurchaseResultEvent(true, "Purchase successful") && r.Message.Success == true));

        var purchaseEvent = new PurchaseEvent(1, 10.0, Guid.NewGuid());
        var consumeContextMock = new Mock<ConsumeContext<PurchaseEvent>>();
        consumeContextMock.SetupGet(x => x.Message).Returns(purchaseEvent);

        // Act 
        var result = await _transactionService.PurchaseAsync(transactionDto);

        var consumer = new PurchaseEventConsumer(_customerRepositoryMock.Object);
        await consumer.Consume(consumeContextMock.Object);

        // Assert 
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Operation completed successfully.");

        _transactionRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Transaction>()), Times.Once);
        _transactionRepositoryMock.Verify(x => x.SaveAsync(), Times.Once);

        customer.Balance.Should().Be(20.0);
    }


    [Fact]
    public async Task RefundAsync_ShouldIncreaseBalanceBy5AZN_WhenRefundSucceeds()
    {
        // Arrange
        var transactionDto = new AddTransactionDTO(1, 5.0);
        var lastPurchase = new Transaction { CustomerId = 1, Amount = 10.0, Type = TransactionTypes.Purchase, Date = DateTime.Now.AddDays(-1) };
        var transaction = new Transaction { CustomerId = 1, Amount = 5.0, Type = TransactionTypes.Refund };

        var customer = new Customer { Id = 1, Balance = 30.0, Birthdate = DateTime.Now, Name = "", Surname = "", PhoneNumber = "" };

        _mapperMock.Setup(m => m.Map<Transaction>(transactionDto)).Returns(transaction);

        _transactionRepositoryMock.Setup(r => r.GetWhere(It.IsAny<Expression<Func<Transaction, bool>>>(), It.IsAny<bool>()))
              .Returns((Expression<Func<Transaction, bool>> predicate, bool tracking) =>
              {
                  // Create a sample transaction list
                  var transactions = new List<Transaction>
                  {
                    new Transaction { CustomerId = 1, Amount = 10.0, Type = TransactionTypes.Purchase, Date = DateTime.Now.AddDays(-1) }
                  }.AsQueryable();

                  // Apply the predicate correctly by calling AsQueryable and then passing the expression
                  return transactions.Where(predicate).AsQueryable();
              });

        _transactionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Transaction>())).Verifiable();
        _transactionRepositoryMock.Setup(r => r.SaveAsync()).Verifiable();

        _customerRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>())).ReturnsAsync(customer);
        _customerRepositoryMock.Setup(r => r.BeginTransaction()).Returns(Mock.Of<IDbContextTransaction>());
        _customerRepositoryMock.Setup(r => r.Update(It.IsAny<Customer>())).Verifiable();
        _customerRepositoryMock.Setup(r => r.SaveAsync()).Returns(Task.FromResult(1));

        _publishEndpointMock
            .Setup(m => m.Publish(It.IsAny<RefundEvent>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act 
        var result = await _transactionService.RefundAsync(transactionDto);

        var refundEvent = new RefundEvent(1, 5.0, lastPurchase.Id);
        var consumeContextMock = new Mock<ConsumeContext<RefundEvent>>();
        consumeContextMock.SetupGet(x => x.Message).Returns(refundEvent);

        var consumer = new RefundEventConsumer(_customerRepositoryMock.Object);
        await consumer.Consume(consumeContextMock.Object);

        // Assert 
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Operation completed successfully.");

        _transactionRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Transaction>()), Times.Once);
        _transactionRepositoryMock.Verify(x => x.SaveAsync(), Times.Once);

        customer.Balance.Should().Be(35.0);

        _publishEndpointMock.Verify(x => x.Publish(It.Is<RefundEvent>(e =>
            e.CustomerId == 1 && e.Amount == 5.0 && e.TransactionId == lastPurchase.Id), CancellationToken.None), Times.Once);
    }
}