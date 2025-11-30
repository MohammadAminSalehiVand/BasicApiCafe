using CafeDb.Models;
using System.ComponentModel.DataAnnotations;

namespace CafeDb.Dtos
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public required string ProductName { get; set; }
        public string? Description { get; set; } = string.Empty;
        public required uint ProductInventory { get; set; }
        public required float Price { get; set; }
        public required byte OffPricePercent { get; set; }
    }
    public class ProductAdminDto
    {
        public Guid Id { get; set; }
        public required string ProductName { get; set; }
        public DateTime? AddDate { get; set; }
        public string? Description { get; set; } = string.Empty;
        public required uint ProductInventory { get; set; }
        public required float Price { get; set; }
        public required byte OffPricePercent { get; set; }
        public List<BillEntity>? ProductHistory { get; set; }
        public string? AdminDescription { get; set; } = string.Empty;

    }
    public class ProductUpdateDto
    {
        public required Guid Id { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; } = string.Empty;
        public uint? ProductInventory { get; set; }
        public float? Price { get; set; }
        public byte? OffPricePercent { get; set; }
        public string? UpdateDescription { get; set; }
    }
    public class ProductUserBuyDto
    {
        public Guid Id { get; set; }
        public required uint Amount { get; set; }
    }
}
