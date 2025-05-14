
using E_CommerceDataAccess.DTO;
using E_CommerceDataBusiness.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDTO>> GetProduct(int id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            return Ok(product);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
    [HttpGet("DownloadImage/{fileName}")]
    public async Task<IActionResult> DownloadImage(string fileName)
    {
        try
        {
            var (fileStream, contentType) = await _productService.DownloadImageAsync(fileName);
            return new FileStreamResult(fileStream, contentType);
        }
        catch (FileNotFoundException)
        {
            return NotFound("Image not found.");
        }
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ProductDTO>> CreateProduct([FromForm] ProductCreateDTO createDTO)
    {
        var product = await _productService.CreateProductAsync(createDTO);
        return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateProduct(int id, [FromForm] ProductUpdateDTO updateDTO)
    {
        try
        {
            await _productService.UpdateProductAsync(id, updateDTO);
            return Ok("Update Successful");
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        try
        {
            await _productService.DeleteProductAsync(id);
            return Ok("Delete Successful");
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
