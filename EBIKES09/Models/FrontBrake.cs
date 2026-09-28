using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EBIKES09.Models;

[Table("front_brakes")]
public partial class FrontBrake
{
    [Key]
    [Column("front_brakes_id")]
    public int FrontBrakesId { get; set; }

    [Column("front_brakes_name")]
    [StringLength(50)]
    public string? FrontBrakesName { get; set; }

    [InverseProperty("FrontBrakes")]
    public virtual ICollection<BikeCharacteristic> BikeCharacteristics { get; set; } = new List<BikeCharacteristic>();
}
