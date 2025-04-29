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
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CategoryController(AppDbContext context , IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            var Categories = await _context.Categories.ToListAsync();

            return Ok(_mapper.Map<IEnumerable<CategoryDTO>>(Categories));
        }
        [HttpGet("{Id}")]
        public async Task<ActionResult<CategoryDTO>> GetCategory(int Id)
        {
            var category = await _context.Categories.FindAsync(Id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CategoryDTO>(category));
        }
        
        [HttpPost]
        public async Task<ActionResult<CategoryDTO>> CreateCategory(CategoryCreateDTO createDTO)
        {
            var category = _mapper.Map<Category>(createDTO);

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var categoryDTO = _mapper.Map<CategoryDTO>(category);

            return CreatedAtAction("GetCategory", new { id = category.CategoryId }, categoryDTO);
        }

        [HttpPut("{Id}")]
        public async Task<ActionResult> UpdateCategory(int Id, CategoryUpdateDTO updateDTO)
        {
            if (Id <= 0)
            {
                return BadRequest("Id Must Be Positive");
            }
            var category = await _context.Categories.FindAsync(Id);
            if (category == null)
            {
                return NotFound();
            }

            _mapper.Map(updateDTO, category);
            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok("Update Successfuly");
        }

        [HttpDelete("{Id}")]

        public async Task<ActionResult> DeleteCategory(int Id)
        {
            if (Id <= 0)
            {
                return BadRequest("Id Must be Positive");
            }
            var category = await _context.Categories.FindAsync(Id);
            if (category == null)
            {
                return NotFound();
            }

            bool HasProducts = await _context.Products.AnyAsync(p => p.CategoryId == Id);

            if (HasProducts)
            {
                return BadRequest("Cannot delete Category with Releated Products");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return Ok("Deleted Successfuly");
        }
    }
}
