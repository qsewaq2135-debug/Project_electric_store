using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EBIKES09.Models;

[Table("order_detils")]
public partial class OrderDetil
{
    [Key]
    [Column("order_detil_id")]
    public int OrderDetilId { get; set; }

    [Column("order_id")]
    public int? OrderId { get; set; }

    [Column("product_id")]
    public int? ProductId { get; set; }

    [Column("quantity")]
    public int? Quantity { get; set; }

    [Column("unit_price")]
    [Precision(10, 2)]
    public decimal? UnitPrice { get; set; }

    [Column("total_price")]
    [Precision(10, 2)]
    public decimal? TotalPrice { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("OrderDetils")]
    public virtual Order? Order { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("OrderDetils")]
    public virtual Product? Product { get; set; }
}
