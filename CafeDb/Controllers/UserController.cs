using CafeDb.Dtos;
using CafeDb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeDb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserService service) : ControllerBase
    {
        private readonly IUserService _service = service;

        [HttpPost("create")]
        public async Task<IActionResult> Create(UserDto dto)
        {
            var result = await _service.Create(dto);
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(UserUpdateDto dto)
        {
            var result = await _service.Update(dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.Delete(id);
            if (!result) return NotFound();
            return Ok(result);
        }

        [HttpGet("readAll")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAll();
            return Ok(result);
        }

        [HttpGet("read/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetById(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpPut("ChangingRolling")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangingRolling(string role , Guid id)
        {
            var result = await _service.ChangingRole(role,id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpGet("GetBills")]
        [Authorize]
        public async Task<IActionResult> GetBills()
        {
            var result = await _service.GetAllBills();
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}