namespace CafeDb.Dtos
{
    public class GoogleUserDto
    {
        public required string Email { get; set; }
        public required string Name { get; set; }
        public string? Caption { get; set; }
    }
    public class GoogleUserDtoAdmin
    {
        public required string GoogleId { get; set; }
        public required string Email { get; set; }
        public required string Name { get; set; }
        public string? Caption { get; set; }

    }
}
