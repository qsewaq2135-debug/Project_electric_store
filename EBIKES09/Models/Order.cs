using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EBIKES09.Models;

[Table("orders")]
public partial class Order
{
    [Key]
    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("order_date")]
    public DateOnly? OrderDate { get; set; }

    [Column("total_amount")]
    [Precision(20, 2)]
    public decimal? TotalAmount { get; set; }

    [Column("payment_id")]
    public int? PaymentId { get; set; }

    [Column("discription")]
    [StringLength(255)]
    public string? Discription { get; set; }

    [Column("status_id")]
    public int? StatusId { get; set; }

    [Column("order_date_end")]
    public DateOnly? OrderDateEnd { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<OrderDetil> OrderDetils { get; set; } = new List<OrderDetil>();

    [ForeignKey("PaymentId")]
    [InverseProperty("Orders")]
    public virtual Payment? Payment { get; set; }

    [ForeignKey("StatusId")]
    [InverseProperty("Orders")]
    public virtual Status? Status { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Orders")]
    public virtual User? User { get; set; }
}
