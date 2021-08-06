using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace MixLoginProduct.Models
{
    [Table("ShoppingCart")]
    public partial class ShoppingCart
    {
        [Required]
        [Column("userId")]
        [StringLength(450)]
        public string UserId { get; set; }
       
        [Column("productId")]

        public int ProductId { get; set; }
        [Column("quantity")]
        public int Quantity { get; set; }
        [Key]
        [Column("cartId")]
        public int CartId { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(ProductId))]
        [InverseProperty("ShoppingCarts")]
        public virtual Product Product { get; set; }
        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(AspNetUser.ShoppingCarts))]
        public virtual AspNetUser User { get; set; }
    }

    public class ShoppingCartItem
    {

        public int productId { get; set; }
        public int Quantity { get; set; }
    }

    public class returnCartItem
    {
        public Product product { get; set; }
        public int Quantity { get; set; }
        public bool isSelected { get; set; }

    }
}
