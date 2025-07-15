using AutoMapper;
using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.DTO.Common;
using E_CommerceDataAccess.DTO.Pagination;
using E_CommerceDataAccess.Interfaces;
using E_CommerceDataAccess.Models;
using E_CommerceDataBusiness.Interfaces;
using E_CommerceDataBusiness.Interfaces.ExternalInterface;
using Newtonsoft.Json;

public class ProductService :IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorageService;
    private readonly IRedisService _redisCache;
    private const string KeyPrefix = "product:";

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper,
        IFileStorageService fileStorageService,
        IRedisService redisService)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _fileStorageService = fileStorageService;
        _redisCache = redisService;
    }

    public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
    {
         string cacheKey = $"{KeyPrefix}all";

        var cachedProducts = await _redisCache.GetAsync<IEnumerable<ProductDTO>>(cacheKey);
        if (cachedProducts != null)
        {
            return cachedProducts;
        }

        var products = await _productRepository.GetAllWithCategoryAsync();
        var productsDto = _mapper.Map<List<ProductDTO>>(products);

        foreach (var product in productsDto)
        {
            product.ImageUrl = _fileStorageService.GenerateFileUrl(product.ImageUrl);
        }

        await _redisCache.SetAsync(cacheKey, productsDto, TimeSpan.FromMinutes(30));

        return productsDto;
    }

    public async Task<ProductDTO> GetProductByIdAsync(int id)
    {
        string cacheKey = $"{KeyPrefix}{id}";

        var cachedProduct = await _redisCache.GetAsync<ProductDTO>(cacheKey);
        if (cachedProduct != null)
        {
            return cachedProduct;
        }
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

        await _redisCache.SetAsync(cacheKey, productDto, TimeSpan.FromMinutes(30));

        return productDto;
    }

    public async Task<ProductDTO> CreateProductAsync(ProductCreateDTO createDTO)
    {
        var product = _mapper.Map<Product>(createDTO);
        product.ImageUrl = await _fileStorageService.SaveFileAsync(createDTO.ImageFile);

        var createdProduct = await _productRepository.AddAsync(product);
        var productDto = _mapper.Map<ProductDTO>(createdProduct);
        productDto.ImageUrl = _fileStorageService.GenerateFileUrl(productDto.ImageUrl);

        await _redisCache.RemoveAsync($"{KeyPrefix}all");
        await _redisCache.RemoveAsync($"{KeyPrefix}paged:*");

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

        await _redisCache.RemoveAsync($"{KeyPrefix}{id}");
        await _redisCache.RemoveAsync($"{KeyPrefix}all");
        await _redisCache.RemoveAsync($"{KeyPrefix}paged:*");
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) throw new KeyNotFoundException("Product not found");

        await _fileStorageService.DeleteFileAsync(product.ImageUrl);
        await _productRepository.DeleteAsync(id);
         await _redisCache.RemoveAsync($"{KeyPrefix}{id}");
    await _redisCache.RemoveAsync($"{KeyPrefix}all");
    await _redisCache.RemoveAsync($"{KeyPrefix}paged:*");
    }

    public async Task<(Stream FileStream, string ContentType)> DownloadImageAsync(string fileName)
    {
        return await _fileStorageService.GetFileAsync(fileName);
    }

    public async Task<IEnumerable<ProductDTO>> GetProductsWithCategoriesAsync(string categoryname)
    {
        var products = await _productRepository.GetAllWithCategoryNameAsync(categoryname);

        var produtdto = _mapper.Map<List<ProductDTO>>(products);

        foreach (var product in produtdto)
        {
            product.ImageUrl = _fileStorageService.GenerateFileUrl(product.ImageUrl);
        }
        return produtdto;
    }

    public async Task<IEnumerable<ProductDTO>> GetAvailableProductsAsync()
    {
        var products = await _productRepository.GetAvailableProductsAsync();
        var productDtos = _mapper.Map<List<ProductDTO>>(products);

        foreach (var product in productDtos)
        {
            product.ImageUrl = _fileStorageService.GenerateFileUrl(product.ImageUrl);
        }

        return productDtos;
    }

    public async Task<IEnumerable<ProductDTO>> SearchProductsAsync(string keyword)
    {
        var products = await _productRepository.SearchByNameOrDescriptionAsync(keyword);
        var productDtos = _mapper.Map<List<ProductDTO>>(products);

        foreach (var product in productDtos)
        {
            product.ImageUrl = _fileStorageService.GenerateFileUrl(product.ImageUrl);
        }

        return productDtos;
    }

    public async Task UpdateProductPriceAsync(int id, decimal newPrice)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) throw new KeyNotFoundException("Product not found");

        product.Price = newPrice;
        await _productRepository.UpdateAsync(product);
    }

    public async Task<PagedResult<ProductDTO>> GetProductsPagedAsync(ProductPagination parameters)
    {
        string cacheKey = $"{KeyPrefix}paged:{JsonConvert.SerializeObject(parameters)}";
        var cachedResult = await _redisCache.GetAsync<PagedResult<ProductDTO>>(cacheKey);

        if (cachedResult != null)
        {
            return cachedResult;
        }

        var pagedResult = await _productRepository.GetPagedProductsAsync(parameters);
        var productDtos = _mapper.Map<List<ProductDTO>>(pagedResult.Items);

        // Process image URLs
        foreach (var product in productDtos)
        {
            product.ImageUrl = _fileStorageService.GenerateFileUrl(product.ImageUrl);
        }

        var result = new PagedResult<ProductDTO>
        {
            Items = productDtos,
            TotalCount = pagedResult.TotalCount,
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize
        };

        await _redisCache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(30));

        return result;
    }


}