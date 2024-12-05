using OrderManagementService.Domain.Entities;

namespace OrderManagementService.Application.Ports
{
    public interface IEarningRepository
    {
        Task AddEarningAsync(Earning earning);
        Task <IEnumerable<Earning>> GetAllEarnings();
    }
}
