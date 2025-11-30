using CafeDb.AppDataBase;
using System.ComponentModel.DataAnnotations;

namespace CafeDb.Models
{
    public class BillEntity
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime Date { get; private set; } = DateTime.UtcNow;
        public required double TotalPrice { get; set; }
        public required uint TotalAmount { get; set; }
        public required string ProductName { get; set; }
        public required Guid UserId { get; set; }
        public required UserEntity User { get; set; }
        public required Guid ProductId { get; set; }
        public required ProductEntity Product { get; set; }
    }
}
