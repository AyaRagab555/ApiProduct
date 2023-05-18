using ApiDemo01.Dto;
using ApiDemo01.Helpers;
using ApiDemo01.ResponseModule;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace ApiDemo01.Controllers
{
    public class ProductsController : BaseController
    {
        //private readonly IGenericRepository<Product> _productRepository;
        //private readonly IGenericRepository<ProductType> _productTypeRepository;
        //private readonly IGenericRepository<ProductBrand> _productBrandRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        // private readonly IProductRepository _productRepository;

        public ProductsController(//IProductRepository productRepository
            //IGenericRepository<Product> productRepository,
            //IGenericRepository<ProductType> productTypeRepository,
            //IGenericRepository<ProductBrand> productBrandRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            //_productRepository = productRepository;
            //_productTypeRepository = productTypeRepository;
            //_productBrandRepository = productBrandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            //_productRepository = productRepository;
        }
        [Cached(100)]
        [HttpGet("GetProducts")]
        public async Task<ActionResult<Pagination<ProductDto>>> GetProducts([FromQuery]ProductSpecParams productSpecParams)
        {
            var specs = new ProductsWithTypeAndBrandSpecifications(productSpecParams);
            var countSpec = new ProductWithFiltersForCountSpecifications(productSpecParams);
            var totalItem = await _unitOfWork.Repository<Product>().CountAsync(specs);
            var products = await _unitOfWork.Repository<Product>().GetListAsync(specs);
            var mappedProducts = _mapper.Map<IReadOnlyList<ProductDto>>(products);
            var paginatedData = new Pagination<ProductDto>(productSpecParams.PageIndex, productSpecParams.PageSize,totalItem ,mappedProducts);
            return Ok(paginatedData);
        }

        [Cached(100)]
        [HttpGet("GetProduct")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce),StatusCodes.Status404NotFound)]

        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var specs = new ProductsWithTypeAndBrandSpecifications(id);
            var Product = await _unitOfWork.Repository<Product>().GetEntityWithSpecifications(specs);
            if(Product == null)
                return NotFound(new ApiResponce(404));
            var mappedProducts = _mapper.Map<ProductDto>(Product);

            return Ok(mappedProducts); 
             
        }
        [Cached(100)]
        [HttpGet("Brands")]
        public async Task<ActionResult<ProductBrand>> GetBrands()
           => Ok(await _unitOfWork.Repository<ProductBrand>().ListAllAsync());

        [Cached(100)]
        [HttpGet("Types")]
        public async Task<ActionResult<ProductType>> GetTypes()
          => Ok(await _unitOfWork.Repository<ProductType>().ListAllAsync());
    }
}
 