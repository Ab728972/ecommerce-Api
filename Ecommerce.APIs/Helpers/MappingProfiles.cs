using AutoMapper;
using Ecommerce.APIs.Dtos;
using Ecommerce.Core.Entities;

namespace Ecommerce.APIs.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDto>()
            CreateMap<Ecommerce.Core.Entities.Identity.Address, AddressDto>().ReverseMap();
                .ForMember(d => d.ProductBrand, o => o.MapFrom(s => s.ProductBrand.Name))
                .ForMember(d => d.ProductType, o => o.MapFrom(s => s.ProductType.Name))
                .ForMember(d => d.PictureUrl, o => o.MapFrom<ProductUrlResolver>());
        }
    }
}