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
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProductController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {

            var Products = await _context.Products.ToListAsync();
            if(Products == null)
            {
                return NotFound();
            }
            var ProductsDTO = _mapper.Map<List<ProductDTO>>(Products);

            foreach (var product in ProductsDTO)
            {
                product.ImageUrl = Url.Action("DownLoadImage", new { fileName = product.ImageUrl});
             }
            return Ok(ProductsDTO);
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


        [HttpPut("{Id}")]

        public async Task<ActionResult> UpdateProduct(int Id,  ProductUpdateDTO updateDTO)
        {
            if (Id <= 0)
            {
                return BadRequest("Id must be positive");
            }

            var product = await _context.Products.FindAsync(Id);
            if (product == null)
            {
                return NotFound($"No product with Id: {Id}");
            }


            _mapper.Map(updateDTO, product);
            var uploadDirectory = @"E:\MyUploads";
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                var oldfilepath = Path.Combine(uploadDirectory, product.ImageUrl);
                if (System.IO.File.Exists(oldfilepath))
                {
                    System.IO.File.Delete(oldfilepath);
                }
            }
            var newfilepname = Guid.NewGuid() + Path.GetExtension(updateDTO.ImageFile.FileName);

            var filepath = Path.Combine(uploadDirectory, newfilepname);

            using (var stream = new FileStream(filepath, FileMode.Create))
            {
                await updateDTO.ImageFile.CopyToAsync(stream);

            }
            product.ImageUrl = newfilepname;

            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok("Update Product Successfuly");
        }
        [HttpDelete("{Id}")]
        public async Task<ActionResult> DeleteProduct(int Id)
        {

            var product = await _context.Products.FindAsync(Id);

            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok("Delete SuccessFuly");
        }


    }
}
