using E_CommerceDataAccess.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataBusiness.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
        Task<ProductDTO> GetProductByIdAsync(int id);
        Task<ProductDTO> CreateProductAsync(ProductCreateDTO createDTO);
        Task UpdateProductAsync(int id, ProductUpdateDTO updateDTO);
        Task DeleteProductAsync(int id);
        Task<(Stream FileStream, string ContentType)> DownloadImageAsync(string fileName);
    }
}
