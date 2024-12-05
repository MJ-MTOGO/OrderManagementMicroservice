using System.Threading.Tasks;
using OrderManagementService.Application.DTOs;
using OrderManagementService.Application.Ports;
using OrderManagementService.Domain.Entities;

namespace OrderManagementService.Application.Services
{
    public class EarningService : IEarningService
    {
        private readonly IEarningRepository _earningRepository;

        public EarningService(IEarningRepository earningRepository)
        {
            _earningRepository = earningRepository;
        }

        public async Task ProcessCalculatedEarningsAsync(Earning earnings)
        {
       

            // Save to the database using the repository
            await _earningRepository.AddEarningAsync(earnings);
        }
    }
}
