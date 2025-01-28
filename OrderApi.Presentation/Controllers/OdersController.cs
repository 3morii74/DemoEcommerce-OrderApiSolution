using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.DTOs.Conversations;
using OrderApi.Application.DTOs;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;
using eCommmerce.SharedLibrary.Responses;

namespace OrderApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OdersController(IOrder orderInterface, IOrderService orderService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
        {
            var orders = await orderInterface.GetAllAsync();
            if (!orders.Any())
            {
                return NotFound("No order detected in the database");
            }
            var (_, list) = OrderConversation.FromEntity(null, orders);
            return !list!.Any() ? NotFound() : Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {
            var order = await orderInterface.FindByIdAsync(id);
            if (order is null)
            {
                return NotFound();
            }

            var (orderDto, _) = OrderConversation.FromEntity(order, null);
            return Ok(orderDto);
        }

        [HttpGet("client/{clientId:int}")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetClientOrders(int clientId)
        {
            if (clientId <= 0) return BadRequest("Invalid data provided");

            var orders = await orderService.GetOrdersByClientId(clientId);
            if (!orders.Any())
            {
                return NotFound("No orders found for the specified client");
            }

            return Ok(orders);
        }

        [HttpGet("details/{orderId:int}")]
        public async Task<ActionResult<OrderDetailsDTO>> GetOrderDetails(int orderId)
        {
            if (orderId <= 0) return BadRequest("Invalid data provided");

            var orderDetail = await orderService.GetOrderDetails(orderId);
            if (orderDetail == null || orderDetail.OrderId <= 0)
            {
                return NotFound("No order found");
            }

            return Ok(orderDetail);
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateOrder(OrderDTO orderDTO)
        {
            // Check model state if all data annotations are passed.
            if (!ModelState.IsValid)
            {
                return BadRequest("Incomplete data submitted");
            }

            // Convert to entity
            var getEntity = OrderConversation.ToEntity(orderDTO);
            var response = await orderInterface.CreateAsync(getEntity);

            return response.flag ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        public async Task<ActionResult<Response>> DeleteOrder(OrderDTO orderDTO)
        {
            // Convert from DTO to entity
            var order = OrderConversation.ToEntity(orderDTO);
            var response = await orderInterface.DeleteAsync(order);

            return response.flag ? Ok(response) : BadRequest(response);
        }
    }
}