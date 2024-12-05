using System.Threading.Tasks;
using Google.Api;
using Microsoft.EntityFrameworkCore;
using OrderManagementService.Application.Ports;
using OrderManagementService.Domain.Entities;
using OrderManagementService.Infrastructure;

namespace OrderManagementService.Infrastructure.Adapters
{
    public class EarningRepository : IEarningRepository
    {
        private readonly OrderManagementServiceDbContext _dbContext;

        public EarningRepository(OrderManagementServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddEarningAsync(Earning earning)
        {
            await _dbContext.Earnings.AddAsync(earning);
            await _dbContext.SaveChangesAsync();
        }

   
        public async Task<IEnumerable<Earning>> GetAllEarnings()
        {
            return await _dbContext.Earnings.ToListAsync();
        }
    }
}
