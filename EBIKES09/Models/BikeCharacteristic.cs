using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EBIKES09.Models;

[Table("bike_characteristics")]
[Index("ProductId", Name = "bike_characteristics_product_id_key", IsUnique = true)]
public partial class BikeCharacteristic
{
    [Key]
    [Column("bike_characteristics_id")]
    public int BikeCharacteristicsId { get; set; }

    [Column("maximum_speed")]
    public int? MaximumSpeed { get; set; }

    [Column("maximum_range")]
    public int? MaximumRange { get; set; }

    [Column("max_load")]
    public int? MaxLoad { get; set; }

    [Column("motor_power")]
    public int? MotorPower { get; set; }

    [Column("voltage")]
    public int? Voltage { get; set; }

    [Column("capacity")]
    public int? Capacity { get; set; }

    [Column("full_charge_time")]
    public int? FullChargeTime { get; set; }

    [Column("frame_material_id")]
    public int? FrameMaterialId { get; set; }

    [Column("wheel_diameter")]
    public int? WheelDiameter { get; set; }

    [Column("number_of_speeds")]
    public int? NumberOfSpeeds { get; set; }

    [Column("front_brakes_id")]
    public int? FrontBrakesId { get; set; }

    [Column("rear_brakes_id")]
    public int? RearBrakesId { get; set; }

    [Column("product_id")]
    public int? ProductId { get; set; }

    [Column("sound_signal")]
    public bool? SoundSignal { get; set; }

    [Column("wings")]
    public bool? Wings { get; set; }

    [Column("dimensions")]
    [StringLength(50)]
    public string? Dimensions { get; set; }

    [Column("package_dimensions")]
    [StringLength(50)]
    public string? PackageDimensions { get; set; }

    [Column("weight")]
    [StringLength(50)]
    public string? Weight { get; set; }

    [Column("drive_id")]
    public int? DriveId { get; set; }

    [ForeignKey("DriveId")]
    [InverseProperty("BikeCharacteristics")]
    public virtual Drive? Drive { get; set; }

    [ForeignKey("FrameMaterialId")]
    [InverseProperty("BikeCharacteristics")]
    public virtual FrameMaterial? FrameMaterial { get; set; }

    [ForeignKey("FrontBrakesId")]
    [InverseProperty("BikeCharacteristics")]
    public virtual FrontBrake? FrontBrakes { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("BikeCharacteristic")]
    public virtual Product? Product { get; set; }

    [ForeignKey("RearBrakesId")]
    [InverseProperty("BikeCharacteristics")]
    public virtual RearBrake? RearBrakes { get; set; }
}
