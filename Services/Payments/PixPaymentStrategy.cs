using ProvaPub.Models;

namespace ProvaPub.Payments
{
    public class PixPaymentStrategy : IPaymentStrategy
    {
        public async Task<bool> PayOrderAsync(Order order, decimal amount)
        {
            // Lógica de pagamento via Pix
            return await Task.FromResult(true);
        }
    }
}