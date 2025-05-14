using AutoMapper;
using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.Interfaces;
using E_CommerceDataAccess.Models;
using E_CommerceDataBusiness.Interfaces;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorageService;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper,
        IFileStorageService fileStorageService)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _fileStorageService = fileStorageService;
    }

    public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllWithCategoryAsync();
        var productsDto = _mapper.Map<List<ProductDTO>>(products);

        foreach (var product in productsDto)
        {
            product.ImageUrl = _fileStorageService.GenerateFileUrl(product.ImageUrl);
        }

        return productsDto;
    }

    public async Task<ProductDTO> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdWithCategoryAsync(id);
        if (product == null) throw new KeyNotFoundException("Product not found");

        var productDto = _mapper.Map<ProductDTO>(product);
        productDto.ImageUrl = _fileStorageService.GenerateFileUrl(product.ImageUrl);

        try
        {
            var (fileStream, _) = await _fileStorageService.GetFileAsync(product.ImageUrl);
            using (var memoryStream = new MemoryStream())
            {
                await fileStream.CopyToAsync(memoryStream);
                productDto.ImageBase64 = Convert.ToBase64String(memoryStream.ToArray());
            }
        }
        catch (FileNotFoundException)
        {
            productDto.ImageBase64 = null;
        }

        return productDto;
    }

    public async Task<ProductDTO> CreateProductAsync(ProductCreateDTO createDTO)
    {
        var product = _mapper.Map<Product>(createDTO);
        product.ImageUrl = await _fileStorageService.SaveFileAsync(createDTO.ImageFile);

        var createdProduct = await _productRepository.AddAsync(product);
        var productDto = _mapper.Map<ProductDTO>(createdProduct);
        productDto.ImageUrl = _fileStorageService.GenerateFileUrl(productDto.ImageUrl);

        return productDto;
    }

    public async Task UpdateProductAsync(int id, ProductUpdateDTO updateDTO)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) throw new KeyNotFoundException("Product not found");

        _mapper.Map(updateDTO, product);

        if (updateDTO.ImageFile != null)
        {
            await _fileStorageService.DeleteFileAsync(product.ImageUrl);
            product.ImageUrl = await _fileStorageService.SaveFileAsync(updateDTO.ImageFile);
        }

        await _productRepository.UpdateAsync(product);
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) throw new KeyNotFoundException("Product not found");

        await _fileStorageService.DeleteFileAsync(product.ImageUrl);
        await _productRepository.DeleteAsync(id);
    }

    public async Task<(Stream FileStream, string ContentType)> DownloadImageAsync(string fileName)
    {
        return await _fileStorageService.GetFileAsync(fileName);
    }
}