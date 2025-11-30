using CafeDb.Dtos;
using System.ComponentModel.DataAnnotations;

namespace CafeDb.Models
{
    public class ProductEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public required string ProductName { get; set; }
        public DateTime AddDate { get; private set; } = DateTime.UtcNow;
        [MaxLength(1027)]
        public string? Description { get; set; } = String.Empty;
        public uint ProductInventory { get; set; }
        [Required]
        public required float Price { get; set; }
        public required byte OffPricePercent { get; set; }
        public ICollection<BillEntity>? Bills { get; set; } 
    }
}