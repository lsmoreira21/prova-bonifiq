using ProvaPub.Models;

namespace ProvaPub.Payments
{
    public class PaypalPaymentStrategy : IPaymentStrategy
    {
        public async Task<bool> PayOrderAsync(Order order, decimal amount)
        {
            // Lógica de pagamento via PayPal
            return await Task.FromResult(true);
        }
    }
}