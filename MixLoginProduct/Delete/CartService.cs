using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MixLoginProduct.Authentication;
using MixLoginProduct.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MixLoginProduct.Interface
{
    public class CartService : ICartRepo
    {
        private readonly EcommerceContext _context;
        

        public CartService(EcommerceContext context)
        {
            _context = context;
        }

        public async Task AddCart(ShoppingCart newCart)
        {
            var getUserId = _context.AspNetUsers.Where(x => x.UserName == newCart.UserId).Select(x => x.Id).FirstOrDefault();
            newCart.UserId = getUserId;

            if (_context.ShoppingCarts.Any(x => x.UserId == newCart.UserId && x.ProductId == newCart.ProductId))
            {
                await UpdateCart(newCart);
            }
            else
            {
                _context.ShoppingCarts.Add(newCart);
                await _context.SaveChangesAsync();
            } 
        }

        public async Task<List<returnCartItem>> GetCarts(string username)
        {
            var getUserId = _context.AspNetUsers.Where(x => x.UserName == username).Select(x => x.Id).FirstOrDefault();
            List<returnCartItem> getcarts = _context.ShoppingCarts.Where(x => x.UserId == getUserId).Include(x => x.Product).
                Select(x => new returnCartItem { product = x.Product, Quantity = x.Quantity, isSelected = false }).ToList();
            return getcarts;

        }

        // _context.ShoppingCarts.Include(x => x.Product).Where(x => x.UserId == getUserId).ToList();

        //List<returnCartItem> returncart = new List<returnCartItem>();
        //foreach(ShoppingCart cartitem in cart)
        //{

        //       // _context.Products.Where(x => x.ProductId == cartitem.ProductId).FirstOrDefault();
        //    returnCartItem addCartItem = new returnCartItem
        //    {
        //        product = cartitem.Product,
        //        Quantity = cartitem.Quantity,
        //    };
        //    returncart.Add(addCartItem);
        //}

    

        public async Task UpdateCart(ShoppingCart updateCart)
        {
            //var getUserId = _context.AspNetUsers.Where(x => x.UserName == updateCart.UserId).Select(x => x.Id).FirstOrDefault();
            //updateCart.UserId = getUserId;
            var getCartId = _context.ShoppingCarts.Where(x => x.UserId == updateCart.UserId && x.ProductId == updateCart.ProductId).FirstOrDefault();
            getCartId.Quantity += updateCart.Quantity;


            _context.Entry(getCartId).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }



        public async Task DeleteCart(List<ProductIdOnly> products, string username)
        {
            var getUserId = _context.AspNetUsers.Where(x => x.UserName == username).Select(x => x.Id).FirstOrDefault();
            foreach(ProductIdOnly product in products) {
                ShoppingCart getCart = _context.ShoppingCarts.Where(x => x.UserId == getUserId && x.ProductId == product.ProductId).FirstOrDefault();
                _context.ShoppingCarts.Remove(getCart);

            }
            
            await _context.SaveChangesAsync();
        }

        public async Task<List<Response>> PayCart(List<ShoppingCart> shoppingCarts)
        {
            List<Response> r = new List<Response>();
            return r;
            
            //Response response = new Response();
            //if (shoppingCarts != null)
            //{
            //    var getUserId = _context.AspNetUsers.Where(x => x.UserName == shoppingCarts[0].UserId).Select(x => x.Id).FirstOrDefault();
            //    //foreach (ShoppingCart shoppingCart1 in shoppingCarts)
            //    //{
            //    //    var getcartId = _context.ShoppingCarts.Where(x => x.UserId == getUserId && x.ProductId == shoppingCart1.ProductId).Select(x => x.CartId).FirstOrDefault();
            //    //    shoppingCart1.CartId = getcartId;
            //    //}

            //    Order order = new Order
            //    {
            //        UserId = getUserId,
            //        OrderDate = DateTime.Now,
            //    };
            //    _context.Orders.Add(order);
            //    await _context.SaveChangesAsync();
               

            //    //_context.Entry(order).State = EntityState.Added;
            //    foreach (ShoppingCart shoppingCart in shoppingCarts)
            //    {
                    
            //        OrderDetail od = new OrderDetail();
            //        od.OrderId = order.OrderId;
            //        od.ProductId = shoppingCart.ProductId;
            //        od.Quantity = shoppingCart.Quantity;
            //        var getcartId = _context.ShoppingCarts.Where(x => x.UserId == getUserId && x.ProductId == shoppingCart.ProductId).Select(x => x.CartId).FirstOrDefault();
            //        shoppingCart.CartId = getcartId;
            //        var product = _context.Products.Where(x=>x.ProductId==od.ProductId).FirstOrDefault();
                    
            //        if (product.Stock >= od.Quantity)
            //        {
            //            product.Stock -= od.Quantity;
            //            if (product.Stock == 0)
            //            {
            //                product.Status = 2;
            //            }
            //            response.Status = "Success";
            //            response.Message = "Success";
            //            _context.Products.Update(product);

            //            _context.OrderDetails.Add(od);
            //            // _context.Entry(od).State = EntityState.Added;
            //            //await _context.SaveChangesAsync();
            //            // _context.Entry(shoppingCart).State = EntityState.Deleted;
            //            _context.ShoppingCarts.Remove(shoppingCart);
            //        }
            //        else if(product.Stock<=0)
            //        {
            //            response.Status = "Fail";
            //            response.Message = "Out of stock";
                       
            //        }
            //        else
            //        {
            //            response.Status = "Fail";
            //            response.Message = "The Product, " + product.ProductName + "quantity have exceed the stock, please delete the item and buy again or unselect this item to pay";
                       
            //        }
                    
                    
            //    }
            //    await _context.SaveChangesAsync();
               

            //}
            //return response;
        }



    }
}
