using CafeDb.AppDataBase;
using CafeDb.Dtos;
using CafeDb.Models;
using CafeDb.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Security.Claims;

namespace CafeDb.Services
{
    public interface IUserService
    {
        Task<UserResponse> Create(UserDto dto);
        Task<UserResponse?> Update(UserUpdateDto dto);
        Task<bool> Delete(Guid id);
        Task<List<UserGetallDto>> GetAll();
        Task<UserResponse?> GetById(Guid id);
        Task<UserResponseAdminSide?> ChangingRole(string role, Guid id);
        Task<List<BillUserDto>?> GetAllBills();
        Task<GoogleUserDto> MergingWithGoogle(string GoogleId, string email, string? name);
        Task<UserResponse> GettingActiveUserInfo();
    }
    public class UserService(IHttpContextAccessor httpContextAccessor, AppDbContext _dbContext) : IUserService
    {
        private List<string> RoleList { get; set; } = ["Admin", "Writer", "Reader"];
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly AppDbContext dbContext = _dbContext;
        private readonly GoogleUserService googleUserService = new (httpContextAccessor, _dbContext);

        public async Task<UserResponse> Create(UserDto dto)
        {
            UserEntity entity = new()
            {
                Id = Guid.NewGuid(),
                BirthDate = dto.BirthDate,
                Email = dto.Email,
                FullName = dto.FullName,
                IsMarried = dto.IsMarried,
                EntityRole = dto.EntityRole,
                PhoneNumber = dto.PhoneNumber,
                UnusedUserTime = DateTime.UtcNow,
                Password = PasswordHasher.Hash(dto.Password)
            };
            EntityEntry<UserEntity> res = await dbContext.Users.AddAsync(entity);
            await dbContext.SaveChangesAsync();
            return new UserResponse
            {
                Email = res.Entity.Email,
                FullName = res.Entity.FullName,
                IsMarried = res.Entity.IsMarried,
                PhoneNumber = res.Entity.PhoneNumber,
                EntityRole = res.Entity.EntityRole,
                BirthDate = res.Entity.BirthDate
            };
        }

        public async Task<bool> Delete(Guid id)
        {
            UserEntity? user = await dbContext.Users.FindAsync(id);
            if (user == null) return false;
            dbContext.Users.Remove(user);
            await dbContext.SaveChangesAsync();
            return true;
        }
        
        public async Task<List<UserGetallDto>> GetAll()
        {
            List<UserGetallDto> list = await dbContext.Users.Select(u => new UserGetallDto 
            {
                Id = u.Id,
                Email = u.Email,
                FullName= u.FullName,
                IsMarried = u.IsMarried,
                PhoneNumber = u.PhoneNumber,
                EntityRole =  u.EntityRole,
                BirthDate = u.BirthDate
            }).ToListAsync();         
            return list;
        }

        public async Task<UserResponse?> GetById(Guid id)
        {
            UserResponse? user = await dbContext.Users.Where(x => x.Id == id).Select(u => new UserResponse 
            { 
                Email = u.Email,
                FullName = u.FullName,
                IsMarried = u.IsMarried,
                PhoneNumber = u.PhoneNumber,
                EntityRole = u.EntityRole,
                BirthDate = u.BirthDate
            }).FirstOrDefaultAsync();
            if (user == null) 
            {
                return null;
            }
            return new UserResponse
            {
                Email = user.Email,
                FullName = user.FullName,
                IsMarried = user.IsMarried,
                PhoneNumber = user.PhoneNumber,
                EntityRole = user.EntityRole,
                BirthDate = user.BirthDate
            };
        }

        public async Task<UserResponse?> Update(UserUpdateDto dto)
        {
            UserEntity? user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (user == null) return null;
            if (dto.FullName!=null) user.FullName = dto.FullName;
            if (dto.Email!=null) user.Email = dto.Email;
            if (dto.PhoneNumber!=null) user.PhoneNumber = dto.PhoneNumber;
            if (dto.BirthDate!=null) user.BirthDate = dto.BirthDate;
            if (dto.IsMarried!=null) user.IsMarried = dto.IsMarried;
            dbContext.Update(user);
            await dbContext.SaveChangesAsync();
            return new UserResponse 
            { 
                Email = user.Email,
                FullName = user.FullName,
                IsMarried = user.IsMarried,
                PhoneNumber = user.PhoneNumber,
                EntityRole = user.EntityRole,
                BirthDate = user.BirthDate
            };
        }
    
