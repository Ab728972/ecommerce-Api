using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Ecommerce.APIs.Dtos;
using Ecommerce.APIs.Helpers;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces;
using Ecommerce.Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        
        [HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery] ProductSpecParams productParams)
        {
            
            var spec = new ProductsWithTypesAndBrandsSpecification(productParams);

            
            var countSpec = new ProductWithFiltersForCountSpecification(productParams);

            var totalItems = await _unitOfWork.Repository<Product>().CountAsync(countSpec);

            
            var products = await _unitOfWork.Repository<Product>().ListAsync(spec);

            
            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

            
            return Ok(new Pagination<ProductToReturnDto>(productParams.PageIndex, productParams.PageSize, totalItems, data));
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(id);
            var product = await _unitOfWork.Repository<Product>().GetEntityWithSpec(spec);

            if (product == null) return NotFound();

            return _mapper.Map<Product, ProductToReturnDto>(product);
        }

        
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            return Ok(await _unitOfWork.Repository<ProductBrand>().ListAllAsync());
        }

        
        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
        {
            return Ok(await _unitOfWork.Repository<ProductType>().ListAllAsync());
        }
    }
}