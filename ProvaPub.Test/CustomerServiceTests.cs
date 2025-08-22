using Xunit;
using ProvaPub.Repository;
using ProvaPub.Services;
using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using Moq;

public class CustomerServiceTests
{
    private TestDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new TestDbContext(options);
    }

    private void SeedCustomer(TestDbContext ctx, int customerId, bool hasOrders = false)
    {
        var customer = new Customer { Id = customerId };
        if (hasOrders)
        {
            customer.Orders = new System.Collections.Generic.List<Order>
            {
                new Order { CustomerId = customerId, OrderDate = DateTime.UtcNow.AddMonths(-2) }
            };
        }
        ctx.Customers.Add(customer);
        ctx.SaveChanges();
    }

    private void SeedOrder(TestDbContext ctx, int customerId, DateTime orderDate)
    {
        ctx.Orders.Add(new Order { CustomerId = customerId, OrderDate = orderDate });
        ctx.SaveChanges();
    }

    [Fact (DisplayName= "Custumer Inválido" )]
    public async Task ThrowsException_WhenCustomerIdIsInvalid()
    {
        var ctx = GetDbContext();
        var pagedServiceMock = new Mock<PagedService>();
        var service = new CustomerService(ctx, pagedServiceMock.Object);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.CanPurchase(0, 50));
    }

    [Fact (DisplayName= "Valor Inválido")]
    public async Task ThrowsException_WhenPurchaseValueIsInvalid()
    {
        var ctx = GetDbContext();
        var pagedServiceMock = new Mock<PagedService>();
        var service = new CustomerService(ctx, pagedServiceMock.Object);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.CanPurchase(1, 0));
    }

    [Fact (DisplayName = "Custumer não existe")]
    public async Task ThrowsException_WhenCustomerDoesNotExist()
    {
        var ctx = GetDbContext();
        var pagedServiceMock = new Mock<PagedService>();
        var service = new CustomerService(ctx, pagedServiceMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CanPurchase(999, 50));
    }

    [Fact (DisplayName = "Custumer Ja fez pedido no mes")]
    public async Task ReturnsFalse_WhenCustomerHasOrderThisMonth()
    {
        var ctx = GetDbContext();
        SeedCustomer(ctx, 1, true);
        SeedOrder(ctx, 1, DateTime.UtcNow.AddDays(-1));
        var pagedServiceMock = new Mock<PagedService>();
        var service = new CustomerService(ctx, pagedServiceMock.Object);

        var result = await service.CanPurchase(1, 50);
        Assert.False(result);
    }

    [Fact (DisplayName = "A primeira compra é maior que 100")]
    public async Task ReturnsFalse_WhenFirstPurchaseIsGreaterThan100()
    {
        var ctx = GetDbContext();
        SeedCustomer(ctx, 2, false);
        var pagedServiceMock = new Mock<PagedService>();
        var service = new CustomerService(ctx, pagedServiceMock.Object);

        var result = await service.CanPurchase(2, 150);
        Assert.False(result);
    }


    [Fact(DisplayName = "Todas as regras de negócios são atendidas")]
    public async Task ReturnsTrue_WhenAllBusinessRulesAreSatisfied()
    {
        var ctx = GetDbContext();
        SeedCustomer(ctx, 4, false);
        var pagedServiceMock = new Mock<PagedService>();
        var service = new CustomerService(ctx, pagedServiceMock.Object);
        var result = await service.CanPurchase(4, 50);
        Assert.True(result);
    }
}