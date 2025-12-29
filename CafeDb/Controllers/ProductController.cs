using CafeDb.Dtos;
using CafeDb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace CafeDb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController(IProductService productService) : ControllerBase
    {
        private readonly IProductService _service = productService;
        [HttpPost("GetAllProduct")]
        [Authorize(Roles = "Admin")]
        public IActionResult GettAll(int page = 1 , int pageSize = 10)
        {
            var result = _service.GetAllProduct(page,pageSize);
            return Ok(result);
        }

        [HttpGet("GetByIdProduct/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _service.GetProduct(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("CreateProduct")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct(ProductDto param)
        {
            var result = await _service.CreateProduct(param);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPut("UpdateProduct")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(ProductUpdateDto param)
        {
            var result = await _service.ProductUpdate(param);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpDelete("DeleteProduct")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletProduct(Guid id)
        {
            var result = await _service.DeleteProduct(id);
            return Ok(result);
        }

        [HttpPost("BuyProduct")]
        [Authorize]
        public async Task<IActionResult> BuyProduct(ProductUserBuyDto param)
        {
            var result = await _service.BuyProduct(param)!;
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("SearchingInProducts/{text}")]
        public async Task<IActionResult> SearchingInProducts(string text)
        {
            var result = await _service.SearchingInProducts(text);
            if (result == null) return Ok(new ProductDto
            {
                Id =  Guid.Empty,
                ProductName = String.Empty,
                Description = "Not Found",
                ProductInventory = 0,
                Price = 0,
                OffPricePercent = 0
            });
            return Ok(result);
        }
    }
}
