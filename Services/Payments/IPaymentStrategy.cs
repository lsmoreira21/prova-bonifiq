using ProvaPub.Models;

namespace ProvaPub.Payments
{
    public interface IPaymentStrategy
    {
        Task<bool> PayOrderAsync(Order order, decimal amount);
    }
}