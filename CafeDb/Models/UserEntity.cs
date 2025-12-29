using CafeDb.Dtos;
using Microsoft.OpenApi.Any;
using System.ComponentModel.DataAnnotations;

namespace CafeDb.Models
{
    public class UserEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(100)]
        [MinLength(3)]
        public string FullName { get; set; } = null!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string PhoneNumber { get; set; } = null!;
        [Required]
        [MinLength(5)]
        public required string Password { get; set; }
        public required string EntityRole { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool? IsMarried { get; set; }
        public DateTime CreatedUserTime { get; private set; } = DateTime.UtcNow;
        public DateTime UnusedUserTime { get; set; } = DateTime.UtcNow;
        public List<ProductUserBuyDto>? CartProductList { get; set; }
        public ICollection<BillEntity>? Bills { get; set; }
        public string? GoogleId { get; set; }
        public GoogleUserEntity? GoogleUser { get; set; }

    }
}