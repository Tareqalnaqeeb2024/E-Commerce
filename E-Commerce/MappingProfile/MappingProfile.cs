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

            CreateMap<UserAccount, UserDTO>()
                  .ForMember(dest => dest.userId, opt => opt.MapFrom(src => src.Id))
                  .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
                  .ForMember(dest => dest.Roles, opt => opt.Ignore());


            CreateMap<Order, OrderDTO>();
            CreateMap<OrderCreateDTO, Order>();

            CreateMap<Favorite, FavoriteDTO>()
               .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.ProductId))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Product.Name))
               .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price))
               .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.Product.ImageUrl));
               

        }
    }
}
