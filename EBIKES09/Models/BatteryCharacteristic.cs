using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EBIKES09.Models;

[Table("battery_characteristics")]
[Index("ProductId", Name = "battery_characteristics_product_id_key", IsUnique = true)]
public partial class BatteryCharacteristic
{
    [Key]
    [Column("battery_characteristics_id")]
    public int BatteryCharacteristicsId { get; set; }

    [Column("product_id")]
    public int? ProductId { get; set; }

    [Column("voltage_id")]
    public int? VoltageId { get; set; }

    [Column("battery_type_id")]
    public int? BatteryTypeId { get; set; }

    [Column("nominal_capacity")]
    public int? NominalCapacity { get; set; }

    [Column("service_life")]
    public int? ServiceLife { get; set; }

    [ForeignKey("BatteryTypeId")]
    [InverseProperty("BatteryCharacteristics")]
    public virtual BatteryType? BatteryType { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("BatteryCharacteristic")]
    public virtual Product? Product { get; set; }

    [ForeignKey("VoltageId")]
    [InverseProperty("BatteryCharacteristics")]
    public virtual Voltage? Voltage { get; set; }
}
