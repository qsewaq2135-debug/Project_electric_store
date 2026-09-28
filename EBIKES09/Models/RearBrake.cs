using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EBIKES09.Models;

[Table("rear_brakes")]
public partial class RearBrake
{
    [Key]
    [Column("rear_brakes_id")]
    public int RearBrakesId { get; set; }

    [Column("rear_brakes_name")]
    [StringLength(50)]
    public string? RearBrakesName { get; set; }

    [InverseProperty("RearBrakes")]
    public virtual ICollection<BikeCharacteristic> BikeCharacteristics { get; set; } = new List<BikeCharacteristic>();
}
