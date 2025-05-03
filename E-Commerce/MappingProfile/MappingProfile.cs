using AutoMapper;
using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.Models;

namespace E_Commerce.MappingProfile
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryDTO>();
            CreateMap<CategoryCreateDTO, Category>();
            CreateMap<CategoryUpdateDTO, Category>();


            CreateMap<Product, ProductDTO>();
            CreateMap<ProductCreateDTO, Product>();
            CreateMap<ProductUpdateDTO, Product>();

            CreateMap<OrderItem, OrderItemDTO>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

            CreateMap<OrderItemCreateDTO, OrderItem>();
            CreateMap<OrderItemUpdateDTO, OrderItem>();


            CreateMap<Order, OrderDTO>();
            CreateMap<OrderCreateDTO, Order>();

        }
    }
}
