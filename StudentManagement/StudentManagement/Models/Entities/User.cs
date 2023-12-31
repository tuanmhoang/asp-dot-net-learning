﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagement.Models.Entities;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string PasswordSalt { get; set; } = null!;

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public byte[]? Photo { get; set; }

    public int? Roleid { get; set; }

    public virtual Role? Role { get; set; }
}
