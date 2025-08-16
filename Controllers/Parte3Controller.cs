using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Payments;
using ProvaPub.Repository;
using ProvaPub.Services;

namespace ProvaPub.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Parte3Controller : ControllerBase
    {
        // Lista de métodos de pagamento válidos
        private static readonly List<string> ValidPaymentMethods = new()
        {
            "Pix",
            "CreditCard",
            "Paypal"
        };

        [HttpGet("payment-methods")]
        public IActionResult GetPaymentMethods()
        {
            return Ok(ValidPaymentMethods);
        }

        [HttpGet("orders")]
        public async Task<IActionResult> PlaceOrder(string paymentMethod, decimal paymentValue, int customerId)
        {
            if (!ValidPaymentMethods.Contains(paymentMethod))
                return BadRequest($"Método de pagamento inválido. Métodos válidos: {string.Join(", ", ValidPaymentMethods)}");

            var contextOptions = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Teste;Trusted_Connection=True;")
                .Options;

            using var context = new TestDbContext(contextOptions);

            var paymentStrategies = new List<IPaymentStrategy>
            {
                new PixPaymentStrategy(),
                new CreditCardPaymentStrategy(),
                new PaypalPaymentStrategy()
            };

            var orderService = new OrderService(context, paymentStrategies);

            var order = new Order
            {
                CustomerId = customerId,
                OrderDate = DateTime.UtcNow, // Salva como UTC
                Value = paymentValue
            };

            await orderService.PayOrder(order, paymentValue, paymentMethod);


            order = await orderService.InsertOrder(order); // Salva no banco

            order.OrderDate = TimeZoneInfo.ConvertTimeFromUtc(order.OrderDate,
                TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

            return Ok(order);
        }
    }
}
