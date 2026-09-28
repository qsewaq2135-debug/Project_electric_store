using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EBIKES09.Models;

[Table("promotions")]
public partial class Promotion
{
    [Key]
    [Column("promotion_id")]
    public int PromotionId { get; set; }

    [Column("promotion_name")]
    [StringLength(255)]
    public string? PromotionName { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("start_date", TypeName = "timestamp without time zone")]
    public DateTime? StartDate { get; set; }

    [Column("end_date", TypeName = "timestamp without time zone")]
    public DateTime? EndDate { get; set; }

    [Column("discount_value")]
    [Precision(10, 2)]
    public decimal? DiscountValue { get; set; }

    [Column("discount_type")]
    [StringLength(255)]
    public string? DiscountType { get; set; }

    [InverseProperty("Promotion")]
    public virtual ICollection<PromotionProduct> PromotionProducts { get; set; } = new List<PromotionProduct>();
}
