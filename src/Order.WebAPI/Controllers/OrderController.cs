using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.Model;
using Order.Service;
using Order.Service.Exceptions;
using Order.WebAPI.ViewModels;
using System;
using System.Threading.Tasks;

namespace OrderService.WebAPI.Controllers
{
    [ApiController]
    [Route("orders")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFiltered([FromQuery] GetOrdersParameters parameters = null)
        {
            var ordersSpecification = parameters?.AsOrderSpecification();
            var orders = await _orderService.GetOrdersAsync(ordersSpecification);
            return Ok(orders);
        }

        [HttpGet("{orderId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderById(Guid orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            //if (order != null)
            //{
            //    return Ok(order);
            //}
            //else
            //{
            //    return NotFound();
            //}
            if(order == null)
            {
                throw new ApiNotFoundException($"Invalid request. Order with Id '{orderId}' does not exist.");
            }

            return Ok(order);
        }

        [HttpPatch("{orderId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrder(Guid orderId, [FromBody]OrderToUpdate orderToUpdate)
        {
            var updatedOrder = await _orderService.UpdateOrderAsync(orderId, orderToUpdate);
            if(updatedOrder == null)
            {
                throw new ApiNotFoundException($"Invalid request. Order with Id '{orderId}' does not exist.");
            }

            return Ok(updatedOrder);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddOrder([FromBody]AddOrder orderToAdd)
        {
            var createdOrder = await _orderService.AddOrderAsync(orderToAdd);
            return Ok(createdOrder);
        }
    }
}
