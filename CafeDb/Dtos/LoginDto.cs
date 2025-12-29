using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CafeDb.Dtos
{
    public class LoginDto
    {
        [EmailAddress]
        public required string Email { get; set; }
        [PasswordPropertyText]
        public required string Password { get; set; }
        public string? Role {get; set;}
    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = null!;
        public DateTime Expires { get; set; }
    }
}