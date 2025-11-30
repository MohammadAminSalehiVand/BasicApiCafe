namespace CafeDb.Dtos
{
    public class BillDto
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public required Guid CustomerId { get; set; }
        public required Guid ProductId { get; set; }
        public required double TotalPrice { get; set; }
        public required uint TotalAmount { get; set; }
        public required string ProductName { get; set; }
    }
    public class BillUserDto
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public required Guid ProductId { get; set; }
        public required double TotalPrice { get; set; }
        public required uint TotalAmount { get; set; }
        public required string ProductName { get; set; }
    }
}
