using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EBIKES09.Models;

[Table("promotion_products")]
public partial class PromotionProduct
{
    [Key]
    [Column("promotion_product_id")]
    public int PromotionProductId { get; set; }

    [Column("promotion_id")]
    public int? PromotionId { get; set; }

    [Column("product_id")]
    public int? ProductId { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("PromotionProducts")]
    public virtual Product? Product { get; set; }

    [ForeignKey("PromotionId")]
    [InverseProperty("PromotionProducts")]
    public virtual Promotion? Promotion { get; set; }
}
