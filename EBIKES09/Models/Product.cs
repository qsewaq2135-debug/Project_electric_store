using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EBIKES09.Models;

[Table("products")]
public partial class Product
{
    [Key]
    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("product_name")]
    [StringLength(255)]
    public string? ProductName { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("category_id")]
    public int? CategoryId { get; set; }

    [Column("brand_id")]
    public int? BrandId { get; set; }

    [Column("price")]
    [Precision(10, 2)]
    public decimal? Price { get; set; }

    [Column("stock_quantity")]
    public int? StockQuantity { get; set; }

    [Column("image_url")]
    [StringLength(255)]
    public string? ImageUrl { get; set; }

    [Column("is_active")]
    public bool? IsActive { get; set; }

    [InverseProperty("Product")]
    public virtual BatteryCharacteristic? BatteryCharacteristic { get; set; }

    [InverseProperty("Product")]
    public virtual BikeCharacteristic? BikeCharacteristic { get; set; }

    [ForeignKey("BrandId")]
    [InverseProperty("Products")]
    public virtual Brand? Brand { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    [ForeignKey("CategoryId")]
    [InverseProperty("Products")]
    public virtual Category? Category { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<OrderDetil> OrderDetils { get; set; } = new List<OrderDetil>();

    [InverseProperty("Product")]
    public virtual ICollection<PromotionProduct> PromotionProducts { get; set; } = new List<PromotionProduct>();

    [InverseProperty("Product")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public decimal DiscountedPrice
    {
        get
        {
            var activePromotion = PromotionProducts
                  .Where(pp => pp.Promotion != null && pp.Promotion.EndDate >= DateTime.Now && pp.Promotion.StartDate <= DateTime.Now)
                 .FirstOrDefault()?.Promotion;
            if (activePromotion != null)
            {
                if (activePromotion.DiscountType == "percentage")
                {
                    decimal originalPrice = Price.GetValueOrDefault();
                    decimal discountPercentage = activePromotion.DiscountValue.GetValueOrDefault() / 100;
                    return originalPrice * (1 - discountPercentage);
                }
                else if (activePromotion.DiscountType == "fixed")
                {
                    decimal originalPrice = Price.GetValueOrDefault();
                    return originalPrice - activePromotion.DiscountValue.GetValueOrDefault();
                }
            }
            return Price.GetValueOrDefault();
        }
    }
    [NotMapped]
    public bool HasActivePromotion
    {
        get
        {
            return PromotionProducts.Any(pp => pp.Promotion != null && pp.Promotion.EndDate >= DateTime.Now && pp.Promotion.StartDate <= DateTime.Now);
        }
    }

    public decimal? AverageRating
    {
        get
        {
            if (Reviews != null && Reviews.Any())
            {
                // Вычисляем средний рейтинг
                return (decimal?)Reviews.Average(r => r.Rating);
            }
            return null; // Если отзывов нет, возвращаем null
        }
    }
}
