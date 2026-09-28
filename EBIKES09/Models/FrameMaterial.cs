using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EBIKES09.Models;

[Table("frame_material")]
public partial class FrameMaterial
{
    [Key]
    [Column("frame_material_id")]
    public int FrameMaterialId { get; set; }

    [Column("frame_material_name")]
    [StringLength(50)]
    public string? FrameMaterialName { get; set; }

    [InverseProperty("FrameMaterial")]
    public virtual ICollection<BikeCharacteristic> BikeCharacteristics { get; set; } = new List<BikeCharacteristic>();
}
