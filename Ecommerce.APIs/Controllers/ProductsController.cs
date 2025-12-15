using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces;
using Ecommerce.APIs.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        // 1. Get All Products
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts()
        {
            var products = await _unitOfWork.Repository<Product>().ListAllAsync();

            // تحويل الداتا لـ DTO عشان شكل الـ JSON والـ URL بتاع الصور يظبط
            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

            return Ok(data);
        }

        // 2. Get Product By Id
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);

            if (product == null) return NotFound();

            return _mapper.Map<Product, ProductToReturnDto>(product);
        }

        // 3. Get Brands (عشان الفلتر)
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            var brands = await _unitOfWork.Repository<ProductBrand>().ListAllAsync();
            return Ok(brands);
        }

        // 4. Get Types (عشان الفلتر)
        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
        {
            var types = await _unitOfWork.Repository<ProductType>().ListAllAsync();
            return Ok(types);
        }
    }
}