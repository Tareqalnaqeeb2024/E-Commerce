// E_Commerce.API/Controllers/OrderItemController.cs
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using E_CommerceDataAccess.DTO;
using E_CommerceDataBusiness.Interfaces;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemController : ControllerBase
    {
        private readonly IOrderItemService _orderItemService;
        private readonly IMapper _mapper;

        public OrderItemController(
            IOrderItemService orderItemService,
            IMapper mapper)
        {
            _orderItemService = orderItemService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItemDTO>> GetOrderItem(int id)
        {
            try
            {
                var orderItem = await _orderItemService.GetOrderItemByIdAsync(id);
                return Ok(orderItem);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrderItem(OrderItemCreateDTO orderItemCreate)
        {
            try
            {
                var orderItem = await _orderItemService.CreateOrderItemAsync(orderItemCreate);
                return CreatedAtAction("GetOrderItem", new { id = orderItem.OrderItemId }, orderItem);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItemDTO>>> GetAllOrderItems()
        {
            var orderItems = await _orderItemService.GetAllOrderItemsAsync();
            return Ok(orderItems);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateOrderItem(int id, OrderItemUpdateDTO orderItemUpdate)
        {
            try
            {
                await _orderItemService.UpdateOrderItemAsync(id, orderItemUpdate);
                return Ok("Update OrderItem Successfully");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderItem(int id)
        {
            try
            {
                await _orderItemService.DeleteOrderItemAsync(id);
                return Ok("Deleted OrderItem Successfully");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}