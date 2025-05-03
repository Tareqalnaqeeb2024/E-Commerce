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
    public class OrderItemController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public OrderItemController( AppDbContext context , IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("{Id}")]

        public async Task<ActionResult<OrderItemDTO>> GetOrderItem(int Id)
        {
            if (Id <= 0)
            {
                return BadRequest($"{Id} Must be Positive");
            }

            var OrderItem = await _context.Items
                .Include(p => p.Product)
                .FirstOrDefaultAsync(o => o.OrderItemId == Id);

            if (OrderItem == null)
            {
                return NotFound("No OrderItem Found");
            }

            return Ok(_mapper.Map<OrderItemDTO>(OrderItem));
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrderItem(OrderItemCreateDTO orderItemCreate)
        {

            var orderitem = _mapper.Map<OrderItem>(orderItemCreate);

            _context.Items.Add(orderitem);
            await _context.SaveChangesAsync();


            var createdItem = await _context.Items
                .Include(oi => oi.Product)
                .FirstOrDefaultAsync(oi => oi.OrderItemId == orderitem.OrderItemId);

            //var orderItemDTO = _mapper.Map<OrderItemDTO>(createdItem);

            return CreatedAtAction("GetOrderItem", new { Id = createdItem.OrderItemId },  _mapper.Map<OrderItemDTO>(createdItem));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItemDTO>>> GetAllOrderItems()
        {
            var orderItems = await _context.Items.ToListAsync();

            if (orderItems == null)
            {
                return NotFound();

            }

            return Ok(_mapper.Map<List<OrderItemDTO>>( orderItems));
        }

        [HttpPut("{Id}")]
        public async Task<ActionResult> UpdateOrderItem(int Id, OrderItemUpdateDTO orderItemUpdate)
        {
            if (Id <= 0)
            {
                return BadRequest("Id must be positive");
            }

            var orderitem = await _context.Items.FindAsync(Id);

            if (orderitem == null)
            {
                return NotFound();
            }

            _mapper.Map(orderItemUpdate, orderitem);

            _context.Entry(orderitem).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(" Update OrderItems Successfluy");
        }


        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteOrderItem(int Id)
        {
            if (Id <= 0)
            {
                return BadRequest("ID must be positive");
            }

            var orderItem = await _context.Items.FindAsync(Id);
            if (orderItem == null)
            {
                return NotFound($"No order item found with ID: {Id}");
            }

            _context.Items.Remove(orderItem);
            await _context.SaveChangesAsync();

            return Ok("Deleted Orderitem Successfuly");
        }



        private bool OrderItemExists(int Id)
        {
            return _context.Items.Any(e => e.OrderItemId == Id);
        }

    }
}
