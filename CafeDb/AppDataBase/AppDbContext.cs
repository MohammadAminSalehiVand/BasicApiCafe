using CafeDb.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeDb.AppDataBase
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<UserEntity> Users => Set<UserEntity>();
        public DbSet<ProductEntity> Products => Set<ProductEntity>();
        public DbSet<BillEntity> Bills => Set<BillEntity>();
        public DbSet<GoogleUserEntity> GoogleUsers => Set<GoogleUserEntity>();
    }
}
