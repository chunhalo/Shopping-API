using MixLoginProduct.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MixLoginProduct.Interface
{
    public interface IProductRepo
    {
        //Task<IEnumerable<Product>> GetProducts(PaginationFilter paginationFilter);
        Task<PagedList<List<ProductWithStatusName>>> GetProducts(PaginationFilter paginationFilter);
        Task<PagedList<List<Product>>> GetActiveProducts(PaginationFilter paginationFilter);

        Task<Product> GetProductById(int id);
        Task AddProduct(ProductAddModel product);
        Task UpdateProduct(ProductUpdateRequest productUpdate);
        Task DeleteProduct(int id);
        Task<List<ProductStatus>> GetStatus();



    }
}
