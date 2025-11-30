using CafeDb.AppDataBase;
using CafeDb.Dtos;
using CafeDb.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using CafeDb.Models;
using CafeDb.Services;
using Microsoft.AspNetCore.Authentication;

namespace CafeDb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(AppDbContext db, IConfiguration config, IUserService UserService) : ControllerBase
    {
        private readonly AppDbContext _db = db;
        private readonly IUserService _UserService = UserService;
        private readonly IConfiguration _config = config;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            UserEntity? user = await _db.Users.SingleOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) return Unauthorized();

            bool passwordOk = PasswordHasher.Verify(dto.Password, user.Password);
            if (!passwordOk) return Unauthorized();

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(ClaimTypes.Role, user.EntityRole),
                new("FullName", user.FullName ?? string.Empty)
            };

            var key = _config["Jwt:Key"]!;
            var issuer = _config["Jwt:Issuer"]!;
            var minutes = int.Parse(_config["Jwt:ExpireMinutes"] ?? "60");
            var (token, expires) = JwtTokenGenerator.CreateToken(key, issuer, TimeSpan.FromMinutes(minutes), claims);

            var response = new LoginResponseDto { Token = token, Expires = expires };
            user.UnusedUserTime = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return Ok(response);
        }
        [HttpPost("LoginWithGoogle")]
        public IActionResult LoginGoogle(string? returnUrl = "/")
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action("ExternalCallback", "Auth" , new {returnUrl})
            };
            return Challenge(props, "Google");
        }
        [HttpGet("ExternalCallback")]
        public async Task<IActionResult> ExternalCallback ()
        {
            var result = await HttpContext.AuthenticateAsync("Google");
            if (!result.Succeeded)
                return Redirect("/login?error=external_failed");
            var principal = result.Principal;
            var googleId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = principal.FindFirstValue(ClaimTypes.Email);
            var name = principal.FindFirstValue(ClaimTypes.Name);
            if(googleId == null ||  email == null)
            {
                return Unauthorized();
            }
            GoogleUserDto? finalResult = await _UserService.MergingWithGoogle(googleId, email, name);
            return Ok(finalResult);
        }
    }
}