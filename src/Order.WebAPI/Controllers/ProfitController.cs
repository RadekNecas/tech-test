using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.Service;
using System.Threading.Tasks;

namespace Order.WebAPI.Controllers
{
    [ApiController]
    [Route("profits")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class ProfitController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public ProfitController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfitByMonth()
        {
            var profit = await _orderService.GetProfitByMonthAsync();
            return Ok(profit);
        }
    }
}
