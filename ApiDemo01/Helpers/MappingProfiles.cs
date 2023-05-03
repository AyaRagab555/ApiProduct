using ApiDemo01.Dto;
using AutoMapper;
using Core.Entities;

namespace ApiDemo01.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.ProductType, option => option.MapFrom(src => src.ProductType.Name))
                .ForMember(dest => dest.ProductBrand, option => option.MapFrom(src => src.ProductBrand.Name))
                .ForMember(dest => dest.PictureUrl , option => option.MapFrom<ProductUrlResolver>());

        }
    }
}
