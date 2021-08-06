using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MixLoginProduct.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using static MixLoginProduct.Models.Product;
using Microsoft.AspNetCore.Authorization;
using MixLoginProduct.Authentication;
using MixLoginProduct.Interface;
using System.Security.Claims;

namespace MixLoginProduct.Controllers
{
    [Authorize]
    
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly EcommerceContext _context;
        public static IWebHostEnvironment _environment;
        private readonly IProductRepo _productRepo;

        public ProductsController(EcommerceContext context, IWebHostEnvironment environment,IProductRepo productRepo)
        {
            _context = context;
            _environment = environment;
            _productRepo = productRepo;
        }

        // GET: api/Products
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery] PaginationFilter paginationFilter )
        //{
        //    IEnumerable<Product> productList = await _productRepo.GetProducts(paginationFilter);
        //    return Ok(productList);
        //}

        [HttpGet]
        public async Task<ActionResult<PagedList<List<ProductWithStatusName>>>> GetProducts([FromQuery] PaginationFilter paginationFilter)
        {
            PagedList<List<ProductWithStatusName>> pagedList = await _productRepo.GetProducts(paginationFilter);
            return pagedList;
        }
            // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            Product p = await _productRepo.GetProductById(id);
            if (p == null)
            {
                return NotFound();
            }
            return Ok(p);

        }

        [HttpGet]
        [Route("ActiveProducts")]
        public async Task<ActionResult<PagedList<List<Product>>>> GetActiveProducts([FromQuery] PaginationFilter paginationFilter)
        {
            PagedList<List<Product>> pagedList = await _productRepo.GetActiveProducts(paginationFilter);
            return pagedList;
        }




        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct([FromForm] ProductUpdateRequest productUpdate)
        {

            await _productRepo.UpdateProduct(productUpdate);
            return Ok();
        }

        // POST: api/Products
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<ActionResult> PostProduct([FromForm] ProductAddModel product)
        {
            await _productRepo.AddProduct(product);
            return Ok();
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            await _productRepo.DeleteProduct(id);

            return Ok();
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        [Route("validate")]
        public async Task<ActionResult> Validate()
        {
            return Ok();
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        [Route("GetStatus")]
        public async Task<ActionResult> GetStatus()
        {
            List<ProductStatus> productStatuses = await _productRepo.GetStatus();
            return Ok(productStatuses);
        }

    }
}

