using MixLoginProduct.Authentication;
using MixLoginProduct.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MixLoginProduct.Interface
{
    public interface ICartRepo
    {
        Task<List<returnCartItem>> GetCarts(string username);
        Task AddCart(ShoppingCart newCart);
        Task UpdateCart(ShoppingCart updateCart);
        Task DeleteCart(List<ProductIdOnly> products, string username);
        Task<List<Response>> PayCart(List<ShoppingCart> shoppingCarts);

    }
}
