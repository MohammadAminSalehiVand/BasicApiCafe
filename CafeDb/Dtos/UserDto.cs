using CafeDb.Models;
using System.ComponentModel.DataAnnotations;

namespace CafeDb.Dtos
{
    public class UserDto
    {
        public Guid? Id { get; set; }
        public required string FullName { get; set; }
        [EmailAddress]
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Password { get; set; }
        public required string EntityRole { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool IsMarried { get; set; }
    }
    public class UserUpdateDto
    {
        public required Guid Id { get; set; }
        public  string? FullName { get; set; }
        public  string? Email { get; set; }
        public  string? PhoneNumber { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool? IsMarried { get; set; }
    }
    public class UserResponse
    {
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string EntityRole { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool? IsMarried { get; set; }
        public List<BillEntity>? Bills { get; set; }
    }
    public class UserGetallDto
    {
        public Guid Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string EntityRole { get; set; }

        public DateTime? BirthDate { get; set; }
        public bool? IsMarried { get; set; }
    }
    public class UserResponseAdminSide
    {
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string EntityRole { get; set; }
        public required string Description { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool? IsMarried { get; set; }
    }
    public class UserClearifyDto
    {
        public Guid Id { get; set; }
        public required DateTime UnusedUserTime { get; set; }
    }
    public class UserCartListDto
    {
        public Guid id { get; set; }
        public List<ProductUserBuyDto>? CartProductList { get; set; }
    }
}
