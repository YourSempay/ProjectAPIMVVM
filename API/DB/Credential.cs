using System;
using System.Collections.Generic;

namespace API.DB;

public partial class Credential
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public string? Username { get; set; }

    public string? PasswordHash { get; set; }

    public int RoleId { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
