using CafeDb.Dtos;
using CafeDb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CafeDb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController(IProductService productService) : ControllerBase
    {
        private readonly IProductService _service = productService;
        [HttpGet("GetAllProduct")]
        //[Authorize(Roles = "Admin")]
        public IActionResult GettAll()
        {
            var result = _service.GetAllProduct();
            return Ok(result);
        }

        [HttpGet("GetByIdProduct")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetById(Guid id)
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
    }
}
