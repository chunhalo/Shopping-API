using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace MixLoginProduct.Models
{
    [Table("Product_Status")]
    public partial class ProductStatus
    {
        public ProductStatus()
        {
            Products = new HashSet<Product>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("Status_Name")]
        [StringLength(250)]
        public string StatusName { get; set; }

        [InverseProperty(nameof(Product.StatusNavigation))]
        public virtual ICollection<Product> Products { get; set; }
    }
}
