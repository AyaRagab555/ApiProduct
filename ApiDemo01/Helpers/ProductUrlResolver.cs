using ApiDemo01.Dto;
using AutoMapper;
using Core.Entities;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ApiDemo01.Helpers
{
    public class ProductUrlResolver : IValueResolver<Product , ProductDto,string >
    {
        private readonly IConfiguration _configuration;

        public ProductUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Resolve(Product source, ProductDto destination, string destMember, ResolutionContext context)
        {
            if(!string.IsNullOrEmpty(source.PictureUrl))
                return _configuration["ApiUrl"] + source.PictureUrl;

            return null;
             
        }
    }

}
