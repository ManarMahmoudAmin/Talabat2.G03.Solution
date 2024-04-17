using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;

namespace Talabat.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : BaseApiController
    {
        private readonly IGenaricRepository<Product> _productsRepo;

        public ProductsController(IGenaricRepository<Product> productsRepo)
        {
            _productsRepo = productsRepo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _productsRepo.GetAllAsync();

            ///JsonResult result = new JsonResult(products);
            ///OkResult result = new OkResult(products);

            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var products = await _productsRepo.GetAsync(id);

            if (products is null)
            {
                return NotFound(new { message = "Not found", statsCode = 404 }); // 404 
            }

            return Ok(products); // 200 
        }
    }
}