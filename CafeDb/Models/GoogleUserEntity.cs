using System.ComponentModel.DataAnnotations;

namespace CafeDb.Models
{
    public class GoogleUserEntity
    {
        [Key]
        public required string GoogleId { get; set; }
        public required string Name { get; set; }
        [EmailAddress]
        public required string Email { get; set; }
        public Guid? UserId { get; set; }
        public UserEntity? User { get; set; }

    }
}
