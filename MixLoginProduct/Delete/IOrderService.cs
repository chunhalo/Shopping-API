using Microsoft.EntityFrameworkCore;
using MixLoginProduct.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MixLoginProduct.Interface
{
    public class IOrderService : IOrderRepo
    {
        private readonly EcommerceContext _context;


        public IOrderService(EcommerceContext context)
        {
            _context = context;
        }
        public async Task<PagedList<List<returnOrder>>> PurChaseHistory(string username,PaginationFilter paginationFilter)
        {
            var getUserId = _context.AspNetUsers.Where(x => x.UserName == username).Select(x => x.Id).FirstOrDefault();
            var pagedata = _context.Orders.Where(x => x.UserId == getUserId).Select(x =>
                new returnOrder { OrderId = x.OrderId, OrderDate = x.OrderDate }).Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                .Take(paginationFilter.PageSize).ToList();
            var totalRecords = _context.Orders.Where(x => x.UserId == getUserId).Count();
            return (new PagedList<List<returnOrder>>(pagedata, paginationFilter.PageNumber, paginationFilter.PageSize, totalRecords));
            //List <returnOrderDetail> getAllOrderDetail = new List<returnOrderDetail>();
            //foreach(Order order in orders)
            //{
            //    List<OrderDetail> orderDetails = _context.OrderDetails.Where(x => x.OrderId == order.OrderId).Include(x => x.Product).ToList();

            //    foreach(OrderDetail orderDetail in orderDetails)
            //    {

            //        returnOrderDetail returnOrderDetail = new returnOrderDetail
            //        {
            //            orderDate = order.OrderDate,
            //            product = _context.Products.Find(orderDetail.ProductId),
            //            quantity = orderDetail.Quantity,
            //        };
            //        getAllOrderDetail.Add(returnOrderDetail);
            //    }
            //}

            //return getAllOrderDetail;
        }

        public async Task<List<returnOrderDetail>> TrackPurchaseId(string username, int orderId)
        {
            var getUserId = _context.AspNetUsers.Where(x => x.UserName == username).Select(x => x.Id).FirstOrDefault();
            List<returnOrderDetail> orderDetails = _context.OrderDetails.Where(x => x.OrderId == orderId).Include(x=>x.Product)
                .Select(x=> new returnOrderDetail{ 
                    orderId=x.OrderId,
                    product= x.Product,
                    quantity=x.Quantity}).ToList();
            return orderDetails;
        }
    }
}
