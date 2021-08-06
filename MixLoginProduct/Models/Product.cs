using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace MixLoginProduct.Models
{
    [Table("Product")]
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
            ShoppingCarts = new HashSet<ShoppingCart>();
        }

        [Key]
        [Column("productId")]
        public int ProductId { get; set; }
        [Required]
        [Column("productName")]
        [StringLength(50)]
        public string ProductName { get; set; }
        [Column("price", TypeName = "money")]
        public decimal Price { get; set; }
        [Required]
        [Column("description")]
        [StringLength(250)]
        public string Description { get; set; }
        [Required]
        [Column("image")]
        [StringLength(250)]
        public string Image { get; set; }
        [Column("stock")]
        public int Stock { get; set; }
        [Column("status")]
        public int Status { get; set; }

        [ForeignKey(nameof(Status))]
        [InverseProperty(nameof(ProductStatus.Products))]
        public virtual ProductStatus StatusNavigation { get; set; }
        [InverseProperty(nameof(OrderDetail.Product))]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        [InverseProperty(nameof(ShoppingCart.Product))]
        public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; }
    }

    public class ProductUpdateRequest
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public IFormFile ImageFile { get; set; }
        public int Stock { get; set; }
        public int Status { get; set; }

    }

    public class ProductAddModel
    {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
        public int Stock { get; set; }
        public int Status { get; set; }
    }

    public class ProductIdOnly
    {
        public int ProductId { get; set; }
    }

    public class ProductWithStatusName
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int Stock { get; set; }
        public string Status { get; set; }
    }

    
}

