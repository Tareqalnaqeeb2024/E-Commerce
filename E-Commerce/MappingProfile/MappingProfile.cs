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

        }
    }
}
