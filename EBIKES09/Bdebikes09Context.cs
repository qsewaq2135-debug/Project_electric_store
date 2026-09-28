using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using EBIKES09.Models;

namespace EBIKES09;

public partial class Bdebikes09Context : DbContext
{
    public Bdebikes09Context()
    {
    }

    public Bdebikes09Context(DbContextOptions<Bdebikes09Context> options)
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

            entity.ToTable("battery_characteristics");

            entity.HasIndex(e => e.ProductId, "battery_characteristics_product_id_key").IsUnique();

            entity.Property(e => e.BatteryCharacteristicsId).HasColumnName("battery_characteristics_id");
            entity.Property(e => e.BatteryTypeId).HasColumnName("battery_type_id");
            entity.Property(e => e.NominalCapacity).HasColumnName("nominal_capacity");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ServiceLife).HasColumnName("service_life");
            entity.Property(e => e.VoltageId).HasColumnName("voltage_id");

            entity.HasOne(d => d.BatteryType).WithMany(p => p.BatteryCharacteristics)
                .HasForeignKey(d => d.BatteryTypeId)
                .HasConstraintName("battery_type_id");

            entity.HasOne(d => d.Product).WithOne(p => p.BatteryCharacteristic)
                .HasForeignKey<BatteryCharacteristic>(d => d.ProductId)
                .HasConstraintName("battery_characteristics_product_id_fkey");

            entity.HasOne(d => d.Voltage).WithMany(p => p.BatteryCharacteristics)
                .HasForeignKey(d => d.VoltageId)
                .HasConstraintName("voltage_id");
        });

        modelBuilder.Entity<BatteryType>(entity =>
        {
            entity.HasKey(e => e.BatteryTypeId).HasName("battery_types_pkey");

            entity.ToTable("battery_types");

            entity.Property(e => e.BatteryTypeId).HasColumnName("battery_type_id");
            entity.Property(e => e.BatteryType1)
                .HasMaxLength(255)
                .HasColumnName("battery_type");
        });

        modelBuilder.Entity<BikeCharacteristic>(entity =>
        {
            entity.HasKey(e => e.BikeCharacteristicsId).HasName("bike_characteristics_pkey");

            entity.ToTable("bike_characteristics");

            entity.HasIndex(e => e.ProductId, "bike_characteristics_product_id_key").IsUnique();

            entity.Property(e => e.BikeCharacteristicsId).HasColumnName("bike_characteristics_id");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.Dimensions)
                .HasMaxLength(50)
                .HasColumnName("dimensions");
            entity.Property(e => e.DriveId).HasColumnName("drive_id");
            entity.Property(e => e.FrameMaterialId).HasColumnName("frame_material_id");
            entity.Property(e => e.FrontBrakesId).HasColumnName("front_brakes_id");
            entity.Property(e => e.FullChargeTime).HasColumnName("full_charge_time");
            entity.Property(e => e.MaxLoad).HasColumnName("max_load");
            entity.Property(e => e.MaximumRange).HasColumnName("maximum_range");
            entity.Property(e => e.MaximumSpeed).HasColumnName("maximum_speed");
            entity.Property(e => e.MotorPower).HasColumnName("motor_power");
            entity.Property(e => e.NumberOfSpeeds).HasColumnName("number_of_speeds");
            entity.Property(e => e.PackageDimensions)
                .HasMaxLength(50)
                .HasColumnName("package_dimensions");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.RearBrakesId).HasColumnName("rear_brakes_id");
            entity.Property(e => e.SoundSignal).HasColumnName("sound_signal");
            entity.Property(e => e.Voltage).HasColumnName("voltage");
            entity.Property(e => e.Weight)
                .HasMaxLength(50)
                .HasColumnName("weight");
            entity.Property(e => e.WheelDiameter).HasColumnName("wheel_diameter");
            entity.Property(e => e.Wings).HasColumnName("wings");

            entity.HasOne(d => d.Drive).WithMany(p => p.BikeCharacteristics)
                .HasForeignKey(d => d.DriveId)
                .HasConstraintName("drive_id");

            entity.HasOne(d => d.FrameMaterial).WithMany(p => p.BikeCharacteristics)
                .HasForeignKey(d => d.FrameMaterialId)
                .HasConstraintName("bike_characteristics_frame_material_id_fkey");

            entity.HasOne(d => d.FrontBrakes).WithMany(p => p.BikeCharacteristics)
                .HasForeignKey(d => d.FrontBrakesId)
                .HasConstraintName("bike_characteristics_front_brakes_id_fkey");

            entity.HasOne(d => d.Product).WithOne(p => p.BikeCharacteristic)
                .HasForeignKey<BikeCharacteristic>(d => d.ProductId)
                .HasConstraintName("bike_characteristics_product_id_fkey");

            entity.HasOne(d => d.RearBrakes).WithMany(p => p.BikeCharacteristics)
                .HasForeignKey(d => d.RearBrakesId)
                .HasConstraintName("bike_characteristics_rear_brakes_id_fkey");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("brand_pkey");

            entity.ToTable("brand");

            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.BrandName)
                .HasMaxLength(100)
                .HasColumnName("brand_name");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("cart_pkey");

            entity.ToTable("cart");

            entity.Property(e => e.CartId).HasColumnName("cart_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cart_user_id_fkey");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.CartItemId).HasName("cart_items_pkey");

            entity.ToTable("cart_items");

            entity.Property(e => e.CartItemId).HasColumnName("cart_item_id");
            entity.Property(e => e.CartId).HasColumnName("cart_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(10, 2)
                .HasColumnName("unit_price");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cart_items_cart_id_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cart_items_product_id_fkey");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("category_pkey");

            entity.ToTable("category");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(30)
                .HasColumnName("category_name");
        });

        modelBuilder.Entity<Drive>(entity =>
        {
            entity.HasKey(e => e.DriveId).HasName("drive_pkey");

            entity.ToTable("drive");

            entity.Property(e => e.DriveId).HasColumnName("drive_id");
            entity.Property(e => e.DriveName)
                .HasMaxLength(255)
                .HasColumnName("drive_name");
        });

        modelBuilder.Entity<FrameMaterial>(entity =>
        {
            entity.HasKey(e => e.FrameMaterialId).HasName("frame_material_pkey");

            entity.ToTable("frame_material");

            entity.Property(e => e.FrameMaterialId).HasColumnName("frame_material_id");
            entity.Property(e => e.FrameMaterialName)
                .HasMaxLength(50)
                .HasColumnName("frame_material_name");
        });

        modelBuilder.Entity<FrontBrake>(entity =>
        {
            entity.HasKey(e => e.FrontBrakesId).HasName("front_brakes_pkey");

            entity.ToTable("front_brakes");

            entity.Property(e => e.FrontBrakesId).HasColumnName("front_brakes_id");
            entity.Property(e => e.FrontBrakesName)
                .HasMaxLength(50)
                .HasColumnName("front_brakes_name");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("orders_pkey");

            entity.ToTable("orders");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Discription)
                .HasMaxLength(255)
                .HasColumnName("discription");
            entity.Property(e => e.OrderDate).HasColumnName("order_date");
            entity.Property(e => e.OrderDateEnd).HasColumnName("order_date_end");
            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(20, 2)
                .HasColumnName("total_amount");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Payment).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("orders_payment_id_fkey");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("status_id");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("orders_user_id_fkey");
        });

        modelBuilder.Entity<OrderDetil>(entity =>
        {
            entity.HasKey(e => e.OrderDetilId).HasName("order_detils_pkey");

            entity.ToTable("order_detils");

            entity.Property(e => e.OrderDetilId).HasColumnName("order_detil_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.TotalPrice)
                .HasPrecision(10, 2)
                .HasColumnName("total_price");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(10, 2)
                .HasColumnName("unit_price");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetils)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("order_detils_order_id_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetils)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("order_detils_product_id_fkey");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("payment_pkey");

            entity.ToTable("payment");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.PaymentName)
                .HasMaxLength(30)
                .HasColumnName("payment_name");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("products_pkey");

            entity.ToTable("products");

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("image_url");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.ProductName)
                .HasMaxLength(255)
                .HasColumnName("product_name");
            entity.Property(e => e.StockQuantity).HasColumnName("stock_quantity");

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("products_brand_id_fkey");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("products_category_id_fkey");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.PromotionId).HasName("promotions_pkey");

            entity.ToTable("promotions");

            entity.Property(e => e.PromotionId).HasColumnName("promotion_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DiscountType)
                .HasMaxLength(255)
                .HasColumnName("discount_type");
            entity.Property(e => e.DiscountValue)
                .HasPrecision(10, 2)
                .HasColumnName("discount_value");
            entity.Property(e => e.EndDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("end_date");
            entity.Property(e => e.PromotionName)
                .HasMaxLength(255)
                .HasColumnName("promotion_name");
            entity.Property(e => e.StartDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("start_date");
        });

        modelBuilder.Entity<PromotionProduct>(entity =>
        {
            entity.HasKey(e => e.PromotionProductId).HasName("promotion_products_pkey");

            entity.ToTable("promotion_products");

            entity.Property(e => e.PromotionProductId).HasColumnName("promotion_product_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.PromotionId).HasColumnName("promotion_id");

            entity.HasOne(d => d.Product).WithMany(p => p.PromotionProducts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("promotion_products_product_id_fkey");

            entity.HasOne(d => d.Promotion).WithMany(p => p.PromotionProducts)
                .HasForeignKey(d => d.PromotionId)
                .HasConstraintName("promotion_products_promotion_id_fkey");
        });

        modelBuilder.Entity<RearBrake>(entity =>
        {
            entity.HasKey(e => e.RearBrakesId).HasName("rear_brakes_pkey");

            entity.ToTable("rear_brakes");

            entity.Property(e => e.RearBrakesId).HasColumnName("rear_brakes_id");
            entity.Property(e => e.RearBrakesName)
                .HasMaxLength(50)
                .HasColumnName("rear_brakes_name");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("reviews_pkey");

            entity.ToTable("reviews");

            entity.Property(e => e.ReviewId).HasColumnName("review_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("reviews_product_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("reviews_user_id_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(30)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("status_pkey");

            entity.ToTable("status");

            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(255)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(11)
                .HasColumnName("phone_number");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("users_role_id_fkey");
        });

        modelBuilder.Entity<UserAuth>(entity =>
        {
            entity.HasKey(e => e.UserAuthId).HasName("user_auth_pkey");

            entity.ToTable("user_auth");

            entity.HasIndex(e => new { e.UserId, e.Login }, "user_auth_user_id_login_key").IsUnique();

            entity.Property(e => e.UserAuthId).HasColumnName("user_auth_id");
            entity.Property(e => e.Login)
                .HasMaxLength(255)
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserAuths)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_auth_user_id_fkey");
        });

        modelBuilder.Entity<Voltage>(entity =>
        {
            entity.HasKey(e => e.VoltageId).HasName("voltages_pkey");

            entity.ToTable("voltages");

            entity.Property(e => e.VoltageId).HasColumnName("voltage_id");
            entity.Property(e => e.VoltageName).HasColumnName("voltage_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

