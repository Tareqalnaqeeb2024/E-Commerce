using AutoMapper;
using E_CommerceDataAccess.Data;
using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProductController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("{Id}")]

        public async Task<ActionResult<ProductDTO>> GetProduct(int Id)
        {
            if (Id <= 0)
            {
                return BadRequest("Id Must be Positive");
            }
            var product = await _context.Products.FindAsync(Id);
            if (product == null)
            {
                return NotFound($"No Product with Id: {Id}");
            }

            var productDto = _mapper.Map<ProductDTO>(product);


            var UploadDirctory = @"E:\MyUploads";
            var FilePath = Path.Combine(UploadDirctory, product.ImageUrl);


            productDto.ImageUrl = Url.Action("DownloadImage", new { fileName = product.ImageUrl });

            if (!System.IO.File.Exists(FilePath))
            {
                byte[] imageBytes = await System.IO.File.ReadAllBytesAsync(FilePath);
                productDto.ImageBase64 = Convert.ToBase64String(imageBytes);
            }
            else
            {
                productDto.ImageBase64 = null;
            }
            return Ok(productDto);

            // return Ok(productDto);
        }

        [HttpGet("DownloadImage/{fileName}")]
        public IActionResult DownloadImage(string fileName)
        {
            var uploadDirectory = @"E:\MyUploads";
            var filePath = Path.Combine(uploadDirectory, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("Image not found.");

            var image = System.IO.File.OpenRead(filePath);
            var mimeType = GetMimeType(filePath);

            return File(image, mimeType); // Returns the file for download
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ProductDTO>> CreateProduct([FromForm] ProductCreateDTO productCreateDTO)
        {

            var Product = _mapper.Map<Product>(productCreateDTO);

            if(productCreateDTO.ImageFile != null && productCreateDTO.ImageFile.Length > 0)

            {
                var FileName = Guid.NewGuid() +  Path.GetExtension(productCreateDTO.ImageFile.FileName);

                var uploadDirectory = @"E:\MyUploads";
                var FilePath = Path.Combine(uploadDirectory, FileName);

                //var directorypath = Path.GetDirectoryName(FilePath);

                if (!Directory.Exists(uploadDirectory))
                {
                    Directory.CreateDirectory(uploadDirectory);
                }
                using (var stream = new FileStream(FilePath,FileMode.Create))
                {
                    await productCreateDTO.ImageFile.CopyToAsync(stream);
                }

         
            Product.ImageUrl = FileName;
            }

            _context.Products.Add(Product);
            await _context.SaveChangesAsync();
            var productDto = _mapper.Map<ProductDTO>(Product);

            return CreatedAtAction("GetProduct", new { Id = Product.ProductId }, productDto);

        }

        private string GetMimeType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                _ => "application/octet-stream"
            };
        }


    }
}
