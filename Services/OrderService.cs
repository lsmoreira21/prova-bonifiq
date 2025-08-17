using ProvaPub.Models;
using ProvaPub.Repository;
using ProvaPub.Payments;

namespace ProvaPub.Services
{
	public class OrderService
	{
        private readonly IDictionary<string, IPaymentStrategy> _paymentStrategies;
        TestDbContext _ctx;

        public OrderService(TestDbContext ctx, IEnumerable<IPaymentStrategy> paymentStrategies)
        {
            _ctx = ctx;
            // Exemplo: registrar estratégias por nome
            _paymentStrategies = paymentStrategies.ToDictionary(
                s => s.GetType().Name.Replace("PaymentStrategy", ""), 
                s => s);
        }

        public async Task<bool> PayOrder(Order order, decimal amount, string paymentMethod)
		{
            if (_paymentStrategies.TryGetValue(paymentMethod, out var strategy))
            {
                return await strategy.PayOrderAsync(order, amount);
            }
            throw new NotSupportedException($"Método de pagamento '{paymentMethod}' não suportado.");
        }

		public async Task<Order> InsertOrder(Order order)
        {
            // Insere pedido no banco de dados
            var entityEntry = await _ctx.Orders.AddAsync(order);
            await _ctx.SaveChangesAsync(); // Salva as alterações no banco
            return entityEntry.Entity;
        }
	}
}
