using OrderManagementService.Application.DTOs;
using OrderManagementService.Domain.Entities;

namespace OrderManagementService.Application.Ports
{
    public interface IEarningService
    {
        Task ProcessCalculatedEarningsAsync(Earning earning);
    }
}
