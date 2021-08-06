using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MixLoginProduct.Authentication;
using MixLoginProduct.Interface;
using MixLoginProduct.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MixLoginProduct.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        
        private readonly ICartRepo _cartRepo;
        public static IWebHostEnvironment _environment;

        public ShoppingCartController(IWebHostEnvironment environment, ICartRepo cartRepo)
        {
            _environment = environment;
            _cartRepo = cartRepo;
        }



        [HttpGet]
        public async Task<ActionResult<PagedList<IEnumerable<returnCartItem>>>> GetCart()
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                string username = User.FindFirstValue(ClaimTypes.Name);
                List<returnCartItem> cart = await _cartRepo.GetCarts(username);
                return Ok(cart);
            }
            return Unauthorized();
        }

        [HttpPost]
        public async Task<ActionResult> AddCart([FromForm] ShoppingCartItem sc)
        {

            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                //IEnumerable<Claim> claims = identity.Claims;
                string username = User.FindFirstValue(ClaimTypes.Name);
                
                ShoppingCart newShoppingCart = new ShoppingCart();
                newShoppingCart.UserId = username;
                newShoppingCart.ProductId = sc.productId;
                newShoppingCart.Quantity = sc.Quantity;
                await _cartRepo.AddCart(newShoppingCart);
                return Ok();
            }

            return Unauthorized();
        }

        [HttpPut]
        [Route("Delete")]
        public async Task<ActionResult> DeleteCart([FromBody] List<ProductIdOnly> product)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                //IEnumerable<Claim> claims = identity.Claims;
                string username = User.FindFirstValue(ClaimTypes.Name);
                await _cartRepo.DeleteCart(product, username);
                return Ok();
            }
            
            return Unauthorized();
        }

        [HttpPut]
        [Route("PayCart")]
        public async Task<ActionResult<List<Response>>> PayCart([FromBody] List<ShoppingCartItem> shoppingCartItems)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                //IEnumerable<Claim> claims = identity.Claims;
                string username = User.FindFirstValue(ClaimTypes.Name);
                List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();
                foreach (ShoppingCartItem shoppingCartItem in shoppingCartItems)
                {
                    ShoppingCart newShoppingCart = new ShoppingCart();
                    newShoppingCart.UserId = username;
                    newShoppingCart.ProductId = shoppingCartItem.productId;
                    newShoppingCart.Quantity = shoppingCartItem.Quantity;
                    shoppingCarts.Add(newShoppingCart);
                }
                List<Response> responses = await _cartRepo.PayCart(shoppingCarts);
                return Ok(responses);
            }
            return Unauthorized();
            
        }
    }
}
