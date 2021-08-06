using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using MixLoginProduct.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MixLoginProduct.Interface
{
    public class IProductService : IProductRepo
    {
        private readonly EcommerceContext _context;
        public static IWebHostEnvironment _environment;

        public IProductService(EcommerceContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public async Task<PagedList<List<Product>>> GetActiveProducts(PaginationFilter paginationFilter)
        {
            var pagedata = await _context.Products.Where(x=>x.Status==1).Skip((paginationFilter.PageNumber-1)*paginationFilter.PageSize)
                .Take(paginationFilter.PageSize).ToListAsync();
            var totalRecords = await _context.Products.CountAsync();

            return new PagedList<List<Product>>(pagedata, paginationFilter.PageNumber, paginationFilter.PageSize, totalRecords);
        }

        public async Task<PagedList<List<ProductWithStatusName>>> GetProducts(PaginationFilter paginationFilter)
        {
            var pagedata = await _context.Products.Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                .Take(paginationFilter.PageSize).Select(x=> new ProductWithStatusName { 
                ProductId=x.ProductId,
                ProductName=x.ProductName,
                Price=x.Price,
                Description=x.Description,
                Image=x.Image,
                Status= x.StatusNavigation.StatusName,
                Stock=x.Stock
                }).ToListAsync();
            var totalRecords = await _context.Products.CountAsync();

            return new PagedList<List<ProductWithStatusName>>(pagedata, paginationFilter.PageNumber, paginationFilter.PageSize, totalRecords);
        }

        public async Task<Product> GetProductById(int id)
        {
            var product = await _context.Products.FindAsync(id);
            return product;
        }
        public async Task AddProduct(ProductAddModel product)
        {
            Product newProduct = new Product();
            if (product.Image != null)
            {
                
                var fileName = Path.GetFileName(product.Image.FileName);
                var filePath = Path.Combine(_environment.WebRootPath, "images\\Product\\", fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                
                {
                    await product.Image.CopyToAsync(fileStream);
                }

                newProduct.ProductName = product.ProductName;
                newProduct.Price = product.Price;
                newProduct.Description = product.Description;
                newProduct.Image = fileName;
                newProduct.Stock = product.Stock;
                newProduct.Status = product.Status;

                _context.Products.Add(newProduct);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateProduct(ProductUpdateRequest productUpdate)
        {
            if (_context.Products.Any(x => x.ProductId == productUpdate.ProductId))
            {
                Product updatedProduct = new Product();
                if (productUpdate.ImageFile != null)
                {
                    var a = _environment.WebRootPath;
                    var fileName = Path.GetFileName(productUpdate.ImageFile.FileName);
                    var filePath = Path.Combine(_environment.WebRootPath, "images\\Product\\", fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    //using(FileStream fileStream=System.IO.File.Create(_environment.WebRootPath+"\\images\\Product\\"+fileName))
                    {
                        await productUpdate.ImageFile.CopyToAsync(fileStream);
                    }
                    productUpdate.Image = fileName;
                }
                updatedProduct.ProductId = productUpdate.ProductId;
                updatedProduct.ProductName = productUpdate.ProductName;
                updatedProduct.Price = productUpdate.Price;
                updatedProduct.Description = productUpdate.Description;
                updatedProduct.Image = productUpdate.Image;
                updatedProduct.Stock = productUpdate.Stock;
                updatedProduct.Status = productUpdate.Status;
                if (updatedProduct.Stock == 0)
                {
                    updatedProduct.Status = 2;
                }
                
                _context.Entry(updatedProduct).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }
        }
        public async Task DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductStatus>> GetStatus()
        {
            List<ProductStatus> productStatuses = _context.ProductStatuses.ToList();
            return productStatuses;
        }






    }
}
