using CafeDb.AppDataBase;
using CafeDb.Dtos;
using CafeDb.Models;

namespace CafeDb.Services
{
    public interface IGoogleUserService
    {
        Task<GoogleUserDto> Create(string GoogleId, string email, string? name);
        Task<GoogleUserDto> Merging(string GoogleId, string email, UserEntity user, string? name);
    }
    public class GoogleUserService(IHttpContextAccessor _httpContextAccessor,
                             AppDbContext _dbContext) : IGoogleUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor = _httpContextAccessor;
        private readonly AppDbContext dbContext = _dbContext;

        public async Task<GoogleUserDto> Create(string GoogleId, string email, string? name)
        {
            name ??= "null";
            GoogleUserEntity entity = new()
            {
                GoogleId = GoogleId,
                Name = name,
                Email = email
            };
            await dbContext.GoogleUsers.AddAsync(entity);
            await dbContext.SaveChangesAsync();
            return new GoogleUserDto
            {
                Email = email,
                Name = name,
                Caption = "Created"
            };
        }
        public async Task<GoogleUserDto> Merging(string GoogleId, string email, UserEntity user, string? name)
        {
            name ??= "null";
            GoogleUserEntity newEntity = new()
            {
                GoogleId = GoogleId,
                Name = name,
                Email = email,
                UserId = user.Id,
                User = user
            };
            user.GoogleId = GoogleId;
            user.GoogleUser = newEntity;
            dbContext.Users.Update(user);
            await dbContext.GoogleUsers.AddAsync(newEntity);
            await dbContext.SaveChangesAsync();
            return new GoogleUserDto
            {
                Email = email,
                Name = name,
                Caption = "Merged"
            };
        }
    }
}
