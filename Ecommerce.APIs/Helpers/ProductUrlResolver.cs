using AutoMapper;
using Ecommerce.APIs.Dtos;
using Ecommerce.Core.Entities;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.APIs.Helpers
{
    public class ProductUrlResolver : IValueResolver<Product, ProductToReturnDto, string>
    {
        private readonly IConfiguration _config;

        public ProductUrlResolver(IConfiguration config)
        {
            _config = config;
        }

        public string Resolve(Product source, ProductToReturnDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.PictureUrl))
            {
                //  √ﬂœ ≈‰ ApiUrl „ÊÃÊœ ›Ì appsettings.json
                return _config["ApiUrl"] + source.PictureUrl;
            }
            return null;
        }
    }
}