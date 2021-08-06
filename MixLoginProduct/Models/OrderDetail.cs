using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace MixLoginProduct.Models
{
    [Table("Order_Detail")]
    public partial class OrderDetail
    {
        [Column("orderId")]
        public int OrderId { get; set; }
        [Column("productId")]
        public int ProductId { get; set; }
        [Column("quantity")]
        public int Quantity { get; set; }
        [Key]
        [Column("detail_id")]
        public int DetailId { get; set; }
        [JsonIgnore]
        [ForeignKey(nameof(OrderId))]
        [InverseProperty("OrderDetails")]
        public virtual Order Order { get; set; }
        [JsonIgnore]
        [ForeignKey(nameof(ProductId))]
        [InverseProperty("OrderDetails")]
        public virtual Product Product { get; set; }
    }

    public class returnOrderDetail
    {
        public int orderId { get; set; }
        public Product product { get; set; }
        public int quantity { get; set; }
    }
}
