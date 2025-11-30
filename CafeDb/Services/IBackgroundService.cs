using CafeDb.AppDataBase;

namespace CafeDb.Services
{
    public class AutomaticProcessor(IServiceProvider serviceProvider) : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var cutoffDate = DateTime.UtcNow.AddDays(-10);
                    var oldUsers = dbContext.Users.Where(u => u.UnusedUserTime < cutoffDate);

                    dbContext.Users.RemoveRange(oldUsers);
                    await dbContext.SaveChangesAsync(stoppingToken);
                }
                await Task.Delay(TimeSpan.FromHours(12), stoppingToken);
            }
        }
    }
}