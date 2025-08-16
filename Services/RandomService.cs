using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
    public class RandomService
    {
        Random _random = new Random();
        TestDbContext _ctx;
        public RandomService()
        {
            var contextOptions = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Teste;Trusted_Connection=True;")
                .Options;
            _ctx = new TestDbContext(contextOptions);
        }
        public async Task<string> GetRandom()
        {
            var number = _random.Next(100);

            // Verifica se o número já existe no banco
            bool exists = await _ctx.Numbers.AnyAsync(n => n.Number == number);
            if (exists)
            {
                return $"O número {number} já foi cadastrado.";
            }

            _ctx.Numbers.Add(new RandomNumber() { Number = number });
            await _ctx.SaveChangesAsync();
            return $"Número {number} cadastrado com sucesso.";
        }
    }
}
