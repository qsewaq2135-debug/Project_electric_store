using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EBIKES09.Models;

[Table("battery_types")]
public partial class BatteryType
{
    [Key]
    [Column("battery_type_id")]
    public int BatteryTypeId { get; set; }

    [Column("battery_type")]
    [StringLength(255)]
    public string? BatteryType1 { get; set; }

    [InverseProperty("BatteryType")]
    public virtual ICollection<BatteryCharacteristic> BatteryCharacteristics { get; set; } = new List<BatteryCharacteristic>();
}