        public async Task<UserResponseAdminSide?> ChangingRole(string role, Guid id)
        {
            UserEntity? entity = await dbContext.Users.FindAsync(id);
            if (entity == null) return null;
            string preRole = entity.EntityRole;
            if (string.IsNullOrEmpty(role)) return null;
            if (!RoleList.Contains(role))
            {
                return null;
            }
            entity.EntityRole = role;
            dbContext.Users.Update(entity);
            await dbContext.SaveChangesAsync();
            return new UserResponseAdminSide
            {
                Email = entity.Email,
                BirthDate= entity.BirthDate,
                IsMarried = entity.IsMarried,
                PhoneNumber = entity.PhoneNumber,
                EntityRole = entity.EntityRole,
                FullName= entity.FullName,
                Description = $"{preRole} Changed to {role}"
            };
        }

        public async Task<List<BillUserDto>?> GetAllBills()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var subClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(subClaim) || !Guid.TryParse(subClaim, out Guid customerId))
                throw new UnauthorizedAccessException("شناسه کاربر معتبر نیست یا احراز هویت انجام نشده.");
            //List<BillUserDto> bills = await dbContext.Bills.Where(x => x.UserId == customerId).Select(b => new BillUserDto
            //{
            //    Id = b.Id,
            //    Date = b.Date,
            //    ProductId = b.ProductId,
            //    TotalPrice = b.TotalPrice,
            //    TotalAmount = b.TotalAmount,
            //    ProductName = b.ProductName
            //}).ToListAsync();
            var bills = await dbContext.Users.Where(x => x.Id == customerId).Select(x => x.Bills).ToArrayAsync();
            if (bills == null || bills.Length == 0)
            {
                return null;
            }
            List<BillUserDto> billsLists = [];
            foreach (var billList in bills)
            {
                if(billList != null)
                    foreach (var bill in billList)
                    {
                        var billDto = new BillUserDto
                        {
                            Id = bill.Id,
                            Date = bill.Date,
                            ProductId = bill.ProductId,
                            TotalPrice = bill.TotalPrice,
                            TotalAmount = bill.TotalAmount,
                            ProductName = bill.ProductName
                        };
                        // Add to result list
                        billsLists.Add(billDto);
                    
                    }
            }
            return billsLists;
        }

        public async Task<GoogleUserDto> MergingWithGoogle(string GoogleId, string email, string? name)
        {
            GoogleUserEntity? entityGoogle = await dbContext.GoogleUsers.FirstOrDefaultAsync(u => u.Email == email);
            UserEntity? entity = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (entity == null && entityGoogle == null)
            {
                GoogleUserDto result = await googleUserService.Create(GoogleId, email, name);
                return result;
            }
            else if (entity != null && entity.GoogleUser == null)
            {
                GoogleUserDto response = await googleUserService.Merging(GoogleId, email, entity, name);
                return response;
            }
            else if(entity == null)
                return new GoogleUserDto
                {
                    Email = email,
                    Name = string.Empty,
                    Caption = "User Null"
                };
            else
                return new GoogleUserDto
                {
                    Email = email,
                    Name = string.Empty,
                    Caption = "Aithorization Done"
                };
        }

        public async Task<UserResponse> GettingActiveUserInfo()
        {
            ClaimsPrincipal? user = httpContextAccessor.HttpContext?.User;
            string? subClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(string.IsNullOrEmpty(subClaim) || !Guid.TryParse(subClaim, out Guid userId))
            {
                return new UserResponse
                {
                    Email = "Not Found",
                    EntityRole = "Not Found",
                    FullName = "Not Found",
                    PhoneNumber = "Not Found"
                };
            }
            UserEntity? actualUser = await dbContext.Users.FindAsync(userId);
            if(actualUser == null) return new UserResponse
            {
                Email = "Not Found",
                EntityRole = "Not Found",
                FullName = "Not Found",
                PhoneNumber = "Not Found"
            };
            return new UserResponse
            {
                Email = actualUser.Email,
                EntityRole = actualUser.EntityRole,
                FullName = actualUser.FullName,
                PhoneNumber = actualUser.PhoneNumber,
            };
        }
    }
}