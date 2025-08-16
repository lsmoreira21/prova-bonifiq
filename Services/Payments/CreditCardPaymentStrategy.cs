using ProvaPub.Models;

namespace ProvaPub.Payments
{
    public class CreditCardPaymentStrategy : IPaymentStrategy
    {
        public async Task<bool> PayOrderAsync(Order order, decimal amount)
        {
            // Lógica de pagamento via cartão de crédito
            return await Task.FromResult(true);
        }
    }
}