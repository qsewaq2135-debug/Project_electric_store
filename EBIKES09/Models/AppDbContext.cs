using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EBIKES09.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BatteryCharacteristic> BatteryCharacteristics { get; set; }

    public virtual DbSet<BatteryType> BatteryTypes { get; set; }

    public virtual DbSet<BikeCharacteristic> BikeCharacteristics { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Drive> Drives { get; set; }

    public virtual DbSet<FrameMaterial> FrameMaterials { get; set; }

    public virtual DbSet<FrontBrake> FrontBrakes { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetil> OrderDetils { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<PromotionProduct> PromotionProducts { get; set; }

    public virtual DbSet<RearBrake> RearBrakes { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAuth> UserAuths { get; set; }

    public virtual DbSet<Voltage> Voltages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=BDEBIKES09;Username=postgres;Password=123");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BatteryCharacteristic>(entity =>
        {
            entity.HasKey(e => e.BatteryCharacteristicsId).HasName("battery_characteristics_pkey");

            entity.HasOne(d => d.BatteryType).WithMany(p => p.BatteryCharacteristics).HasConstraintName("battery_type_id");

            entity.HasOne(d => d.Product).WithOne(p => p.BatteryCharacteristic).HasConstraintName("battery_characteristics_product_id_fkey");

            entity.HasOne(d => d.Voltage).WithMany(p => p.BatteryCharacteristics).HasConstraintName("voltage_id");
        });

        modelBuilder.Entity<BatteryType>(entity =>
        {
            entity.HasKey(e => e.BatteryTypeId).HasName("battery_types_pkey");
        });

        modelBuilder.Entity<BikeCharacteristic>(entity =>
        {
            entity.HasKey(e => e.BikeCharacteristicsId).HasName("bike_characteristics_pkey");

            entity.HasOne(d => d.Drive).WithMany(p => p.BikeCharacteristics).HasConstraintName("drive_id");

            entity.HasOne(d => d.FrameMaterial).WithMany(p => p.BikeCharacteristics).HasConstraintName("bike_characteristics_frame_material_id_fkey");

            entity.HasOne(d => d.FrontBrakes).WithMany(p => p.BikeCharacteristics).HasConstraintName("bike_characteristics_front_brakes_id_fkey");

            entity.HasOne(d => d.Product).WithOne(p => p.BikeCharacteristic).HasConstraintName("bike_characteristics_product_id_fkey");

            entity.HasOne(d => d.RearBrakes).WithMany(p => p.BikeCharacteristics).HasConstraintName("bike_characteristics_rear_brakes_id_fkey");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("brand_pkey");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("cart_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cart_user_id_fkey");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.CartItemId).HasName("cart_items_pkey");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cart_items_cart_id_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cart_items_product_id_fkey");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("category_pkey");
        });

        modelBuilder.Entity<Drive>(entity =>
        {
            entity.HasKey(e => e.DriveId).HasName("drive_pkey");
        });

        modelBuilder.Entity<FrameMaterial>(entity =>
        {
            entity.HasKey(e => e.FrameMaterialId).HasName("frame_material_pkey");
        });

        modelBuilder.Entity<FrontBrake>(entity =>
        {
            entity.HasKey(e => e.FrontBrakesId).HasName("front_brakes_pkey");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("orders_pkey");

            entity.HasOne(d => d.Payment).WithMany(p => p.Orders).HasConstraintName("orders_payment_id_fkey");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders).HasConstraintName("status_id");

            entity.HasOne(d => d.User).WithMany(p => p.Orders).HasConstraintName("orders_user_id_fkey");
        });

        modelBuilder.Entity<OrderDetil>(entity =>
        {
            entity.HasKey(e => e.OrderDetilId).HasName("order_detils_pkey");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetils).HasConstraintName("order_detils_order_id_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetils).HasConstraintName("order_detils_product_id_fkey");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("payment_pkey");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("products_pkey");

            entity.HasOne(d => d.Brand).WithMany(p => p.Products).HasConstraintName("products_brand_id_fkey");

            entity.HasOne(d => d.Category).WithMany(p => p.Products).HasConstraintName("products_category_id_fkey");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.PromotionId).HasName("promotions_pkey");
        });

        modelBuilder.Entity<PromotionProduct>(entity =>
        {
            entity.HasKey(e => e.PromotionProductId).HasName("promotion_products_pkey");

            entity.HasOne(d => d.Product).WithMany(p => p.PromotionProducts).HasConstraintName("promotion_products_product_id_fkey");

            entity.HasOne(d => d.Promotion).WithMany(p => p.PromotionProducts).HasConstraintName("promotion_products_promotion_id_fkey");
        });

        modelBuilder.Entity<RearBrake>(entity =>
        {
            entity.HasKey(e => e.RearBrakesId).HasName("rear_brakes_pkey");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("reviews_pkey");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews).HasConstraintName("reviews_product_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews).HasConstraintName("reviews_user_id_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("roles_pkey");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("status_pkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Users).HasConstraintName("users_role_id_fkey");
        });

        modelBuilder.Entity<UserAuth>(entity =>
        {
            entity.HasKey(e => e.UserAuthId).HasName("user_auth_pkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserAuths).HasConstraintName("user_auth_user_id_fkey");
        });

        modelBuilder.Entity<Voltage>(entity =>
        {
            entity.HasKey(e => e.VoltageId).HasName("voltages_pkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
