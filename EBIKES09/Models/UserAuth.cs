using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EBIKES09.Models;

[Table("user_auth")]
[Index("UserId", "Login", Name = "user_auth_user_id_login_key", IsUnique = true)]
public partial class UserAuth
{
    [Key]
    [Column("user_auth_id")]
    public int UserAuthId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("login")]
    [StringLength(255)]
    public string? Login { get; set; }

    [Column("password")]
    [StringLength(255)]
    public string? Password { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserAuths")]
    public virtual User User { get; set; } = null!;
}
