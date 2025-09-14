// E_Commerce.API/Controllers/OrderController.cs
using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.DTO.Common;
using E_CommerceDataAccess.DTO.Pagination;
using E_CommerceDataBusiness.Interfaces;
using E_CommerceDataBusiness.Interfaces.ExternalInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Security.Claims;
using System.Text;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IRabbitMQService  _rabbitMQService;

    public OrderController(IOrderService orderService , IRabbitMQService rabbitMQService)
    {
        _orderService = orderService;
        _rabbitMQService = rabbitMQService;
    }

  
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var orders = await _orderService.GetUserOrdersAsync(userId);
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDTO>> GetOrder(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
        {
            return NotFound();
        }
        return Ok(order);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<OrderDTO>> CreateOrder(OrderCreateDTO orderCreate)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();
        var order = await _orderService.CreateOrderAsync(orderCreate, userId);

      
        _rabbitMQService.PublishMessage(order, "orders");
       
        return CreatedAtAction("GetOrder", new { id = order.OrderId }, order);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateOrder(int id, OrderUpdateDTO updateDTO)
    {
        if (id <= 0)
        {
            return BadRequest("Id Must Be Positive");
        }

        await _orderService.UpdateOrderAsync(id, updateDTO);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteOrder(int id)
    {
        if (id <= 0)
        {
            return BadRequest("ID Must be Positive");
        }

        await _orderService.DeleteOrderAsync(id);
        return Ok("Delete successfully");
    }

    [Authorize]
    [HttpPut("CancelOrder/{id}")]
    public async Task<ActionResult> CancelOrder(int id)
    {
        if (id <= 0)
        {
            return BadRequest("Id must be positive.");
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        try
        {
            await _orderService.CancelOrderAsync(id, userId, isAdmin);
            return Ok("Order has been canceled.");
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return NotFound("Order not found.");
        }


    }

    [HttpGet("SearchBy{key}")]
    public async Task<ActionResult<IEnumerable<OrderDTO>>> SearchByStatusOrId(string key)
    {
       var reslut =  await _orderService.SearchStatusKeyOrId(key);
        return Ok(reslut);
    }
    [Authorize(Roles = "Admin")]
    [HttpGet("paged")]
    public async Task<ActionResult<PagedResult<OrderDTO>>> GetOrdersPaged(
     [FromQuery] OrderPagination parameters)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _orderService.GetOrdersPagedAsync(parameters);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("user/paged")]
    public async Task<ActionResult<PagedResult<OrderDTO>>> GetUserOrdersPaged(
        [FromQuery] OrderPagination parameters)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _orderService.GetOrdersPagedAsync(parameters, userId);
        return Ok(result);
    }

}