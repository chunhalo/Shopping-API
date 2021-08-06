using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class OrderController : ControllerBase
    {

        private readonly IOrderRepo _orderRepo;


        public OrderController(IOrderRepo orderRepo)
        {

            _orderRepo = orderRepo;
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<List<returnOrder>>>> PurchaseHistory([FromQuery] PaginationFilter paginationFilter)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                string username = User.FindFirstValue(ClaimTypes.Name);
                PagedList<List<returnOrder>> orders = await _orderRepo.PurChaseHistory(username,paginationFilter);
                return Ok(orders);
            }
            return Unauthorized();
        }

        [HttpGet("{id}")]
        [Route("trackId")]
        public async Task<ActionResult<returnOrderDetail>> GetTrackId([FromQuery] int id)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                string username = User.FindFirstValue(ClaimTypes.Name);
                List<returnOrderDetail> orderDetails = await _orderRepo.TrackPurchaseId(username,id);
                return Ok(orderDetails);
            }
            return Unauthorized();
        }
    }
}
