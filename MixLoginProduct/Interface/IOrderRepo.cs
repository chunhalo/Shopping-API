using MixLoginProduct.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MixLoginProduct.Interface
{
    public interface IOrderRepo
    {
        Task<PagedList<List<returnOrder>>> PurChaseHistory(string username,PaginationFilter paginationFilter);
        Task<List<returnOrderDetail>> TrackPurchaseId(string username, int id);
        //Task<PagedList<List<returnOrderDetail>>> TrackPurchaseId(string username, int id, PaginationFilter paginationFilter);
    }
}
