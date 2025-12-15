using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Ecommerce.APIs.Dtos;
using Ecommerce.APIs.Helpers; // عشان الـ CachedAttribute و Pagination
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

        // 1. Get All Products (مع الكاش والفلترة والصفحات)
        [HttpGet]
        [Cached(600)] // دي الإضافة بتاعة سيشن 4 (الكاش لمدة 10 دقايق)
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery] ProductSpecParams productParams)
        {
            // مواصفات الداتا (ترتيب، بحث، فلترة)
            var spec = new ProductsWithTypesAndBrandsSpecification(productParams);

            // مواصفات العدد (عشان الـ Pagination يعرف في كام صفحة)
            var countSpec = new ProductWithFiltersForCountSpecification(productParams);
            var totalItems = await _unitOfWork.Repository<Product>().CountAsync(countSpec);

            // تنفيذ الكويري
            var products = await _unitOfWork.Repository<Product>().ListAsync(spec);

            // تحويل لـ DTO
            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

            // إرجاع النتيجة
            return Ok(new Pagination<ProductToReturnDto>(productParams.PageIndex, productParams.PageSize, totalItems, data));
        }

        // 2. Get Product By Id (ممكن تضيف [Cached(600)] هنا كمان لو حابب)
        [HttpGet("{id}")]
        [Cached(600)] // يفضل نضيف الكاش هنا كمان عشان الأداء
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            // بنستخدم Spec عشان نجيب بيانات الماركة والنوع مع المنتج
            var spec = new ProductsWithTypesAndBrandsSpecification(id);
            var product = await _unitOfWork.Repository<Product>().GetEntityWithSpec(spec);

            if (product == null) return NotFound();

            return _mapper.Map<Product, ProductToReturnDto>(product);
        }

        // 3. Get Brands
        [HttpGet("brands")]
        [Cached(600)] // الماركات مش بتتغير كتير، الكاش هنا مفيد جداً
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            return Ok(await _unitOfWork.Repository<ProductBrand>().ListAllAsync());
        }

        // 4. Get Types
        [HttpGet("types")]
        [Cached(600)] // الأنواع كمان مش بتتغير كتير
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
        {
            return Ok(await _unitOfWork.Repository<ProductType>().ListAllAsync());
        }
    }
}