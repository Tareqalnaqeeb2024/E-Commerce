using E_CommerceDataAccess.DTO;
using E_CommerceDataBusiness.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDTO>> GetCategory(int id)
    {
        try
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            return Ok(category);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDTO>> CreateCategory(CategoryCreateDTO createDTO)
    {
        var category = await _categoryService.CreateCategoryAsync(createDTO);
        return CreatedAtAction("GetCategory", new { id = category.CategoryId }, category);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCategory(int id, CategoryUpdateDTO updateDTO)
    {
        try
        {
            await _categoryService.UpdateCategoryAsync(id, updateDTO);
            return Ok("Update Successful");
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCategory(int id)
    {
        try
        {
            await _categoryService.DeleteCategoryAsync(id);
            return Ok("Deleted Successfully");
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}