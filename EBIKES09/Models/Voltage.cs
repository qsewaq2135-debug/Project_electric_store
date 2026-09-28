using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EBIKES09.Models;

[Table("voltages")]
public partial class Voltage
{
    [Key]
    [Column("voltage_id")]
    public int VoltageId { get; set; }

    [Column("voltage_name")]
    [StringLength(20)]
    public string? VoltageName { get; set; }

    [InverseProperty("Voltage")]
    public virtual ICollection<BatteryCharacteristic> BatteryCharacteristics { get; set; } = new List<BatteryCharacteristic>();
}
