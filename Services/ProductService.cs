using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
	public class ProductService
	{
		TestDbContext _ctx;
        PagedService _pagedService;

        public ProductService(TestDbContext ctx)
		{
			_ctx = ctx;

			_pagedService = new PagedService(ctx);
        }

		public PagedList<Product>  ListProducts(int page)
		{
            return _pagedService.GetPagedList(_ctx.Products, page);
        }

	}
}
