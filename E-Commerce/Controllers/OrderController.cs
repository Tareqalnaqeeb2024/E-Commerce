using AutoMapper;
using E_CommerceDataAccess.Data;
using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public OrderController(AppDbContext context , IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrder()
        {
            var Orders = await _context.Orders
                  .Include(o => o.OrderItems)
                  .ThenInclude(o => o.Product)
                  .ToListAsync();
            var orderDTOs = _mapper.Map<List<OrderDTO>>(Orders);
            return Ok(orderDTOs);

        }

        [HttpGet("{Id}")]

        public async Task<ActionResult<OrderDTO>> GetOrder(int Id)
        {
            var order = await _context.Orders
                .Include (o => o.OrderItems)
                .ThenInclude (o => o.Product)
                .FirstOrDefaultAsync(o => o.OrderId == Id);

            if( order == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<OrderDTO>(order));
        }

        [HttpPost]
        public async Task< ActionResult< OrderDTO>> CreateOder(OrderCreateDTO orderCreate)
        {

            var order = _mapper.Map<Order>(orderCreate);

            // Auto-set values
            order.OrderDate = DateTime.UtcNow;
            order.Status = "Pending"; // Redundant if set in entity, but explicit

            // Calculate total
            order.TotalAmount = orderCreate.OrderItems.Sum(oi => oi.Price * oi.Quentity);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var createdOrder = await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);


            return CreatedAtAction("GetOrder", new { Id = order.OrderId }, _mapper.Map<OrderDTO>(createdOrder));


        }
    }
}
