using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EBIKES09.Models;

[Table("drive")]
public partial class Drive
{
    [Key]
    [Column("drive_id")]
    public int DriveId { get; set; }

    [Column("drive_name")]
    [StringLength(255)]
    public string? DriveName { get; set; }

    [InverseProperty("Drive")]
    public virtual ICollection<BikeCharacteristic> BikeCharacteristics { get; set; } = new List<BikeCharacteristic>();
}
