using ApiDemo01.Dto;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace ApiDemo01.Controllers
{
    public class ProductsController : BaseController
    {
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<ProductType> _productTypeRepository;
        private readonly IGenericRepository<ProductBrand> _productBrandRepository;
        private readonly IMapper _mapper;

        // private readonly IProductRepository _productRepository;

        public ProductsController(//IProductRepository productRepository
            IGenericRepository<Product> productRepository,
            IGenericRepository<ProductType> productTypeRepository,
            IGenericRepository<ProductBrand> productBrandRepository,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _productTypeRepository = productTypeRepository;
            _productBrandRepository = productBrandRepository;
            _mapper = mapper;
            //_productRepository = productRepository;
        }

        [HttpGet("GetProducts")]
        public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetProducts([FromQuery]ProductSpecParams productSpecParams)
        {
            var specs = new ProductsWithTypeAndBrandSpecifications(productSpecParams);
            var products = await _productRepository.GetListAsync(specs);
            var mappedProducts = _mapper.Map<IReadOnlyList<ProductDto>>(products);
            return Ok(mappedProducts);
        }
        [HttpGet("GetProduct")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var specs = new ProductsWithTypeAndBrandSpecifications(id);
            var Product = await _productRepository.GetEntityWithSpecifications(specs);
            if(Product == null)
                return NotFound();
            var mappedProducts = _mapper.Map<ProductDto>(Product);

            return Ok(mappedProducts); 
             
        }
        [HttpGet("Brands")]
        public async Task<ActionResult<ProductBrand>> GetBrands()
           => Ok(await _productBrandRepository.ListAllAsync());

        [HttpGet("Types")]
        public async Task<ActionResult<ProductType>> GetTypes()
          => Ok(await _productTypeRepository.ListAllAsync());
    }
}
 