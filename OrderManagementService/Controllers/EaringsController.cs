using Microsoft.AspNetCore.Mvc;
using OrderManagementService.Application.Ports;

namespace OrderManagementService.Controllers
{
    [ApiController]
    [Route("api/earnings")]
    public class EaringsController : Controller
    {
        private readonly IEarningRepository _earningRepository;


        public EaringsController(IEarningRepository earningRepository)
        {
            _earningRepository = earningRepository;     
        }

        [HttpGet()]
        public async Task<IActionResult> GetAllEarnings()   
        {
            var earnings = await _earningRepository.GetAllEarnings();
            return earnings == null || !earnings.Any() ? NotFound("No earnings found.") : Ok(earnings);
        }
    }
}
