using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MixLoginProduct.Authentication;
using MixLoginProduct.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MixLoginProduct.Interface
{
    public class Service:IProductRepo,ICartRepo,IOrderRepo
    {
        private readonly EcommerceContext _context;
        public static IWebHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public Service(EcommerceContext context, IWebHostEnvironment environment, UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager, IConfiguration configuration, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _environment = environment;
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            _signInManager = signInManager;
        }

        //Product Section
        public async Task<PagedList<List<Product>>> GetActiveProducts(PaginationFilter paginationFilter)
        {
            var pagedata = await _context.Products.Where(x => x.Status == 1).Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                .Take(paginationFilter.PageSize).ToListAsync();
            var totalRecords = await _context.Products.Where(x=>x.Status==1).CountAsync();

            return new PagedList<List<Product>>(pagedata, paginationFilter.PageNumber, paginationFilter.PageSize, totalRecords);
        }

        public async Task<PagedList<List<ProductWithStatusName>>> GetProducts(PaginationFilter paginationFilter)
        {
            var pagedata = await _context.Products.Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                .Take(paginationFilter.PageSize).Select(x => new ProductWithStatusName
                {
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    Price = x.Price,
                    Description = x.Description,
                    Image = x.Image,
                    Status = x.StatusNavigation.StatusName,
                    Stock = x.Stock
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

        //Cart Section
        public async Task AddCart(ShoppingCart newCart)
        {
           // Response response = new Response();
            var getUserId = _context.AspNetUsers.Where(x => x.UserName == newCart.UserId).Select(x => x.Id).FirstOrDefault();
            newCart.UserId = getUserId;

            if (_context.ShoppingCarts.Any(x => x.UserId == newCart.UserId && x.ProductId == newCart.ProductId))
            {

                await UpdateCart(newCart);
            }
            else
            {
                //response.Status = "AddCart";
                //response.Message = "Add successfully";
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
            foreach (ProductIdOnly product in products)
            {
                ShoppingCart getCart = _context.ShoppingCarts.Where(x => x.UserId == getUserId && x.ProductId == product.ProductId).FirstOrDefault();
                _context.ShoppingCarts.Remove(getCart);

            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<Response>> PayCart(List<ShoppingCart> shoppingCarts)
        {
            int testing = 1;
            List<Response> responses = new List<Response>();
            if (shoppingCarts != null)
            {
                var getUserId = _context.AspNetUsers.Where(x => x.UserName == shoppingCarts[0].UserId).Select(x => x.Id).FirstOrDefault();
                //foreach (ShoppingCart shoppingCart1 in shoppingCarts)
                //{
                //    var getcartId = _context.ShoppingCarts.Where(x => x.UserId == getUserId && x.ProductId == shoppingCart1.ProductId).Select(x => x.CartId).FirstOrDefault();
                //    shoppingCart1.CartId = getcartId;
                //}


                Order order = new Order
                {
                    UserId = getUserId,
                    OrderDate = DateTime.Now,
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                List<ShoppingCart> checkcart = new List<ShoppingCart>();

                //_context.Entry(order).State = EntityState.Added;
                foreach (ShoppingCart shoppingCart in shoppingCarts)
                {

                    OrderDetail od = new OrderDetail();
                    od.OrderId = order.OrderId;
                    od.ProductId = shoppingCart.ProductId;
                    od.Quantity = shoppingCart.Quantity;
                    var getcartId = _context.ShoppingCarts.Where(x => x.UserId == getUserId && x.ProductId == shoppingCart.ProductId).Select(x => x.CartId).FirstOrDefault();
                    shoppingCart.CartId = getcartId;
                    var product = _context.Products.Where(x => x.ProductId == od.ProductId).FirstOrDefault();

                    if (product.Stock >= od.Quantity)
                    {

                        product.Stock -= od.Quantity;
                        if (product.Stock == 0)
                        {
                            product.Status = 2;
                        }
                        //Response response = new Response();
                        //response.Status = "Success";
                        //response.Message = "Success";
                        //responses.Add(response);
                        _context.Products.Update(product);

                        _context.OrderDetails.Add(od);
                        checkcart.Add(shoppingCart);
                       
                    }
                    else if (product.Stock <= 0)
                    {
                        Response response = new Response();
                        testing = 2;
                        response.Status = "OutOfStock";
                        response.Message = "The Product With Name, "+product.ProductName+" is Out of stock";
                        responses.Add(response);


                    }
                    else
                    {
                        Response response = new Response();
                        testing = 2;
                        response.Status = "Fail";
                        response.Message = "The Product With Name, " + product.ProductName + " quantity have exceed the stock, please delete the item and buy again or unselect this item to pay";
                        responses.Add(response);


                    }


                }

                if (testing == 1)
                {
                    foreach (ShoppingCart sc in checkcart)
                    {

                        _context.ShoppingCarts.Remove(sc);
                        await _context.SaveChangesAsync();

                    }
                    
                    checkcart.Clear();

                }
                if (testing == 2)
                {
                    _context.Orders.Remove(order);
                    await _context.SaveChangesAsync();
                   
                }

            }
            //await _context.SaveChangesAsync();
            return responses;
        }

        //Order Section
        public async Task<PagedList<List<returnOrder>>> PurChaseHistory(string username,PaginationFilter paginationFilter)
        {
            var getUserId = _context.AspNetUsers.Where(x => x.UserName == username).Select(x => x.Id).FirstOrDefault();
            var pagedata = _context.Orders.Where(x => x.UserId == getUserId).Select(x =>
                new returnOrder { OrderId = x.OrderId, OrderDate = x.OrderDate }).Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                .Take(paginationFilter.PageSize).ToList();
            var totalRecords = _context.Orders.Where(x => x.UserId == getUserId).Count();
            return (new PagedList<List<returnOrder>>(pagedata, paginationFilter.PageNumber, paginationFilter.PageSize, totalRecords));
        }

        public async Task<List<returnOrderDetail>> TrackPurchaseId(string username, int orderId)
        {
            var getUserId = _context.AspNetUsers.Where(x => x.UserName == username).Select(x => x.Id).FirstOrDefault();
            List<returnOrderDetail> orderDetails = _context.OrderDetails.Where(x => x.OrderId == orderId).Include(x => x.Product)
                .Select(x => new returnOrderDetail
                {
                    orderId = x.OrderId,
                    product = x.Product,
                    quantity = x.Quantity
                }).ToList();
            return orderDetails;
        }



    }
}

