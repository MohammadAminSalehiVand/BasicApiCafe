using CafeDb.AppDataBase;
using CafeDb.Dtos;
using CafeDb.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CafeDb.Services
{
    public interface IProductService
    {
        Task<ProductAdminDto?> CreateProduct(ProductDto product);
        Task<ProductUpdateDto?> ProductUpdate(ProductUpdateDto product);
        Task<bool> DeleteProduct(Guid id);
        Task<object?> GetProduct(string id);
        IEnumerable<ProductAdminDto> GetAllProduct(int page , int pageSize );
        Task<BillDto>? BuyProduct(ProductUserBuyDto product);
        Task<IEnumerable<ProductDto>?> SearchingInProducts(string searchText);
        //Task<bool> AddingToCart(string id);
    }
    public class ProductService(IHttpContextAccessor httpContextAccessor, AppDbContext _dbContext) : IProductService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly AppDbContext dbContext = _dbContext;

        public async Task<BillDto>? BuyProduct(ProductUserBuyDto product)
        {
            ProductEntity? productEntity = await dbContext.Products.FindAsync(product.Id);
            if (productEntity == null) return null!;
            if (productEntity.ProductInventory < product.Amount) return null!;
            var user = _httpContextAccessor.HttpContext?.User;
            var subClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(subClaim) || !Guid.TryParse(subClaim, out Guid customerId))
                throw new UnauthorizedAccessException("شناسه کاربر معتبر نیست یا احراز هویت انجام نشده.");
            BillEntity bill = new()
            {
                Id = Guid.NewGuid(),
                UserId = customerId,
                ProductId = productEntity.Id,
                ProductName = productEntity.ProductName,
                TotalAmount = product.Amount,
                TotalPrice = (product.Amount * productEntity.Price) - (productEntity.OffPricePercent * productEntity.Price),
                Product = productEntity,
                User = await dbContext.Users.FindAsync(customerId) ?? null!,
            };
            productEntity.ProductInventory -= product.Amount;
            //productEntity.Bills
            dbContext.Products.Update(productEntity);
            await dbContext.Bills.AddAsync(bill);
            await dbContext.SaveChangesAsync();
            return new BillDto 
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                ProductId = productEntity.Id,
                ProductName = productEntity.ProductName,
                TotalAmount = product.Amount,
                TotalPrice = product.Amount * productEntity.Price,
            };
        }

        public async Task<ProductAdminDto?> CreateProduct(ProductDto product)
        {
            if(await dbContext.Products.FirstOrDefaultAsync(p => p.ProductName == product.ProductName) != null)
            {
                return new ProductAdminDto
                {
                    ProductName = product.ProductName,
                    OffPricePercent = product.OffPricePercent,
                    Price = product.Price,
                    ProductInventory = product.ProductInventory,
                    Id = product.Id,
                    AdminDescription = "Product Name is not avalaible. the product didnt set"
                };
            }
            ProductEntity entity = new()
            {
                ProductName = product.ProductName,
                Price = product.Price,
                OffPricePercent = product.OffPricePercent,
                Id = Guid.NewGuid(),
                Description = product.Description,
                ProductInventory = product.ProductInventory
            };
            ProductEntity response = dbContext.Products.Add(entity).Entity;
            await dbContext.SaveChangesAsync();
            return new ProductAdminDto 
            {
                ProductName = response.ProductName,
                OffPricePercent = response.OffPricePercent,
                Price = response.Price,
                ProductInventory = response.ProductInventory,
                Id = response.Id,
                Description = response.Description,
                AddDate = response.AddDate,
                AdminDescription = "Product has set!",
            };
        }

        public async Task<bool> DeleteProduct(Guid id)
        {
            ProductEntity? product = await dbContext.Products.FindAsync(id);
            if (product == null)
            {
                return false;
            }
            dbContext.Products.Remove(product);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public IEnumerable<ProductAdminDto> GetAllProduct(int page , int pageSize)
        {
            IEnumerable<ProductAdminDto> entityList =  dbContext.Products.Skip((page -1) * pageSize).Take(pageSize).Select(p => new ProductAdminDto
            {
                Id = p.Id ,
                ProductName = p.ProductName ,
                OffPricePercent= p.OffPricePercent ,
                Price = p.Price,
                ProductInventory = p.ProductInventory ,
                AddDate = p.AddDate,
                Description = p.Description,
            });
            return entityList;
        }

        public async Task<Object?> GetProduct(string id)
        {
            Guid realId;
            try
            {
                realId = Guid.Parse(id);
            }
            catch
            {
                return null;
            }
            ProductEntity? entity = await dbContext.Products.FindAsync(realId);
            if (entity == null)
            {
                return null;
            }
            var user = _httpContextAccessor.HttpContext?.User;
            var role = user?.FindFirst(ClaimTypes.Role)?.Value;
            if (role == "Admin")
                return new ProductAdminDto
                {
                    OffPricePercent = entity.OffPricePercent,
                    Price = entity.Price,
                    ProductInventory = entity.ProductInventory,
                    ProductName = entity.ProductName,
                    Description = entity.Description,
                    Id = realId,
                    AddDate = entity.AddDate
                };
            return new ProductDto
            {
                OffPricePercent = entity.OffPricePercent,
                Price = entity.Price,
                ProductInventory = entity.ProductInventory,
                ProductName = entity.ProductName,
                Description = entity.Description,
                Id = realId,
            };
        }

        public async Task<ProductUpdateDto?> ProductUpdate(ProductUpdateDto product)
        {
            ProductEntity? entity = await dbContext.Products.FindAsync(product.Id);
            if (entity == null)
            {
                return null;
            }
            if (!String.IsNullOrEmpty(product.ProductName)) entity.ProductName = product.ProductName;
            if (!String.IsNullOrEmpty(product.Description)) entity.Description = product.Description;
            if (product.ProductInventory != null) entity.ProductInventory = (uint)product.ProductInventory;
            if (product.Price != null) entity.Price = (float)product.Price;
            if (product.OffPricePercent != null) entity.OffPricePercent = (byte)product.OffPricePercent;
            dbContext.Update(entity);
            await dbContext.SaveChangesAsync();
            product.UpdateDescription = "Update has done";
            return product;
        }

        public async Task<IEnumerable<ProductDto>?> SearchingInProducts(string searchText)
        {
            IEnumerable<ProductDto>? products = await dbContext.Products
                .Where(p => p.ProductName.ToLower().Contains(searchText.Trim().ToLower()) || (p.Description != null && p.Description.ToLower().Contains(searchText.Trim().ToLower())))
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Description = p.Description,
                    ProductInventory = p.ProductInventory,
                    Price = p.Price,
                    OffPricePercent = p.OffPricePercent
                })
                .ToListAsync();
            if (products != null && products.Count() != 0)
                return products;
            else return null;
        }

        //public async Task<bool> AddingToCart(string id)
        //{
        //    Guid realId;
        //    try { realId = Guid.Parse(id); }
        //    catch { return false; }
        //    if (dbContext.Products.FirstOrDefaultAsync(x => x.Id == realId ) != null)
        //    {
        //        UserEntity = await dbContext.Products.FirstOrDefaultAsync( x => x.Id == realId );
        //        //await dbContext.Users.Update()
        //    }
        //    else return false;
        //}
    }
}
